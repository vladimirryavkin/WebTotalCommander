using System.IO.Compression;
using WebTotalComander.Repository.Models;
namespace WebTotalComander.Repository.Services;

public class FolderRepository : IFolderRepository
{
    private static string uploadFolderPath;
    private const int BytesInKilobyte = 1024;

    public FolderRepository(FolderSettings configuration)
    {
        uploadFolderPath = configuration.UploadFolderPath;
    }

    public async Task<bool> CreateFolderAsync(string folderName, string folderPath)
    {
        var path = Path.Combine(uploadFolderPath, folderPath, folderName);
        if (Directory.Exists(path))
            throw new FileNotFoundException("Folder already exist");

        Directory.CreateDirectory(path);
        return true;
    }

    public async Task<bool> DeleteFolderAsync(string folderPath)
    {
        var path = Path.Combine(uploadFolderPath, folderPath);
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException("Directory was not found");

        Directory.Delete(path, recursive: true);
        return true;
    }

    public async Task<List<FileInfoModel>> GetAllAsync(string folderPath)
    {
        var path = Path.Combine(uploadFolderPath, folderPath);

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException("Directory was not found");

        var paths = Directory.GetFileSystemEntries(path);


        var folderFileModel = await FillFileInfoModelAsync(paths);

        return folderFileModel;
    }

    private async Task<List<FileInfoModel>> FillFileInfoModelAsync(string[] paths)
    {
        var filesInfoModel = new List<FileInfoModel>();

        foreach (var path in paths)
        {
            var name = path.Substring(path.LastIndexOf("\\") + 1);
            var size = (File.Exists(path)) ? new FileInfo(path).Length / BytesInKilobyte
                : await CalculateDirectorySizeAsync(path) / BytesInKilobyte;
            var time = (File.Exists(path)) ? File.GetCreationTime(path)
                : Directory.GetCreationTime(path);
            var extension = (name.Split('.').Length > 1) ? name.Substring(name.LastIndexOf(".")) : "folder";

            filesInfoModel.Add(new FileInfoModel()
            {
                FileName = name,
                Size = size,
                FileCreationTime = time,
                FileExtension = extension
            });
        }
        return filesInfoModel;
    }

    public async Task<byte[]> DownloadZipAsync(string folderPath, string zipFileName)
    {
        folderPath = Path.Combine(uploadFolderPath, folderPath);

        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException("Directory was not found");

        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                await ZipFolder(folderPath, archive, "");
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream.ToArray();
        }
    }

    private async Task ZipFolder(string sourceFolder, ZipArchive archive, string entryPrefix)
    {
        foreach (var file in Directory.GetFiles(sourceFolder))
        {
            var entryName = Path.Combine(entryPrefix, Path.GetFileName(file));
            var entry = archive.CreateEntry(entryName);

            using (var entryStream = entry.Open())
            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(entryStream);
            }
        }

        foreach (var subFolder in Directory.GetDirectories(sourceFolder))
        {
            var entryName = Path.Combine(entryPrefix, Path.GetFileName(subFolder) + "/");
            archive.CreateEntry(entryName);
            await ZipFolder(subFolder, archive, entryName);
        }
    }

    private async Task<long> CalculateDirectorySizeAsync(string directoryPath)
    {
        var files = Directory.GetFiles(directoryPath);

        var totalSize = 0L;
        foreach (var file in files)
        {
            totalSize += new FileInfo(file).Length;
        }
        var subdirectories = Directory.GetDirectories(directoryPath);
        foreach (var subdirectory in subdirectories)
        {
            totalSize += await CalculateDirectorySizeAsync(subdirectory);
        }

        return totalSize;
    }
}
