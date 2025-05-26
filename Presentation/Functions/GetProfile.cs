using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Presentation.Services;

namespace Presentation.Functions;

public class GetProfile(ILogger<GetProfile> logger, IProfileService profileService)
{
    private readonly ILogger<GetProfile> _logger = logger;
    private readonly IProfileService _profileService = profileService;

    [Function("GetProfile")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "profile")] HttpRequestData req)
    {
        // Extrahera JWT och userId från claims
        var principal = req.FunctionContext.Features.Get<JwtPrincipalFeature>()?.Principal;
        var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return new UnauthorizedResult();

        var profile = await _profileService.GetProfileAsync(userId);
        if (profile == null)
            return new NotFoundResult();
            
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(profile);
    }
}