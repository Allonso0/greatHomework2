using FileAnalysisService.Data;
using FileAnalysisService.Models;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FileAnalysisService.Services;

public class AnalysisService
{
    private readonly AnalysisDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public AnalysisService(AnalysisDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<AnalysisResult> AnalyzeFileAsync(string fileId)
    {
        var existing = await _context.AnalysisResults.FindAsync(fileId);
        if (existing != null) return existing;

        var storageClient = _httpClientFactory.CreateClient("FileStorage");

        var response = await storageClient.GetAsync($"files/{fileId}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Не удалось получить файл");

        var text = await response.Content.ReadAsStringAsync();

        var rawHttpClient = new HttpClient();
        var jsonContent = JsonContent.Create(new { text });

        var wordCloudResponse = await rawHttpClient.PostAsync("https://quickchart.io/wordcloud", jsonContent);
        if (!wordCloudResponse.IsSuccessStatusCode)
        {
            var err = await wordCloudResponse.Content.ReadAsStringAsync();
            throw new Exception($"Ошибка при получении изображения из WordCloud API: {wordCloudResponse.StatusCode}, content: {err}");
        }

        var imageBytes = await wordCloudResponse.Content.ReadAsByteArrayAsync();

        var imageContent = new MultipartFormDataContent();
        var byteContent = new ByteArrayContent(imageBytes);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        imageContent.Add(byteContent, "file", $"{fileId}_wordcloud.png");

        var uploadResponse = await storageClient.PostAsync("files/upload", imageContent);
        var errorContent = await uploadResponse.Content.ReadAsStringAsync();

        if (!uploadResponse.IsSuccessStatusCode)
            throw new Exception($"Ошибка при загрузке WordCloud: {(int)uploadResponse.StatusCode} - {uploadResponse.ReasonPhrase}. Ответ: {errorContent}");

        var uploadJson = await uploadResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        if (uploadJson == null || !uploadJson.TryGetValue("id", out var wordCloudFileId))
            throw new Exception($"FileStoringService не вернул id картинки. Ответ: {errorContent}");

        var result = new AnalysisResult
        {
            FileId = fileId,
            ParagraphCount = Regex.Split(text.Trim(), @"(\r?\n\s*\r?\n)+")
                                .Count(p => !string.IsNullOrWhiteSpace(p)),
            WordCount = text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length,
            CharacterCount = text.Length,
            WordCloudImageLocation = wordCloudFileId
        };

        _context.AnalysisResults.Add(result);
        await _context.SaveChangesAsync();

        return result;
    }

    public AnalysisResult? GetByFileId(string fileId) =>
        _context.AnalysisResults.FirstOrDefault(a => a.FileId == fileId);
}
