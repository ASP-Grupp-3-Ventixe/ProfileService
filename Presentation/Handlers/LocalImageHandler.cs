using Microsoft.AspNetCore.Http;

namespace Presentation.Handlers;

public class LocalImageHandler(string imagePath) : IImageHandler
{
    private readonly string _imagePath = imagePath;

    public async Task<string?> SaveImageAsync(IFormFile? file, string directory)
    {
        if (file == null || file.Length == 0)
            return null!;

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";

        // Create the full directory path
        var directoryPath = Path.Combine(_imagePath, directory);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        // relative path for web access
        return $"{directory}/{fileName}";
    }
}