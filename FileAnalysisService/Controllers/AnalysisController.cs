using FileAnalysisService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileAnalysisService.Controllers;

[ApiController]
[Route("api/analysis")]
public class AnalysisController : ControllerBase
{
    private readonly AnalysisService _service;
    private readonly HttpClient _http;

    public AnalysisController(AnalysisService service, IHttpClientFactory factory)
    {
        _service = service;
        _http = factory.CreateClient("FileStorage");
    }

    [HttpPost("{fileId}")]
    public async Task<IActionResult> Analyze(string fileId)
    {
        try
        {
            var result = await _service.AnalyzeFileAsync(fileId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{fileId}")]
    public IActionResult Get(string fileId)
    {
        var result = _service.GetByFileId(fileId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("image/{id}")]
    public async Task<IActionResult> GetWordCloudImage(string id)
    {
        var response = await _http.GetAsync($"files/{id}");
        if (!response.IsSuccessStatusCode)
            return NotFound();

        var bytes = await response.Content.ReadAsByteArrayAsync();
        return File(bytes, "image/png");
    }
}
