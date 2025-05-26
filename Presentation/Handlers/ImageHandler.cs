using Microsoft.AspNetCore.Http;

namespace Presentation.Handlers;

public interface IImageHandler
{
    Task <string?>  SaveImageAsync(IFormFile? file, string directory);
}