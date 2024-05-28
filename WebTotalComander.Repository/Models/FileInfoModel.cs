namespace WebTotalComander.Repository.Models;

public class FileInfoModel
{
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public long Size { get; set; }
    public DateTime FileCreationTime { get; set; }
}
