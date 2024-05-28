using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO.Compression;
using WebTotalComander.Repository.Models;

namespace WebTotalComander.Repository.Services;

public class AzureFolderRepository : IFolderRepository
{
    private static string azureContainer;
    private readonly BlobServiceClient _blobServiceClient;
    private const int BytesInKilobyte = 1024;
    public AzureFolderRepository(AzureSettings azureSettings)
    {
        azureContainer = azureSettings.AzureContainer;
        _blobServiceClient = new BlobServiceClient(azureSettings.AzureConnection);
    }

    public async Task<List<FileInfoModel>> GetAllAsync(string folderPath)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(azureContainer);

        var blobs = containerClient.GetBlobs(prefix: folderPath).ToList();

        var check = false;
        for (var i = 0; i < blobs.Count; i++)
        {
            check = (!blobs[i].Name.StartsWith(folderPath + "/") || blobs[i].Name.Split('/').Length != folderPath.Split('/').Length + 1);
            if (check)
            {
                blobs.Remove(blobs[i]);
                --i;
            }
        }
        var folderFileModel = await FillFileInfoModelAsync(blobs);
        return folderFileModel;
    }

    private async Task<List<FileInfoModel>> FillFileInfoModelAsync(List<BlobItem> blobs)
    {
        var filesInfoModel = new List<FileInfoModel>();

        foreach (var blob in blobs)
        {
            var name = blob.Name.Substring(blob.Name.LastIndexOf("/") + 1);
            var size = blob.Properties.ContentLength ?? 0;
            var time = blob.Properties.LastModified ?? DateTimeOffset.MinValue;
            var extension = (name.Split('.').Length > 1) ? name.Substring(name.LastIndexOf(".")) : "folder";

            filesInfoModel.Add(new FileInfoModel()
            {
                FileName = name,
                Size = size / BytesInKilobyte,
                FileCreationTime = time.DateTime,
                FileExtension = extension
            });
        }

        return filesInfoModel;
    }

    public async Task<bool> CreateFolderAsync(string folderName, string folderPath)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(azureContainer);

        await containerClient.CreateIfNotExistsAsync();

        var fullFolderPath = Path.Combine(folderPath, folderName);

        var blobClient = containerClient.GetBlobClient(fullFolderPath);

        if (await blobClient.ExistsAsync())
        {
            throw new InvalidOperationException("Folder already exists");
        }

        await blobClient.UploadAsync(new System.IO.MemoryStream(Array.Empty<byte>()), true);

        return true;
    }

    public async Task<bool> DeleteFolderAsync(string folderPath)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(azureContainer);

        var blobs = containerClient.GetBlobsAsync(prefix: folderPath);

        await foreach (var blobItem in blobs)
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
        return true;
    }

    public async Task<byte[]> DownloadZipAsync(string folderPath, string zipFileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(azureContainer);

        using (var zipStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                var blobs = containerClient.GetBlobsAsync(prefix: folderPath);

                await foreach (var blobItem in blobs)
                {
                    if (await IsFolder(blobItem.Name)) continue;

                    var blobClient = containerClient.GetBlobClient(blobItem.Name);
                    BlobDownloadInfo download = await blobClient.DownloadAsync();

                    var relativePath = blobItem.Name.Substring(folderPath.Length).Trim('/');

                    var entry = archive.CreateEntry(relativePath);

                    using (var entryStream = entry.Open())
                    {
                        await download.Content.CopyToAsync(entryStream);
                    }
                }
            }

            return zipStream.ToArray();
        }
    }

    private async Task<bool> IsFolder(string blobPath)
    {
        return blobPath.LastIndexOf(".") == -1;
    }
}