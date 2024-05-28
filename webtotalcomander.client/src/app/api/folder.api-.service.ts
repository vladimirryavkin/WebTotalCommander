import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, filter, map } from 'rxjs';
import { FolderModel } from './models/folder.model';
import { FolderGetAllQueryModel } from './models/queries/folderGetAllQuery.model';
import { FolderGetAllModel } from './models/folderGetAll.model';
import { SubFilter } from '@@services/models/filter-helper/sub-filter';

@Injectable({
  providedIn: 'root',
})
export class FolderApiService {
  constructor(private http: HttpClient) {}

  public res: any;
 
  public getAll(query: FolderGetAllQueryModel): Observable<FolderGetAllModel> {
    let url = 'folder/getAll?';

    if (query.pagination) {
      url = `${url}Pagination.Skip=${query.pagination.skip}&Pagination.Take=${query.pagination.take}`;
    }
    if (query.filter) {
      url = this.addFiltersToQuery(url, query.filter);
    }
    if (query.sort) {
      url = `${url}&Sort.SortField=${query.sort.sortField}&Sort.SortDirection=${query.sort.sortDirection}`;
    }
    const encodedPath = encodeURIComponent(query.folderPath);
    url = `${url}&FolderPath=${encodedPath}`;
    return this.http.get<FolderGetAllModel>(url);
  }

  public createFolder(model: FolderModel): Observable<boolean> {
    this.res = this.http.post('folder/create', model, {
      headers: { 'Content-Type': 'application/json' },
    });
    return this.res;
  }

  public deleteFolder(folderPath: string): Observable<boolean> {
    const encodedPath = encodeURIComponent(folderPath);
    return this.http.delete<boolean>(`folder/delete?folderPath=${encodedPath}`);
  }

  public downloadFolderAsZip(folderPath: string): Observable<Blob> {
    const encodedPath = encodeURIComponent(folderPath);
    return this.http.get<Blob>(`folder/downloadZip?folderPath=${encodedPath}`,{ observe: 'response', responseType: 'blob' as 'json' })
    .pipe(
      filter(response => !!response), 
      map(response => {
        if (!response!.body) {
          throw new Error('Response body is null or undefined');
        }
        return response!.body;
      })
    );
  }

  private addFiltersToQuery(url: string, params: {
    'Filter.Logic': string;
    'Filter.Filters': Array<SubFilter>;
}): string {
    let result = url;
    if (params['Filter.Filters'].length > 0) {
        result += `&Filter.Logic=${params['Filter.Logic']}`;
        for (let i = 0; i < params['Filter.Filters'].length; i++) {
            const subFilter = params['Filter.Filters'][i];

            if (subFilter.filters && subFilter.filters.length > 0)
                for (let j = 0; j < subFilter.filters.length; j++) {
                    const filterDefinition = subFilter.filters[j];
                    result += `&Filter.Filters[${i}].Filters[${j}].Field=${filterDefinition.field}`;
                    result += `&Filter.Filters[${i}].Filters[${j}].Operator=${filterDefinition.operator}`;
                    result += `&Filter.Filters[${i}].Filters[${j}].Value=${filterDefinition.value}`;
                }
            result += `&Filter.Filters[${i}].Logic=${subFilter.logic}`;
        }
    }
    return result;
}
}
