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

public class CreateProfile(ILogger<CreateProfile> logger, IProfileService profileService)
{
    private readonly ILogger<CreateProfile> _logger = logger;
    private readonly IProfileService _profileService = profileService;

    [Function("CreateProfile")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "profile")] HttpRequestData req)
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

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<UserEntity>(body);

            if (user == null)
                return new OkObjectResult(new UserResult
                {
                    Succeeded = false, 
                    StatusCode = 400, 
                    Error = "Invalid request"
                });

            user.UserId = userId;

            var result = await _profileService.CreateProfileAsync(user);
            if (result == null)
                return new OkObjectResult(new UserResult
                {
                    Succeeded = false, 
                    StatusCode = 409, 
                    Error = "Profile already exists or could not be created."
                });

            _logger.LogInformation("Created profile for UserId: {UserId}", userId);
            return new OkObjectResult(new UserResult<UserEntity>
            {
                Succeeded = true, 
                StatusCode = 200, 
                Result = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in CreateProfile.");
            return new OkObjectResult(new UserResult
            {
                Succeeded = false, 
                StatusCode = 500, 
                Error = "Internal server error while creating profile"
            });
        }
    }
}