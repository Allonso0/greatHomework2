using FileStoringService.Data;
using FileStoringService.Models;
using System.Security.Cryptography;

namespace FileStoringService.Services;

public class FileStorageService
{
    private readonly FileDbContext _context;
    private readonly string _storagePath;

    public FileStorageService(FileDbContext context)
    {
        _context = context;
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
        if (!Directory.Exists(_storagePath))
            Directory.CreateDirectory(_storagePath);
    }

    public async Task<FileMetadata> SaveFileAsync(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var content = ms.ToArray();
        var hash = ComputeHash(content);

        var existing = _context.Files.FirstOrDefault(f => f.Hash == hash);
        if (existing != null)
            return existing;

        var id = Guid.NewGuid().ToString();
        var fileName = id + ".txt";
        var path = Path.Combine(_storagePath, fileName);

        await File.WriteAllBytesAsync(path, content);

        var metadata = new FileMetadata
        {
            Id = id,
            FileName = file.FileName,
            Hash = hash,
            Location = path,
            UploadedAt = DateTime.UtcNow
        };

        _context.Files.Add(metadata);
        await _context.SaveChangesAsync();

        return metadata;
    }

    public FileMetadata? GetByHash(string hash) =>
        _context.Files.FirstOrDefault(f => f.Hash == hash);

    public (byte[] content, string fileName)? GetFileById(string id)
    {
        var metadata = _context.Files.FirstOrDefault(f => f.Id == id);
        if (metadata == null) return null;
        if (!File.Exists(metadata.Location)) return null;

        var content = File.ReadAllBytes(metadata.Location);
        return (content, metadata.FileName);
    }

    private string ComputeHash(byte[] data)
    {
        using var sha256 = SHA256.Create();
        return BitConverter.ToString(sha256.ComputeHash(data)).Replace("-", "").ToLowerInvariant();
    }
}
