using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Functions;

public class DeleteProfile
{
    private readonly ILogger<DeleteProfile> _logger;

    public DeleteProfile(ILogger<DeleteProfile> logger)
    {
        _logger = logger;
    }

    [Function("DeleteProfile")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
        
    }

}