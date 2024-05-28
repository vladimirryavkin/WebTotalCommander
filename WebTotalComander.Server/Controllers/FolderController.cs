using Microsoft.AspNetCore.Mvc;
using WebTotalComander.Core.Errors;
using WebTotalComander.Service.Services;
using WebTotalComander.Service.ViewModels;

namespace WebTotalComander.Server.Controllers;

[Route("api/folder")]
[ApiController]
public class FolderController : ControllerBase
{
    private readonly IFolderService _folderService;

    public FolderController(IFolderService folderService)
    {
        _folderService = folderService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<bool>> CreateFolder(FolderViewModel folderViewModel)
    {
        if (folderViewModel.FolderName == null || folderViewModel.FolderName == string.Empty)
            throw new RequestParametrsInvalidExeption("Invalid parametrs");

        await _folderService.CreateFolderAsync(folderViewModel);

        return Ok(true);
    }

    [HttpDelete("delete")]
    public async Task<ActionResult<bool>> DeleteFolder(string folderPath)
    {
        if (folderPath == null || folderPath.StartsWith(" "))
            throw new RequestParametrsInvalidExeption("Invalid parametrs");

        var res = await _folderService.DeleteFolderAsync(folderPath);

        return res;
    }

    [HttpGet("getAll")]
    public async Task<ApiResponseGetAllViewModel> GetAll([FromQuery] QueryParametersViewModel queryParameters)
    {
        if (queryParameters.FolderPath == null) queryParameters.FolderPath = "myFiles";
        if (queryParameters == null)
            throw new RequestParametrsInvalidExeption("request is null");

        var resultForRequest = await _folderService.GetAllAsync(queryParameters);

        return resultForRequest;
    }

    [HttpGet("downloadZip")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> DownloadFolder(string folderPath)
    {
        var zipFileName = "";
        var index = folderPath.LastIndexOf('/');

        if (index >= 0)
        {
            zipFileName = folderPath.Substring(index + 1);
        }
        else
            zipFileName = folderPath;

        var zipFileBytes = await _folderService.DownloadZipAsync(folderPath, zipFileName);
        var res = File(zipFileBytes, "application/zip", zipFileName);
        return res;
    }
}
