using System.Security.Claims;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using Presentation.Services;

namespace Presentation.Functions;

public class UpdateProfile(ILogger<UpdateProfile> logger, IProfileService profileService)
{
    private readonly ILogger<UpdateProfile> _logger = logger;
    public readonly IProfileService _profileService = profileService;

    [Function("UpdateProfile")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "profile")] HttpRequestData req)
    {

        // extract JWT och userId från claims
        var principal = req.FunctionContext.Features.Get<JwtPrincipalFeature>()?.Principal;
        var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return new UnauthorizedResult();
        
        // read body and deserialize profile data
        // body contains only the fields to update, (FirstNamn, Lastnamn, PhoneNumber, avatar image etc).
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var updatedProfile = JsonConvert.DeserializeObject<UserEntity>(body);

        if (updatedProfile == null)
            return new BadRequestObjectResult("Invalid request body.");
        
        // set userId from token, not from client!
        updatedProfile.UserId = userId;

        var success = await _profileService.UpdateProfileAsync(updatedProfile);

        if (!success)
            return new NotFoundObjectResult("Profile not found or could not be updated.");
        
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Profile updated successfully.");
        
    }

}