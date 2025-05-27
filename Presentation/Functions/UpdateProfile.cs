using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using Presentation.Helpers;
using Presentation.Services;
using Presentation.Responses;

namespace Presentation.Functions;

public class UpdateProfile(ILogger<UpdateProfile> logger, IProfileService profileService)
{
    private readonly ILogger<UpdateProfile> _logger = logger;
    private readonly IProfileService _profileService = profileService;

    [Function("UpdateProfile")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "profile")] HttpRequestData req)
    {
        try
        {
            // read Authorization-header
            var userId = AuthUtils.ExtractUserId(req);
            if (string.IsNullOrEmpty(userId))
                return new OkObjectResult(new UserResult {
                    Succeeded = false,
                    StatusCode = 401,
                    Error = "Unauthorized"
                });
        
            // read and deserialize body
            // body contains (firstName, lastName, PhoneNumber etc).
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedProfile = JsonConvert.DeserializeObject<UserEntity>(body);

            if (updatedProfile == null)
                return new OkObjectResult(new UserResult {
                    Succeeded = false,
                    StatusCode = 400,
                    Error = "Invalid request body."
                });
        
            // set userId from token, ensure profile is updated for the correct user
            updatedProfile.UserId = userId;

            var success = await _profileService.UpdateProfileAsync(updatedProfile);

            if (!success)
                return new OkObjectResult(new UserResult {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = "Profile not found or could not be updated."
                });

            _logger.LogInformation("Updated profile for UserId: {UserId}", userId);
            return new OkObjectResult(new UserResult {
                Succeeded = true,
                StatusCode = 200,
                Message = "Profile updated successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in UpdateProfile.");
            return new OkObjectResult(new UserResult {
                Succeeded = false,
                StatusCode = 500,
                Error = "Internal server error while updating profile"
            });
        }
    }
}
