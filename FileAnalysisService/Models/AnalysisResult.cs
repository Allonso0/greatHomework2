using System.ComponentModel.DataAnnotations;

namespace FileAnalysisService.Models;

public class AnalysisResult
{
    [Key]
    public string FileId { get; set; } = "";

    public int ParagraphCount { get; set; }

    public int WordCount { get; set; }

    public int CharacterCount { get; set; }

    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

    public string? WordCloudImageLocation { get; set; }
}
