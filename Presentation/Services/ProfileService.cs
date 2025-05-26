using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using Data.Entities;
using Newtonsoft.Json;
using Data.Context;
using Microsoft.EntityFrameworkCore;


namespace Presentation.Services;

public interface IProfileService
{
    Task<UserEntity?> CreateProfileAsync(UserEntity userEntity);
    Task<UserEntity?> GetProfileAsync(string userId);
    Task<bool> UpdateProfileAsync(UserEntity userEntity);
    Task<bool> DeleteProfileAsync(string userId);
}

public class ProfileService : IProfileService
{
    private readonly ILogger<ProfileService> _logger;
    private readonly AppDbContext _context;
    private readonly string? _serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnection");
    private readonly string? _queueName = Environment.GetEnvironmentVariable("UserDeletionQueueName");

    public ProfileService(ILogger<ProfileService> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<UserEntity?> CreateProfileAsync(UserEntity userEntity)
    {
        _logger.LogInformation("Creating profile for UserId: {UserId}", userEntity.UserId);
        try
        {
            // check if the user already exists
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userEntity.UserId);
            if (existing != null)
                return null; // User already exists, return null

            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();
            return userEntity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile for UserId: {UserId}", userEntity.UserId);
            return null;
        }
    }

    public async Task<UserEntity?> GetProfileAsync(string userId)
    {
        _logger.LogInformation("Getting profile for UserId: {UserId}", userId);
        return await _context.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<bool> UpdateProfileAsync(UserEntity userEntity)
    {
        _logger.LogInformation("Updating profile for UserId: {UserId}", userEntity.UserId);
        try
        {
            var existingUser = await _context.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.UserId == userEntity.UserId);
            if (existingUser == null)
                return false;

            // Uppdatera fält
            existingUser.FirstName = userEntity.FirstName;
            existingUser.LastName = userEntity.LastName;
            existingUser.Initials = userEntity.Initials;
            existingUser.Email = userEntity.Email;
            existingUser.PhoneNumber = userEntity.PhoneNumber;
            existingUser.JobTitle = userEntity.JobTitle;
            existingUser.DateOfBirth = userEntity.DateOfBirth;
            existingUser.ImageUrl = userEntity.ImageUrl;

            // Uppdatera address om den finns
            if (userEntity.Address != null)
            {
                if (existingUser.Address == null)
                {
                    existingUser.Address = userEntity.Address;
                }
                else
                {
                    existingUser.Address.StreetAddress = userEntity.Address.StreetAddress;
                    existingUser.Address.PostalCode = userEntity.Address.PostalCode;
                    existingUser.Address.City = userEntity.Address.City;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for UserId: {UserId}", userEntity.UserId);
            return false;
        }
    }

    public async Task<bool> DeleteProfileAsync(string userId)
    {
        _logger.LogInformation("Attempting to delete profile for UserId: {UserId}", userId);

        // TODO: Implement actual profile deletion logic from this service's data store (e.g., database)
        // For now, let's assume local deletion attempt is successful for the sake of queuing the message.
        bool localDeletionSuccess = true; 
        await Task.Delay(50); // Simulate local DB operation

        if (!localDeletionSuccess)
        {
            _logger.LogError("Failed to delete profile locally for UserId: {UserId}. Aborting message send.", userId);
            return false;
        }

        if (string.IsNullOrEmpty(_serviceBusConnectionString) || string.IsNullOrEmpty(_queueName))
        {
            _logger.LogError("Service Bus connection string or queue name is not configured. Cannot send delete notification.");
            // Depending on requirements, you might still return true if local deletion was the primary goal,
            // or false/throw if notification is critical.
            return false; 
        }

        try
        {
            _logger.LogInformation("Local profile deletion for UserId: {UserId} processed. Now sending notification to queue: {QueueName}", userId, _queueName);

            await using var client = new ServiceBusClient(_serviceBusConnectionString);
            await using var sender = client.CreateSender(_queueName);

            var messagePayload = new { UserId = userId };
            var jsonPayload = JsonConvert.SerializeObject(messagePayload);
            var serviceBusMessage = new ServiceBusMessage(jsonPayload)
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString() // Optional: useful for tracking
            };

            await sender.SendMessageAsync(serviceBusMessage);
            _logger.LogInformation("User deletion notification for UserId: {UserId} sent successfully to queue: {QueueName}", userId, _queueName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send user deletion notification for UserId: {UserId} to queue: {QueueName}", userId, _queueName);
            return false;
        }
    }
}
