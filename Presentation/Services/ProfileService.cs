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
}

public class ProfileService(ILogger<ProfileService> logger, AppDbContext context) : IProfileService
{
    private readonly ILogger<ProfileService> _logger = logger;
    private readonly AppDbContext _context = context;

    public async Task<UserEntity?> CreateProfileAsync(UserEntity userEntity)
    {
        _logger.LogInformation("Creating profile for UserId: {UserId}", userEntity.UserId);
        try
        {
            // check if the user already exists
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userEntity.UserId);
            if (existing != null)
                return null;

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
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("UserId is null or empty in GetProfileAsync");
            return null;
        }
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
}
