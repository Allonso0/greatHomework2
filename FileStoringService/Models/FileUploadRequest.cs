using Microsoft.AspNetCore.Http;

namespace FileStoringService.Models;

public class FileUploadRequest
{
    public IFormFile File { get; set; } = null!;
}
