import { Component, OnInit } from '@angular/core';
import { FolderService } from '@@services/folder.service';
import { FileService } from '@@services/file.service';

import {
  CellClickEvent,
  DataStateChangeEvent,
  GridDataResult,
  PagerPosition,
  PagerType,
} from '@progress/kendo-angular-grid';

import {
  SVGIcon,
  arrowLeftIcon,
  arrowRightIcon,
  arrowRotateCcwIcon,
  arrowUpIcon,
  changeManuallyIcon,
  clearCssIcon,
  downloadIcon,
  editToolsIcon,
  exeIcon,
  fileAudioIcon,
  fileExcelIcon,
  fileImageIcon,
  filePdfIcon,
  fileProgrammingIcon,
  fileTxtIcon,
  fileTypescriptIcon,
  fileVideoIcon,
  fileWordIcon,
  fileZipIcon,
  folderAddIcon,
  folderIcon,
  homeIcon,
  saveIcon,
  trackChangesAcceptIcon,
  trackChangesRejectIcon,
  trashIcon,
  uploadIcon,
  xIcon,
} from '@progress/kendo-svg-icons';
import { BreadCrumbItem } from '@progress/kendo-angular-navigation';
import { ToastrService } from 'ngx-toastr';
import {
  CompositeFilterDescriptor,
  FilterDescriptor,
  SortDescriptor,
} from '@progress/kendo-data-query';
import { FilterExpression } from '@progress/kendo-angular-filter';
import { FolderGetAllQuery } from '@@services/models/queries/folderGetAllQuery';
import { FileInfo } from '@@services/models/fileInfo';
import { GridState } from '@@services/models/filter-helper/grid-state';
import { SubFilter } from '@@services/models/filter-helper/sub-filter';
import { InputSize } from '@progress/kendo-angular-inputs';
import { ButtonFillMode } from '@progress/kendo-angular-buttons';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { DialogThemeColor } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  public path: string = '';
  public folderName: string = '';
  public file!: File;
  public pathes: string[] = [];
  public maxPathes: string[] = [];
  public pathesForward: string[] = [];

  public flashok: number = 0;

  public shouldWork: number = 1;
  public fileNameWhileDownload: string = '';

  public isLoading: boolean = false;

  private defaultItems: BreadCrumbItem[] = [
    {
      text: 'Home',
      title: 'Home',
      svgIcon: homeIcon,
    },
  ];

  public items: BreadCrumbItem[] = [...this.defaultItems];
  public homeIcon: SVGIcon = homeIcon;
  public rotateIcon: SVGIcon = arrowRotateCcwIcon;

  public isPaginationVisible: boolean = true;

  private fileIcons: { [key: string]: SVGIcon } = {
    default: xIcon,
    folder: folderIcon,
    empty: folderIcon,
    '.pdf': filePdfIcon,
    '.jpg': fileImageIcon,
    '.jpeg': fileImageIcon,
    '.png': fileImageIcon,
    '.gif': fileImageIcon,
    '.xlsx': fileExcelIcon,
    '.xls': fileExcelIcon,
    '.docx': fileWordIcon,
    '.doc': fileWordIcon,
    '.txt': fileTxtIcon,
    '.mp4': fileVideoIcon,
    '.exe': exeIcon,
    '.py': fileProgrammingIcon,
    '.js': fileProgrammingIcon,
    '.mp3': fileAudioIcon,
    '.ts': fileTypescriptIcon,
    '.zip': fileZipIcon,
    left: arrowLeftIcon,
    right: arrowRightIcon,
    up: arrowUpIcon,
    trash: trashIcon,
    download: downloadIcon,
    editFile: editToolsIcon,
    addFolder: folderAddIcon,
    upload: uploadIcon,
    clear: clearCssIcon,
    saveEditedFile: trackChangesAcceptIcon,
    rejectEditFile: trackChangesRejectIcon,
  };

  public leftSVG: SVGIcon = this.fileIcons['left'];
  public rightSVG: SVGIcon = this.fileIcons['right'];
  public upSVG: SVGIcon = this.fileIcons['up'];
  public trashSVG: SVGIcon = this.fileIcons['trash'];
  public downloadSVG: SVGIcon = this.fileIcons['download'];
  public editFileSVG: SVGIcon = this.fileIcons['editFile'];
  public addFolderSVG: SVGIcon = this.fileIcons['addFolder'];
  public uploadSVG: SVGIcon = this.fileIcons['upload'];
  public clearSVG: SVGIcon = this.fileIcons['clear'];
  public saveEditedFileSVG: SVGIcon = this.fileIcons['saveEditedFile'];
  public rejectEditFileSVG: SVGIcon = this.fileIcons['rejectEditFile'];

  public fileContent: string = '';
  public editedFileContent: string = '';

  public nameForEditedFile: string = '';

  public opened: boolean = false;
  public openedFilter: boolean = false;

  public isValidFolderName: boolean = false;
  public isValidFile: boolean = false;

  public extensionResponse: string = 'all';
  public fileNameResponse: string = '';

  public pathesForBack: string[] = [];

  public filters: FilterExpression[] = [
    {
      field: 'country',
      title: 'Country',
      editor: 'string',
      operators: ['neq', 'eq'],
    },
    {
      field: 'budget',
      editor: 'number',
    },
  ];

  public listFilterItems: Array<string> = [
    'folder',
    '.mp3',
    '.mp4',
    '.png',
    '.txt',
    '.doc',
    'all',
  ];

  public type: PagerType = 'numeric';
  public buttonCount = 4;
  public info = true;
  public pageSizes = [5, 8, 12, 16, 20, 50, 100];
  public previousNext = true;
  public position: PagerPosition = 'bottom';

  public foldersView: GridDataResult = {
    data: [],
    total: 0,
  };

  public query: FolderGetAllQuery = new FolderGetAllQuery();

  public sort: SortDescriptor[] = [
    {
      field: 'fileName',
      dir: undefined,
    },
  ];

  public gridState: GridState = this.createInitialState();

  public size: InputSize = 'large';
  public fillMode: ButtonFillMode = 'outline';

  constructor(
    private toastr: ToastrService,
    private folderService: FolderService,
    private fileService: FileService,
    private location: Location,
    private router: Router
  ) {}

  public backPathes: string[] = [];
  public forwardPathes: string[] = [];
  public urlPath: string = '';

  ngOnInit(): void {
    this.urlPath = this.location.path();
    this.setLastRequestParamsFromUrl();
    this.loadData();
  }

  public setLastRequestParamsFromUrl(): void {
    if (this.urlPath.indexOf('/folder?path=') === -1) return;
    if (this.urlPath.indexOf('Skip=') === -1) return;
    if (this.urlPath.indexOf('Take=') === -1) return;

    const decodedPath = decodeURIComponent(this.urlPath).replace('/folder?path=','');
    const urlParams = decodedPath.split('&');

    this.path = urlParams[0];
    this.query.pagination.skip = parseInt(urlParams[1].replace('Skip=', ''));
    this.query.pagination.take = parseInt(urlParams[2].replace('Take=', ''));
  }

  public loadData(): void {
    this.isLoading = true;

    this.query.sort.sortField = this.sort[0].field;
    this.query.sort.sortDirection = this.sort[0].dir;
    this.query.folderPath = this.path;
    this.query.filter['Filter.Logic'] = 'and';
    this.query.filter['Filter.Filters'] = this.convertFilters(
      this.gridState.filter
    );

    this.folderService.getAll(this.query).subscribe(
      (response) => {
        this.foldersView = {
          data: response.filesInfo,
          total: response.totalCount,
        };
        this.path = response.folderPath;
        this.query.folderPath = response.folderPath;
        this.pathes = this.query.folderPath
          .split('/')
          .flatMap((item) => (item ? [item, '/'] : ['/']));

        if (
          this.pathes.length > 0 &&
          this.pathes[this.pathes.length - 1] === '/'
        ) {
          this.pathes.pop();
        }

        if (this.pathes.length > 1) {
          const exitRow: FileInfo = new FileInfo();
          exitRow.fileName = '...';
          exitRow.fileExtension = '';
          exitRow.fileCreationTime = null;
          exitRow.size = null;

          this.foldersView.data.unshift(exitRow);
        }

        if (
          this.backPathes.length == 0 ||
          (this.keyPath &&
            this.backPathes[this.backPathes.length - 1] !== this.path)
        ) {
          this.backPathes.push(this.path);
        }
        this.keyPath = true;
        this.urlPath = `folder?path=${this.path}&Skip=${this.query.pagination.skip}&Take=${this.query.pagination.take}`;
        this.router.navigateByUrl(this.urlPath);
        this.isLoading = false;
      },
      (error) => {
        this.isLoading = false;
      }
    );
  }

  public checkAmountItemsForPagination():void{
    const isAmountOfItemsEnough = this.foldersView.total > this.query.pagination.skip + 1;
    if( isAmountOfItemsEnough ) return;
    this.query.pagination.skip -= this.query.pagination.take;
    this.urlPath = `folder?path=${this.path}&Skip=${this.query.pagination.skip}&Take=${this.query.pagination.take}`;
    this.setLastRequestParamsFromUrl();
  }

  public itemsDataStateChange(data: DataStateChangeEvent): void {
    this.query.pagination.skip = data.skip;
    this.query.pagination.take = data.take;

    if (data.filter) {
      this.gridState.filter = data.filter;
    } else {
      this.gridState.filter = {
        logic: 'and',
        filters: [],
      };
    }
    if (data.sort && data.sort.length == 1) {
      this.sort = data.sort;
    }

    this.loadData();
  }

  private createInitialState(): GridState {
    return {
      filter: {
        filters: [],
        logic: 'and',
      },
    };
  }

  public getIconForExtension(extension: string): SVGIcon {
    if (extension == '') return this.fileIcons['empty'];
    return this.fileIcons[extension.toLowerCase()] || fileTypescriptIcon;
  }
  public keyPath = true;

  public goBack(): void {
    this.keyPath = false;
    if (this.backPathes.length <= 1) {
      this.keyPath = true;
      return;
    }
    this.path = this.backPathes[this.backPathes.length - 2];
    this.forwardPathes.push(this.backPathes[this.backPathes.length - 1]);
    this.query.pagination.skip = 0;
    this.urlPath = `folder?path=${this.path}&Skip=${this.query.pagination.skip}&Take=${this.query.pagination.take}`;
    this.setLastRequestParamsFromUrl();
    this.backPathes.pop();
    this.loadData();
  }

  public goForward(): void {
    if (this.forwardPathes.length == 0) return;
    this.path = this.forwardPathes[this.forwardPathes.length - 1];
    this.forwardPathes.pop();
    this.query.pagination.skip = 0;
    this.urlPath = `folder?path=${this.path}&Skip=${this.query.pagination.skip}&Take=${this.query.pagination.take}`;
    this.setLastRequestParamsFromUrl();
    this.loadData();
  }

  public goUp(): void {
    const index = this.path.lastIndexOf('/');
    if (index == -1) return;
    this.path = this.path.substring(0, index);
    this.query.pagination.skip = 0;
    this.urlPath = `folder?path=${this.path}&Skip=${this.query.pagination.skip}&Take=${this.query.pagination.take}`;
    this.setLastRequestParamsFromUrl();
    this.loadData();
  }

  public getIndex(index: number): void {
    const amount = this.pathes.length - index - 1;
    this.path = '';
    for (let i = 0; i < amount; i++) {
      this.pathes.pop();
    }
    for (let i = 0; i < this.pathes.length; i++) {
      this.path += this.pathes[i];
    }
    this.query.pagination.skip = 0;
    this.urlPath = `folder?path=${this.path}&Skip=${this.query.pagination.take}&Take=${this.query.pagination.take}`;
    this.setLastRequestParamsFromUrl();
    this.loadData();
  }

  public addFolder(): void {
    if (this.folderName === null || this.folderName === '') {
      this.isValidFolderName = true;
      return;
    }

    this.isLoading = true;

    this.folderService.createFolder(this.folderName, this.path).subscribe(
      (response) => {
        this.toastr.success('Folder created successfully ');
        this.folderName = '';
        this.loadData();
      },
      (error) => {
        this.toastr.error('Error occured ');
        this.folderName = '';
        this.loadData();
      }
    );
  }
       
  public deleteItem(path: string): void {
    this.isLoading = true;
    this.folderService.deleteFolder(this.path + '/' + path).subscribe(
      (response) => {
        this.toastr.success('Folder deleted successfully ');
        this.checkAmountItemsForPagination();
        this.loadData();
      },
      (error) => {
        this.loadData();
      }
    );
  }

  private saveFile(response: Blob, filePath: string): void {
    const blob = new Blob([response], { type: 'application/zip' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    document.body.appendChild(a);
    a.style.display = 'none';
    a.href = url;
    a.download = `${filePath}.zip`;
    a.click();
    window.URL.revokeObjectURL(url);
  }

  public downloadAsZip(filePath: string): void {
    ++this.flashok;
    this.isLoading = true;
    this.folderService.dowloadZip(this.path + '/' + filePath).subscribe(
      (response) => {
        this.saveFile(response, filePath);
        this.toastr.success('Folder downloaded successfully ');
        this.isLoading = false;
      },
      (error) => {
        this.toastr.error('Error occured while folder downloading');
        this.isLoading = false;
      }
    );
  }

  public downloadFile(filePath: string): void {
    this.isLoading = true;
    this.fileService.downloadFile(filePath).subscribe(
      (response: Blob) => {
        this.toastr.success('File downloaded successfully ');
        const fileExtension = filePath.split('.').pop() || 'unknown';

        const blob = new Blob([response], {
          type: `application/${fileExtension}`,
        });

        const link = document.createElement('a');

        link.download = `${this.fileNameWhileDownload}.${fileExtension}`;
        link.href = window.URL.createObjectURL(blob);

        document.body.appendChild(link);
        link.click();

        document.body.removeChild(link);
        window.URL.revokeObjectURL(link.href);
        this.isLoading = false;
      },
      (error) => {
        this.isLoading = false;
        this.toastr.error('Error occured while file downloading');
      }
    );
  }

  public cellClickHandler(args: CellClickEvent): void {
    if (args.dataItem.fileCreationTime === null) {
      this.goUp();
      return;
    }
    if (args.dataItem.fileExtension === 'folder' && this.flashok === 0) {
      this.pathesForward.length = 0;
      this.shouldWork++;
      this.path = this.path + '/' + args.dataItem.fileName;
      this.query.pagination.skip = 0;
      this.query.pagination.take = 5;
      this.urlPath = `folder?path=${this.path}&Skip=${this.query.pagination.skip}&Take=${this.query.pagination.take}`;
      this.setLastRequestParamsFromUrl();
      this.loadData();
    } else if (args.dataItem.fileExtension !== 'folder' && this.flashok === 0) {
      this.fileNameWhileDownload = args.dataItem.fileName;
      const path = this.path + '/' + args.dataItem.fileName;
      this.downloadFile(path);
    }

    this.flashok = 0;
  }
  public getTextOfFile(dataItem: FileInfo): void {
    this.flashok++;
    this.nameForEditedFile = dataItem.fileName;
    this.isLoading = true;
    this.fileService
      .downloadFileForEdit(this.path + '/' + dataItem.fileName)
      .subscribe(
        (response) => {
          this.handleBlobResponse(response);
          this.opened = true;
          this.isLoading = false;
        },
        (error) => {
          this.isLoading = false;
        }
      );
  }

  private handleBlobResponse(blob: Blob): void {
    const reader = new FileReader();
    reader.onload = (event) => {
      if (event.target) {
        this.fileContent = event.target.result as string;
      }
    };
    reader.readAsText(blob);
  }

  public closeEditModal(status: string): void {
    this.opened = false;
    if (status === 'save') {
      this.generateTextFile();
    } else {
      this.editedFileContent = this.fileContent;
    }
  }

  private generateTextFile(): void {
    const blob = new Blob([this.editedFileContent], {
      type: 'text/plain;charset=utf-8',
    });

    this.file = new File([blob], this.nameForEditedFile, {
      type: 'text/plain',
    });
    console.log('Blob + ' + this.file);
    this.isLoading = true;
    this.fileService
      .replaceFile(this.file, this.path )
      .subscribe(
        (response) => {
          this.isLoading = false;
          this.loadData();
          this.toastr.success('File edited successfully ');
        },
        (error) => {
          this.isLoading = false;
        }
      );
  }

  public delete(dataItem: FileInfo): void {
    this.flashok++;
    if (dataItem.fileExtension === 'folder') {
      this.deleteItem(dataItem.fileName);
    } else {
      this.isLoading = true;
      this.fileService.deleteFile(this.path, dataItem.fileName).subscribe(
        (response) => {
          this.checkAmountItemsForPagination();
          this.loadData();
          this.toastr.success('File deleted successfully ');
        },
        (error) => {
          this.loadData();
        }
      );
    }
  }

  private convertFilters(filter: CompositeFilterDescriptor): SubFilter[] {
    const result: SubFilter[] = [];
    for (let i = filter.filters.length - 1; i >= 0; i--) {
      const currentFilter: CompositeFilterDescriptor = <any>filter.filters[i];
      if (!currentFilter || !currentFilter.logic) {
        filter.filters.splice(i, 1);
      }
    }
    for (let i = 0; i < filter.filters.length; i++) {
      const currentFilter: CompositeFilterDescriptor = <any>filter.filters[i];
      if (currentFilter)
        result.push({
          logic: currentFilter.logic,
          filters:
            currentFilter.filters?.map((x) => {
              const descriptor: FilterDescriptor = <any>x;
              let strVal;
              if (
                typeof descriptor.value == 'object' &&
                descriptor.value.constructor == Date
              ) {
                if (
                  descriptor.operator === 'lte' ||
                  descriptor.operator === 'gt'
                ) {
                  const oneDayInMs = 24 * 60 * 60 * 1000;
                  strVal = new Date(
                    descriptor.value.getTime() + oneDayInMs
                  ).toISOString();
                } else {
                  strVal = descriptor.value.toISOString();
                }
              } else {
                strVal = descriptor.value;
              }
              return {
                field: <string>(<any>descriptor.field),
                operator: <string>(<any>descriptor.operator),
                value: strVal,
              };
            }) || [],
        });
    }
    return result;
  }

  public uploadFiles(): void {
    if (this.mySelectedFiles == null || this.mySelectedFiles.length === 0) {
      this.isValidFile = true;
      return;
    }
    this.isLoading = true;
    this.fileService.uploadFiles(this.mySelectedFiles, this.path).subscribe(
      (response) => {
        this.clearFiles();
        this.loadData();
      },
      (error) => {
        this.clearFiles();
        this.loadData();
      }
    );
  }

  public mySelectedFiles: File[] = [];
  public selectModalVisible: boolean = false;
  public dialogThemeColor: DialogThemeColor = 'dark';

  public clearFiles(): void {
    this.mySelectedFiles = [];
    this.selectModalVisible = false;
  }

  public valueChange(): void {
    if (this.mySelectedFiles === null) {
      this.selectModalVisible = false;
    } else if (this.mySelectedFiles.length > 0) {
      this.selectModalVisible = true;
    }
  }

  public displaySelectModal(): void {
    if (this.mySelectedFiles === null) {
      this.selectModalVisible = false;
    } else if (this.mySelectedFiles.length > 0) {
      this.selectModalVisible = true;
    }
  }

  public closeSelectModal(): void {
    this.mySelectedFiles = [];
    this.selectModalVisible = false;
  }
}
