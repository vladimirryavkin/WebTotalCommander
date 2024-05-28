using WebTotalComander.Service.ViewModels;
using FileInfo = WebTotalComander.Service.ViewModels.FileInfo;


namespace WebTotalComander.Service.Services;

public class FilterService : IFilterService
{
    public List<FileInfo> FilterFolder(FilterViewModel filter, List<FileInfo> folderList)
    {
        var folderFilterColumns = new List<List<FileInfo>>();
        for (var i = 0; i < filter.Filters.Count; i++)
        {
            var columnName = filter.Filters[i].Filters[0].Field;
            if (!String.IsNullOrEmpty(columnName))
            {
                switch (columnName)
                {
                    case "fileName":
                        var folderFilterListByName = new List<List<FileInfo>>();
                        foreach (var item in filter.Filters[i].Filters)
                        {
                            switch (item.Operator)
                            {
                                case "contains":
                                    folderFilterListByName.Add(folderList.Where(x => x.FileName.Contains(item.Value)).ToList());
                                    break;
                                case "doesnotcontain":
                                    folderFilterListByName.Add(folderList.Where(x => !x.FileName.Contains(item.Value)).ToList());
                                    break;
                                case "eq":
                                    folderFilterListByName.Add(folderList.Where(x => x.FileName == item.Value).ToList());
                                    break;
                                case "neq":
                                    folderFilterListByName.Add(folderList.Where(x => x.FileName != item.Value).ToList());
                                    break;
                                case "startswith":
                                    folderFilterListByName.Add(folderList.Where(x => x.FileName.StartsWith(item.Value)).ToList());
                                    break;
                                case "endswith":
                                    folderFilterListByName.Add(folderList.Where(x => x.FileName.EndsWith(item.Value)).ToList());
                                    break;
                            }
                        }
                        if (folderFilterListByName.Count > 1)
                        {
                            if (filter.Filters[i].Logic == "and")
                            {
                                if (folderFilterListByName[0].Intersect(folderFilterListByName[1]).ToList().Count > 0)
                                {
                                    folderFilterColumns.Add(folderFilterListByName[0].Intersect(folderFilterListByName[1]).ToList());
                                }
                                else
                                {
                                    folderFilterColumns.Add(folderFilterListByName[0]);
                                }

                            }
                            else
                            {
                                folderFilterColumns.Add(folderFilterListByName[0].Concat(folderFilterListByName[1]).Distinct().ToList());
                            }
                        }
                        else
                        {
                            folderFilterColumns.Add(folderFilterListByName[0]);
                        }


                        break;

                    case "fileExtension":

                        var folderFilterListByExtension = new List<List<FileInfo>>();
                        foreach (var item in filter.Filters[i].Filters)
                        {
                            switch (item.Operator)
                            {
                                case "contains":
                                    folderFilterListByExtension.Add(folderList.Where(x => x.FileExtension.Contains(item.Value)).ToList());
                                    break;
                                case "doesnotcontain":
                                    folderFilterListByExtension.Add(folderList.Where(x => !x.FileExtension.Contains(item.Value)).ToList());
                                    break;
                                case "eq":
                                    folderFilterListByExtension.Add(folderList.Where(x => x.FileExtension == item.Value).ToList());
                                    break;
                                case "neq":
                                    folderFilterListByExtension.Add(folderList.Where(x => x.FileExtension != item.Value).ToList());
                                    break;
                                case "startswith":
                                    folderFilterListByExtension.Add(folderList.Where(x => x.FileExtension.StartsWith(item.Value)).ToList());
                                    break;
                                case "endswith":
                                    folderFilterListByExtension.Add(folderList.Where(x => x.FileExtension.EndsWith(item.Value)).ToList());
                                    break;
                            }
                        }
                        if (folderFilterListByExtension.Count > 1)
                        {
                            if (filter.Filters[i].Logic == "and")
                            {
                                if (folderFilterListByExtension[0].Intersect(folderFilterListByExtension[1]).ToList().Count > 0)
                                {
                                    folderFilterColumns.Add(folderFilterListByExtension[0].Intersect(folderFilterListByExtension[1]).ToList());
                                }
                                else
                                {
                                    folderFilterColumns.Add(folderFilterListByExtension[0]);
                                }
                            }
                            else
                            {
                                folderFilterColumns.Add(folderFilterListByExtension[0].Concat(folderFilterListByExtension[1]).Distinct().ToList());
                            }
                        }
                        else
                        {
                            folderFilterColumns.Add(folderFilterListByExtension[0]);
                        }

                        break;

                    case "fileCreationTime":
                        var folderFilterListByCreatedDate = new List<List<FileInfo>>();
                        foreach (var item in filter.Filters[i].Filters)
                        {
                            switch (item.Operator)
                            {
                                case "eq":
                                    folderFilterListByCreatedDate.Add(folderList.Where(x => x.FileCreationTime.Equals(item.Value)).ToList());
                                    break;
                                case "neq":
                                    folderFilterListByCreatedDate.Add(folderList.Where(x => !x.FileCreationTime.Equals(item.Value)).ToList());
                                    break;
                                case "gte":
                                    folderFilterListByCreatedDate.Add(folderList.Where(x => x.FileCreationTime >= DateTime.Parse(item.Value)).ToList());
                                    break;
                                case "gt":
                                    folderFilterListByCreatedDate.Add(folderList.Where(x => x.FileCreationTime > DateTime.Parse(item.Value)).ToList());
                                    break;
                                case "lte":
                                    folderFilterListByCreatedDate.Add(folderList.Where(x => x.FileCreationTime <= DateTime.Parse(item.Value)).ToList());
                                    break;
                                case "lt":
                                    folderFilterListByCreatedDate.Add(folderList.Where(x => x.FileCreationTime < DateTime.Parse(item.Value)).ToList());
                                    break;
                            }
                        }
                        if (folderFilterListByCreatedDate.Count > 1)
                        {
                            if (filter.Filters[i].Logic == "and")
                            {
                                if (folderFilterListByCreatedDate[0].Intersect(folderFilterListByCreatedDate[1]).ToList().Count > 0)
                                {
                                    folderFilterColumns.Add(folderFilterListByCreatedDate[0].Intersect(folderFilterListByCreatedDate[1]).ToList());
                                }
                                else
                                {
                                    folderFilterColumns.Add(folderFilterListByCreatedDate[0]);
                                }
                            }
                            else
                            {
                                folderFilterColumns.Add(folderFilterListByCreatedDate[0].Concat(folderFilterListByCreatedDate[1]).Distinct().ToList());
                            }
                        }
                        else
                        {
                            folderFilterColumns.Add(folderFilterListByCreatedDate[0]);
                        }

                        break;
                }
            }
        }
        if (folderFilterColumns.Count <= 0)
            return folderList;
        var folderFilterResult = new List<FileInfo>();
        folderFilterResult = folderFilterColumns[0].Slice(0, folderFilterColumns[0].Count);
        if (folderFilterColumns.Count > 1)
        {
            for (var i = 1; i < folderFilterColumns.Count; i++)
            {
                folderFilterResult = folderFilterResult.Intersect(folderFilterColumns[i]).ToList();
            }
        }

        return folderFilterResult;
    }
}
