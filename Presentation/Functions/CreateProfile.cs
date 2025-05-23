using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Services;

namespace Presentation.Functions;

public class CreateProfile(ILogger<CreateProfile> logger, IProfileService profileService)
{
    private readonly ILogger<CreateProfile> _logger = logger;
    public readonly IProfileService _profileService = profileService;

    [Function("CreateProfile")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        
        
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
        
    }

}