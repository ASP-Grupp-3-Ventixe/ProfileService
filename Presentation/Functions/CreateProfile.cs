using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using Presentation.Services;

namespace Presentation.Functions;

public class CreateProfile(ILogger<CreateProfile> logger, IProfileService profileService)
{
    private readonly ILogger<CreateProfile> _logger = logger;
    private readonly IProfileService _profileService = profileService;

    [Function("CreateProfile")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "profile")] HttpRequestData req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var user = JsonConvert.DeserializeObject<UserEntity>(body);

        if (user == null || string.IsNullOrEmpty(user.UserId))
            return new BadRequestObjectResult("Invalid request");

        var result = await _profileService.CreateProfileAsync(user);
        if (result == null)
            return new ConflictObjectResult("Profile already exists or could not be created.");

        
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(result);
        
    }

}