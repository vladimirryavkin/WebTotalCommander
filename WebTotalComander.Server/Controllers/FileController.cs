using Microsoft.AspNetCore.Mvc;
using WebTotalComander.Core.Errors;
using WebTotalComander.Service.Services;
using WebTotalComander.Service.ViewModels;

namespace WebTotalComander.Server.Controllers;

[Route("api/file")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("replace")]
    public async Task<ActionResult<bool>> ReplaceFile([FromForm] FileViewModel fileViewModel)
    {
        if (fileViewModel.File == null)
            throw new RequestParametrsInvalidExeption("Invalid parametrs");

        var stream = fileViewModel.File.OpenReadStream();
        var responeForClient = await _fileService.ReplaceFileAsync(stream, fileViewModel.File.FileName, fileViewModel.FilePath);
        return Ok(responeForClient);
    }

    [HttpPost("upload")]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<bool>> UploadFile([FromForm] FileViewModel fileViewModel)
    {
        if (fileViewModel.File == null)
            throw new RequestParametrsInvalidExeption("Invalid parametrs");

        var stream = fileViewModel.File.OpenReadStream();
        var responeForClient = await _fileService.SaveFileAsync(stream, fileViewModel.File.FileName, fileViewModel.FilePath);
        return Ok(responeForClient);
    }

    [HttpDelete("delete")]
    public async Task<ActionResult<bool>> DeleteFile(string fileName, string filePath = "")
    {
        if (fileName == null)
            throw new RequestParametrsInvalidExeption("Invalid parametrs");

        var res = await _fileService.DeleteFileAsync(fileName, filePath);
        return Ok(res);
    }

    [HttpGet("download-file")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> DownloadFile(string filePath)
    {
        var type = filePath.Substring(filePath.LastIndexOf('.') + 1);
        var memoryStream = await _fileService.DownloadFileAsync(filePath);
        var res = File(memoryStream, $"application/{type}");
        return res;
    }
}
