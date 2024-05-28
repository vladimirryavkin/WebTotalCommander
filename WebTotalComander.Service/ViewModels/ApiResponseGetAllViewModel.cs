namespace WebTotalComander.Service.ViewModels;

public class ApiResponseGetAllViewModel
{
    public int TotalCount { get; set; }
    public string FolderPath { get; set; }
    public List<FileInfo> FilesInfo { get; set; }
}
