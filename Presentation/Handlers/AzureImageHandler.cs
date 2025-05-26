using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Presentation.Handlers;

public class AzureImageHandler : IImageHandler
{
    private readonly BlobContainerClient _containerClient;
    private readonly IConfiguration _configuration;

    public AzureImageHandler(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionstring = _configuration["AzureStorageAccount:ConnectionString"];
        var containerName = _configuration["AzureStorageAccount:ContainerName"];

        _containerClient = new BlobContainerClient(connectionstring, containerName);
        _containerClient.CreateIfNotExists();
    }
    
    public async Task<string?> SaveImageAsync(IFormFile? file, string directory)
    {
        if (file == null || file.Length == 0) return null!;

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var blobPath = string.IsNullOrWhiteSpace(directory)
            ? fileName
            : $"{directory.TrimEnd('/')}/{fileName}";

        // Content type fix for SVG
        var contentType = string.IsNullOrEmpty(file.ContentType)
            ? "application/octet-stream"
            : file.ContentType;
        
        if ((contentType == "application/octet-stream" || string.IsNullOrEmpty(contentType)) && extension.Equals(".svg", StringComparison.OrdinalIgnoreCase))
            contentType = "image/svg+xml";

        var blobClient = _containerClient.GetBlobClient(blobPath);
        var blobHttpHeader = new BlobHttpHeaders { ContentType = contentType };

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeader });

        // return the blob URL
        return blobClient.Uri.ToString();
    }
}