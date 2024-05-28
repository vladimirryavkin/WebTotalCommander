using Microsoft.AspNetCore.Http;

namespace WebTotalComander.Service.ViewModels;

public class UploadFilesViewModel
{
    public IFormFile[] Files { get; set; }
    public string FilePath { get; set; }
}
