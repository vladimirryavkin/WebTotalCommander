using Azure.Storage.Blobs;
using WebTotalComander.Core.Errors;

namespace WebTotalComander.Repository.Services;

public class AzureFileRepository : IFileRepository
{
    private static string azureContainer;
    private readonly BlobServiceClient _blobServiceClient;
    public AzureFileRepository(AzureSettings azureSettings)
    {
        azureContainer = azureSettings.AzureContainer;
        _blobServiceClient = new BlobServiceClient(azureSettings.AzureConnection);
    }
    public async Task<bool> SaveFileAsync(Stream stream, string fileName, string path)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(azureContainer);

        var blobName = ($"{path}/{fileName}").Replace(Path.DirectorySeparatorChar, '/');

        var blobClient = blobContainerClient.GetBlobClient(blobName);

        if (await blobClient.ExistsAsync())
            throw new FileAlreadyExistException("File already exists in Azure Blob Storage");

        await blobClient.UploadAsync(stream, true);

        return true;
    }
    public async Task<bool> ChangeFileAsync(Stream stream, string fileName, string path)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(azureContainer);

        var blobClient = containerClient.GetBlobClient(Path.Combine(path, fileName));

        var resBool = await blobClient.ExistsAsync();
        if (!resBool)
            throw new FileNotFoundException("No file to change");

        await blobClient.DeleteIfExistsAsync();
        await blobClient.UploadAsync(stream, true);

        return true;
    }

    public async Task<bool> DeleteFileAsync(string fileName, string path)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(azureContainer);

        var blobClient = containerClient.GetBlobClient(Path.Combine(path, fileName));

        if (!await blobClient.ExistsAsync())
            throw new FileNotFoundException("File was not found");

        await blobClient.DeleteAsync();
        return true;
    }

    public async Task<MemoryStream> DownloadFileAsync(string filePath)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(azureContainer);

        var blobClient = containerClient.GetBlobClient(filePath);

        if (!await blobClient.ExistsAsync())
            throw new FileNotFoundException("File was not found to download");


        var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);

        memoryStream.Position = 0;

        return memoryStream;
    }
}
