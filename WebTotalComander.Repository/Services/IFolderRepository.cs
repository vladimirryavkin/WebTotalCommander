using WebTotalComander.Repository.Models;

namespace WebTotalComander.Repository.Services;

public interface IFolderRepository
{
    public Task<bool> CreateFolderAsync(string folderName, string folderPath);
    public Task<List<FileInfoModel>> GetAllAsync(string folderPath);
    public Task<bool> DeleteFolderAsync(string folderPath);
    public Task<byte[]> DownloadZipAsync(string folderPath, string zipFileName);
}
