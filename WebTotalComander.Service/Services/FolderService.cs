using WebTotalComander.Repository.Services;
using WebTotalComander.Service.ViewModels;
using FileInfo = WebTotalComander.Service.ViewModels.FileInfo;

namespace WebTotalComander.Service.Services;

public class FolderService : IFolderService
{
    private readonly IFolderRepository _folderRepository;
    private readonly IFilterService _filterService;
    private readonly string uploadPath;

    public FolderService(IFolderRepository folderRepository, IFilterService filterService, FolderSettings configuration)
    {
        _folderRepository = folderRepository;
        _filterService = filterService;
        uploadPath = configuration.UploadFolderPath;
    }

    public async Task<ApiResponseGetAllViewModel> GetAllAsync(QueryParametersViewModel queryParameters)
    {
        var fileFolderInfos = await _folderRepository.GetAllAsync(queryParameters.FolderPath);

        var apiResponse = new ApiResponseGetAllViewModel()
        {
            FilesInfo = new List<FileInfo>(),
            TotalCount = 0,
            FolderPath = queryParameters.FolderPath
        };

        if (fileFolderInfos.Count == 0 || fileFolderInfos == null)
            return apiResponse;

        if (queryParameters.Pagination == null)
        {
            queryParameters.Pagination.Skip = 0;
            queryParameters.Pagination.Take = 10;
        }

        if (queryParameters.Pagination.Skip < 0) queryParameters.Pagination.Skip = 0;
        if (queryParameters.Pagination.Take > 100) queryParameters.Pagination.Take = 100;


        for (var i = 0; i < fileFolderInfos.Count; i++)
        {
            apiResponse.FilesInfo.Add(new FileInfo
            {
                FileExtension = fileFolderInfos[i].FileExtension,
                FileName = fileFolderInfos[i].FileName,
                Size = fileFolderInfos[i].Size,
                FileCreationTime = fileFolderInfos[i].FileCreationTime
            });
        }

        if (queryParameters.Filter != null)
        {
            apiResponse.FilesInfo = _filterService.FilterFolder(queryParameters.Filter, apiResponse.FilesInfo);
        }

        if (queryParameters.Sort != null)
        {
            if (queryParameters.Sort.SortDirection == "desc")
            {
                apiResponse.FilesInfo = await SortDescAsync(queryParameters.Sort, apiResponse.FilesInfo);
            }
            else if (queryParameters.Sort.SortDirection == "asc")
            {
                apiResponse.FilesInfo = await SortAscAsync(queryParameters.Sort, apiResponse.FilesInfo);
            }
        }

        apiResponse.TotalCount = apiResponse.FilesInfo.Count();
        apiResponse.FilesInfo = apiResponse.FilesInfo
            .Skip(queryParameters.Pagination.Skip)
            .Take(queryParameters.Pagination.Take).ToList();

        return apiResponse;
    }

    public async Task<bool> CreateFolderAsync(FolderViewModel folderViewModel)
    {
        return await _folderRepository.CreateFolderAsync(folderViewModel.FolderName, folderViewModel.FolderPath);
    }

    public async Task<bool> DeleteFolderAsync(string folderPath)
    {
        return await _folderRepository.DeleteFolderAsync(folderPath);
    }

    public async Task<byte[]> DownloadZipAsync(string folderPath, string zipFileName)
    {
        return await _folderRepository.DownloadZipAsync(folderPath, zipFileName);
    }

    private async Task<List<FileInfo>> SortAscAsync(SortViewModel sortViewModel, List<FileInfo> fileInfos)
    {
        return sortViewModel.SortField switch
        {
            "fileName" => fileInfos.OrderBy(x => x.FileName).ToList(),
            "fileExtension" => fileInfos.OrderBy(x => x.FileExtension).ToList(),
            "fileCreationTime" => fileInfos.OrderBy(x => x.FileCreationTime).ToList(),
            "size" => fileInfos.OrderBy(x => x.Size).ToList(),
            _ => throw new ArgumentException($"Invalid sort field: {sortViewModel.SortField}")
        };
    }

    private async Task<List<FileInfo>> SortDescAsync(SortViewModel sortViewModel, List<FileInfo> fileInfos)
    {
        return sortViewModel.SortField switch
        {
            "fileName" => fileInfos.OrderByDescending(x => x.FileName).ToList(),
            "fileExtension" => fileInfos.OrderByDescending(x => x.FileExtension).ToList(),
            "fileCreationTime" => fileInfos.OrderByDescending(x => x.FileCreationTime).ToList(),
            "size" => fileInfos.OrderByDescending(x => x.Size).ToList(),
            _ => throw new ArgumentException($"Invalid sort field: {sortViewModel.SortField}")
        };
    }
}
