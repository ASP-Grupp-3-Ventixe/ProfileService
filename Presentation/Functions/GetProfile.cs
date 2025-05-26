using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Functions;

public class GetProfile
{
    private readonly ILogger<GetProfile> _logger;

    public GetProfile(ILogger<GetProfile> logger)
    {
        _logger = logger;
    }

    [Function("GetProfile")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
        
    }

}