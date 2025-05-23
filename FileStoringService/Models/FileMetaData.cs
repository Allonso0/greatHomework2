using System.ComponentModel.DataAnnotations;

namespace FileStoringService.Models;

public class FileMetadata
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string FileName { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
