import { FileInfo } from './fileInfo.model';

export interface FolderGetAllModel {
  totalCount: number;
  folderPath: string;
  filesInfo: FileInfo[];
}
