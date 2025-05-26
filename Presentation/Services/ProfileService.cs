using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Presentation.Services;

public interface IProfileService
{
    Task<UserEntity?> CreateProfileAsync(UserEntity userEntity);
    Task<UserEntity?> GetProfileAsync(string userId);
    Task<bool> UpdateProfileAsync(UserEntity userEntity);
    Task<bool> DeleteProfileAsync(string userId);
}


public class ProfileService(ILogger<ProfileService> logger) : IProfileService
{
    private readonly ILogger<ProfileService> _logger = logger;
    private readonly string? _serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnection");
    private readonly string? _queueName = Environment.GetEnvironmentVariable("UserDeletionQueueName");

    public async Task<UserEntity?> CreateProfileAsync(UserEntity userEntity)
    {
        _logger.LogInformation("Creating profile for UserId: {UserId}", userEntity.UserId);
        // TODO: Implement actual profile creation logic (e.g., save to database)
        // For now, let's assume it's successful and return the profile
        // In a real scenario, you would interact with a data store.
        await Task.Delay(100); // Simulate async operation
        return userEntity; 
    }

    public async Task<UserEntity?> GetProfileAsync(string userId)
    {
        _logger.LogInformation("Getting profile for UserId: {UserId}", userId);
        // TODO: Implement actual profile retrieval logic (e.g., fetch from database)
        // For now, returning a null or a new UserProfile as a placeholder
        await Task.Delay(100); // Simulate async operation
        // Example: return new UserProfile { UserId = userId, FirstName = "Test", LastName = "User", Email = "test@example.com" };
        return null; 
    }

    public async Task<bool> UpdateProfileAsync(UserEntity userEntity)
    {
        _logger.LogInformation("Updating profile for UserId: {UserId}", userEntity.UserId);
        // TODO: Implement actual profile update logic (e.g., update in database)
        // For now, let's assume it's successful
        await Task.Delay(100); // Simulate async operation
        return true;
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
