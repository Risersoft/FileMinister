/// <reference path="../common/angular.d.ts" />
declare class FileService implements IFileService {
    ServiceBase: ServiceBase;
    constructor(ServiceBase: ServiceBase);
    GetAll(): ng.IPromise<any>;
    GetAllUserShares(): ng.IPromise<any>;
    GetAllChildren(data: any): ng.IPromise<any>;
    GetAllDeletedChildren(data: any): ng.IPromise<any>;
    GetVersionHistory(id: any): ng.IPromise<any>;
    CheckOut(id: any, version: any): ng.IPromise<any>;
    CheckIn(id: any, version: any, size: any, blobName: any, fileHash: any): ng.IPromise<any>;
    UndoCheckout(id: any): ng.IPromise<any>;
    IsFileExists(id: any, fileName: any): ng.IPromise<any>;
    AddFileAndCheckout(id: any, blobName: any, fileName: string, fileSize: any): ng.IPromise<any>;
    GetBlobNameSasUrl(id: any, filesize: any): ng.IPromise<any>;
    UploadBlob(url: any, requestData: any): ng.IPromise<any>;
    DownloadBlob(id: any, name: any, version: any): ng.IPromise<any>;
    DownloadDeletedBlob(id: any, name: any, version: any): ng.IPromise<any>;
    CreateFolder(id: any, name: string): ng.IPromise<any>;
    Rename(fileEntryId: any, versionNumber: any, newName: string): ng.IPromise<any>;
    SyncServerData(): ng.IPromise<any>;
    ConvertToMainGridModel(data: any): any;
    Move(fileEntryId: string, destinationFileEntryId: string, isReplaceExistingFile: boolean, newFileName: string): ng.IPromise<any>;
    Delete(fileEntryId: string, currentVersionNumber: number): ng.IPromise<any>;
    DeleteMultiple(fileList: any): ng.IPromise<any>;
    Restore(fileEntryId: string): ng.IPromise<any>;
    Purge(fileEntryId: string): ng.IPromise<any>;
    PurgeMultiple(fileList: any): ng.IPromise<any>;
}
