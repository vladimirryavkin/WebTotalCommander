namespace WebTotalComander.Service.ViewModels;

public class QueryParametersViewModel
{
    public PaginationViewModel Pagination { get; set; }
    public FilterViewModel Filter { get; set; }
    public SortViewModel Sort { get; set; }
    public string FolderPath { get; set; }
}
