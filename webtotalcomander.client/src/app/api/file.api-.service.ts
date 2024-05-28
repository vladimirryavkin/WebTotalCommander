import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FileModel } from './models/file.model';
import { FilesModel } from './models/files.model';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class FileApiService {
  constructor(private http: HttpClient, private toastr: ToastrService) {}

  public f: FileModel | undefined;
  
  public uploadFiles(filesModel: FilesModel): Observable<boolean> {
    const totalFiles = filesModel.files.length;
    return new Observable<boolean>((observer) => {
      let successCount = 0;
      let errorCount = 0;

      for (let i = 0; i < totalFiles; i++) {
        const formData: FormData = new FormData();
        formData.append('File', filesModel.files[i]);
        formData.append('FilePath', filesModel.filePath);

        this.http.post<boolean>('file/upload', formData).subscribe(
          (response) => {
            this.toastr.success(
              `${filesModel.files[i].name} uploaded successfully`
            );
            successCount++;
            if (successCount + errorCount === totalFiles) {
              observer.next(true);
              observer.complete();
            }
          },
          (error) => {
            this.toastr.error(`${filesModel.files[i].name} not uploaded`);
            errorCount++;
            if (successCount + errorCount === totalFiles) {
              observer.next(false);
              observer.complete();
            }
          }
        );
      }
    });
  }

  public replaceFile(model: FileModel): Observable<boolean> {
    const formData: FormData = new FormData();
    formData.append('File', model.file);
    formData.append('FilePath', model.filePath);
    return this.http.post<boolean>('file/replace', formData);
  }

  public downloadFile(filePath: string): Observable<Blob> {
    const encodedPath = encodeURIComponent(filePath);
    return this.http.get<Blob>(`file/download-file?filePath=${encodedPath}`, {
      responseType: 'blob' as 'json',
    });
  }

  public downloadFileForEdit(filePath: string): Observable<Blob> {
    const encodedPath = encodeURIComponent(filePath);
    return this.http.get<Blob>(`file/download-file?filePath=${encodedPath}`);
  }

  public deleteFile(folderPath: string, fileName: string): Observable<boolean> {
    const encodedPath = encodeURIComponent(folderPath);
    const encodedName = encodeURIComponent(fileName);
    return this.http.delete<boolean>(
      `file/delete?fileName=${encodedName}&filePath=${encodedPath}`
    );
  }
}
