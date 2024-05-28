using WebTotalComander.Core.Errors;

namespace WebTotalComander.Repository.Services;

public class FileRepository : IFileRepository
{

    private static string uploadFolderPath;

    public FileRepository(FolderSettings configuration)
    {
        uploadFolderPath = configuration.UploadFolderPath;
    }

    public async Task<bool> SaveFileAsync(Stream stream, string fileName, string path)
    {
        var filePath = Path.Combine(uploadFolderPath, path, fileName);

        if (File.Exists(filePath))
            throw new FileAlreadyExistException("File already exist");

        using (var newStream = new FileStream(filePath, FileMode.Create))
        {
            await stream.CopyToAsync(newStream);
        }

        return true;
    }
    public async Task<bool> ChangeFileAsync(Stream stream, string fileName, string path)
    {
        var filePath = Path.Combine(uploadFolderPath, path, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        else
        {
            throw new FileNotFoundException("No file to change");
        }

        using (var newStream = new FileStream(filePath, FileMode.Create))
        {
            await stream.CopyToAsync(newStream);
        }

        return true;
    }

    public async Task<bool> DeleteFileAsync(string fileName, string path)
    {
        var filePath = Path.Combine(uploadFolderPath, path, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }
        else
        {
            throw new FileNotFoundException("File was not found");
        }
    }

    public async Task<MemoryStream> DownloadFileAsync(string filePath)
    {
        var fullPath = Path.Combine(uploadFolderPath, filePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File was not found to download");
        }

        var memoryStream = new MemoryStream();
        using (var stream = new FileStream(uploadFolderPath + filePath, FileMode.Open))
        {
            await stream.CopyToAsync(memoryStream);
        }
        memoryStream.Position = 0;
        return memoryStream;
    }
}

