import { Injectable } from '@angular/core';
import { FolderApiService } from '@api/folder.api-.service';
import { Observable, map } from 'rxjs';
import { FolderGetAllQuery } from './models/queries/folderGetAllQuery';
import { FolderGetAll } from './models/folderGetAll';


@Injectable({
  providedIn: 'root',
})
export class FolderService {
  constructor(private folderApiService: FolderApiService) {}

  public getAll(query: FolderGetAllQuery): Observable<FolderGetAll> {
    return this.folderApiService.getAll({
      pagination: query.pagination,
      filter: query.filter,
      sort: query.sort,
      folderPath: query.folderPath,
    });
  }

  public createFolder(folderName: string, folderPath: string): Observable<boolean> {
    return this.folderApiService.createFolder({
      folderName: folderName,
      folderPath: folderPath,
    });
  }

  public deleteFolder(folderPath: string): Observable<boolean> {
    return this.folderApiService.deleteFolder(folderPath);
  }

  public dowloadZip(folderPath: string): Observable<Blob> {
    return this.folderApiService.downloadFolderAsZip(folderPath);
  }

}
