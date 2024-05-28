namespace WebTotalComander.Repository.Services;

public class FolderSettings
{
    public string UploadFolderPath { get; }
    public FolderSettings(string uploadFolderPath)
    {
        UploadFolderPath = uploadFolderPath;
    }
}
