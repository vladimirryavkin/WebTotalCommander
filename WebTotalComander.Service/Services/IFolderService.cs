using WebTotalComander.Service.ViewModels;

namespace WebTotalComander.Service.Services;

public interface IFolderService
{
    public Task<bool> CreateFolderAsync(FolderViewModel folderViewModel);
    public  Task<ApiResponseGetAllViewModel> GetAllAsync(QueryParametersViewModel queryParameters);
    public Task<bool> DeleteFolderAsync(string folderPath);
    public Task<byte[]> DownloadZipAsync(string folderPath, string zipFileName);
}
