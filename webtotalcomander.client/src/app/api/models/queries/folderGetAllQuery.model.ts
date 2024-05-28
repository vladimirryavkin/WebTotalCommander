import { FilterQueryModel } from "./filterQuery.model";
import { PaginationQueryModel } from "./paginationQuery.model";
import { SortQueryModel } from "./sortQuery.model";

export interface FolderGetAllQueryModel {
  pagination: PaginationQueryModel;
  filter: FilterQueryModel;
  sort: SortQueryModel;
  folderPath: string;
}
