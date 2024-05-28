import { FilterQuery } from './filterQuery';
import { PaginationQuery } from './paginationQuery';
import { SortQuery } from './sortQuery';

export class FolderGetAllQuery {
  public pagination: PaginationQuery = new PaginationQuery();
  public filter: FilterQuery = new FilterQuery();
  public sort: SortQuery = new SortQuery();
  public folderPath: string = 'my files';
}
