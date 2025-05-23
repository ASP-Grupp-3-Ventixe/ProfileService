using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Services;

namespace Presentation.Functions;

public class UpdateProfile(ILogger<UpdateProfile> logger, IProfileService profileService)
{
    private readonly ILogger<UpdateProfile> _logger = logger;
    public readonly IProfileService _profileService = profileService;

    [Function("UpdateProfile")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequest req)
    {
        
        
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
        
    }

}