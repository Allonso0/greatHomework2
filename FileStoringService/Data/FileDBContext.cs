using Microsoft.EntityFrameworkCore;
using FileStoringService.Models;

namespace FileStoringService.Data;

public class FileDbContext : DbContext
{
    public FileDbContext(DbContextOptions<FileDbContext> options)
        : base(options)
    {
    }

    public DbSet<FileMetadata> Files => Set<FileMetadata>();
}
