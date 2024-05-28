import { Injectable } from '@angular/core';
import { FileApiService } from '@api/file.api-.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FileService {

  constructor(private fileApiService : FileApiService) { }

  public uploadFiles(files: File[], filePath: string): Observable<boolean>{
   
    return this.fileApiService.uploadFiles(
      {
        files : files,
        filePath: filePath
      }
    )
  }

  public replaceFile(file: File, filePath: string): Observable<boolean>{
    return this.fileApiService.replaceFile(
      {
        file : file,
        filePath: filePath
      }
    )
  }

  public deleteFile(filePath: string, fileName : string): Observable<boolean> {
    return this.fileApiService.deleteFile(filePath, fileName);
  }

  public downloadFile(filePath: string): Observable<Blob> {
    return this.fileApiService.downloadFile(filePath);
  }

  public downloadFileForEdit(filePath: string): Observable<Blob> {
    return this.fileApiService.downloadFile(filePath);
  }
}
