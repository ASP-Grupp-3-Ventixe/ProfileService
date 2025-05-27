using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Presentation.Helpers;
using Presentation.Services;
using Presentation.Responses;

namespace Presentation.Functions;

public class GetProfile(ILogger<GetProfile> logger, IProfileService profileService)
{
    private readonly ILogger<GetProfile> _logger = logger;
    private readonly IProfileService _profileService = profileService;

    [Function("GetProfile")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "profile")] HttpRequestData req)
    {
        try
        {
            var userId = AuthUtils.ExtractUserId(req);
            if (string.IsNullOrEmpty(userId))
                return new OkObjectResult(new UserResult
                {
                    Succeeded = false, 
                    StatusCode = 401, 
                    Error = "Unauthorized"
                });

            var profile = await _profileService.GetProfileAsync(userId);
            if (profile == null)
                return new OkObjectResult(new UserResult 
                {
                    Succeeded = false,
                    StatusCode = 404, 
                    Error = "Profile not found"
                });

            _logger.LogInformation("Profile retrieved successfully for user {UserId}", userId);
            return new OkObjectResult(new UserResult<UserEntity>
            {
                Succeeded = true, 
                StatusCode = 200, 
                Result = profile
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in GetProfile.");
            return new OkObjectResult(new UserResult
            {
                Succeeded = false, 
                StatusCode = 500, 
                Error = "Internal server error while retrieving profile"
            });
        }
    }
}