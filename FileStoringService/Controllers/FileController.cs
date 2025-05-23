using FileStoringService.Services;
using FileStoringService.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileStoringService.Controllers;

[ApiController]
[Route("api/files")]
public class FileController : ControllerBase
{
    private readonly FileStorageService _service;

    public FileController(FileStorageService service)
    {
        _service = service;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] FileUploadRequest request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("Файл пуст или отсутствует");

        var result = await _service.SaveFileAsync(request.File);
        return Ok(new { id = result.Id });
    }

    [HttpGet("{id}")]
    public IActionResult GetFile(string id)
    {
        var file = _service.GetFileById(id);
        if (file == null) return NotFound();

        return File(file.Value.content, "text/plain", file.Value.fileName);
    }

    [HttpGet("hash/{hash}")]
    public IActionResult GetByHash(string hash)
    {
        var result = _service.GetByHash(hash);
        return result == null ? NotFound() : Ok(result);
    }
}
