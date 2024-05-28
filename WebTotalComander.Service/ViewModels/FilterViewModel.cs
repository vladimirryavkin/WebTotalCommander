using System.ComponentModel.DataAnnotations;
using WebTotalCommander.Service.ViewModels.Common;

namespace WebTotalComander.Service.ViewModels;

public class FilterViewModel
{
    private List<SubFilterViewModel> _subFilters;

    [Required]
    public string Logic { get; set; }
    public List<SubFilterViewModel> Filters
    {
        get => _subFilters ??= new List<SubFilterViewModel>();
        set => _subFilters = value;
    }
}
