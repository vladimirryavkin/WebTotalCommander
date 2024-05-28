using WebTotalComander.Service.ViewModels;
using FileInfo = WebTotalComander.Service.ViewModels.FileInfo;

namespace WebTotalComander.Service.Services;

public interface IFilterService
{
    public List<FileInfo> FilterFolder(FilterViewModel filter, List<FileInfo> folderList);
}
