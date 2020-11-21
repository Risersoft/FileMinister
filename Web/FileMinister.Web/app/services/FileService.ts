/// <reference path="../common/angular.d.ts"/>
class FileService implements IFileService {
    constructor(public ServiceBase: ServiceBase) {
    }

    public GetAll(): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'FileVersion/';
        return this.ServiceBase.get(config);
    }

    public GetAllUserShares(): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/GetAllUserShares';
        return this.ServiceBase.get(config);
    }

    public GetAllChildren(data): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/GetAllChildren';
        config.method = "POST";
        config.data = data;
        return this.ServiceBase.post(config);

    }

    public GetAllDeletedChildren(data): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/GetAllDeletedChildren';
        config.method = "POST";
        config.data = data;
        return this.ServiceBase.post(config);

    }

    public GetVersionHistory(id: any): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'FileVersion/GetVersionHistory?id=' + id;
        //todo: need to convert in async call

        return this.ServiceBase.get(config);
    }

    public CheckOut(id: any, version: any): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/CheckOut';
        config.data = { fileEntryId: id, localFileVersionNumber: version, workSpaceId: Settings.WebWorkSpaceId };

        return this.ServiceBase.post(config);
    }

    public CheckIn(id: any, version: any, size: any, blobName: any, fileHash: any): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/CheckInWeb';
        config.data = { id: id, version: version, fileSize: size, blobName: blobName, fileHash: fileHash };
        return this.ServiceBase.post(config);
    }

    public UndoCheckout(id: any): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/UndoCheckOut';
        config.data = { fileEntryId: id };
        return this.ServiceBase.post(config);
    }

    public IsFileExists(id: any, fileName: any): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/CheckFileExists';
        config.data = { id: id, fileName: fileName };
        return this.ServiceBase.post(config);
    }

    public AddFileAndCheckout(id: any, blobName: any, fileName: string, fileSize: any): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/AddFileAndCheckoutWeb';
        config.method = "POST";
        config.data = { id: id, fileName: fileName, blobName: blobName, fileSize: fileSize };
        return this.ServiceBase.post(config);
    }

    public GetBlobNameSasUrl(id: any, filesize: any): ng.IPromise<any> {
        var config = new RequestConfig()
        config.url = 'sync/' + id + '/' + filesize + '/SasUrlBlob';
        return this.ServiceBase.get(config);
    }

    public UploadBlob(url: any, requestData: any): ng.IPromise<any> {
        //var config = new RequestConfig()
        //config.headers = config.headers || {};
        //config.headers["x-ms-blob-type"] = "BlockBlob";
        ////config.headers["Content-Length"] = contentLength;
        //return this.ServiceBase.$http.put(url, requestData, config);

        var config = new RequestConfig();
        config.headers = config.headers || {};
        config.headers["x-ms-blob-type"] = "BlockBlob";
        config.url = url;
        config.data = requestData;
        config.method = "PUT";
        return this.ServiceBase.$http(config);
    }

    public DownloadBlob(id: any, name: any, version: any): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'Sync/' + id + '/' + version + '/' + name + '/SasUrlBlob';
        return this.ServiceBase.get(config);
    }

    public DownloadDeletedBlob(id: any, name: any, version: any): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'Sync/' + id + '/' + version + '/' + name + '/true/SasUrlBlob';
        return this.ServiceBase.get(config);
    }

    public CreateFolder(id: any, name: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/AddFolderWeb';
        config.data = { id: id, name: name };
        return this.ServiceBase.post(config);
    }

    public Rename(fileEntryId: any, versionNumber: any, newName: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/RenameFile';
        config.data = { fileEntryId: fileEntryId, versionNumber: versionNumber, newName: newName };
        return this.ServiceBase.post(config);
    }

    public SyncServerData(): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'Sync/server-data';
        return this.ServiceBase.post(config);
    }

    public ConvertToMainGridModel(data: any): any {
        var allChildNodes = [];
        angular.forEach(data, function (item, index) {
            var relativePath = item["FileVersion"]["FileEntryRelativePath"];
            if (relativePath) {
                var fileNameLength = item["FileVersion"]["FileEntryNameWithExtension"].length;
                relativePath = relativePath.substring(0, relativePath.length - fileNameLength - 1);

                if (relativePath.length <= 0)
                    relativePath = "(root)";
            }


            allChildNodes.push({
                folder: item["FileEntryTypeId"] == 2,
                open: false,
                name: item["FileVersion"]["FileEntryNameWithExtension"],
                path: relativePath,
                value: item["FileEntryId"],
                version: item["CurrentVersionNumber"],
                CheckedOutBy: item["CheckedOutByUserName"],
                LastCheckedIn: new Date(item["FileVersion"]["CreatedOnUTC"]).toLocaleString("en-IN"),
                children: [],
                CanWrite: item["CanWrite"],
                fileShareId: item["FileShareId"],
                currentVersionNumber: item["CurrentVersionNumber"],
                isDeleted: item["IsDeleted"] || item["FileVersion"]["IsDeleted"]
            });

        });
        return allChildNodes;
    }      

    public Move(fileEntryId: string, destinationFileEntryId: string, isReplaceExistingFile: boolean, newFileName: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/MoveFile';
        config.data = { fileEntryId: fileEntryId, destinationFileEntryId: destinationFileEntryId, isReplaceExistingFile: isReplaceExistingFile, newFileName: newFileName };
        return this.ServiceBase.post(config);
    }

    public Delete(fileEntryId: string, currentVersionNumber: number): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/SoftDelete';
        config.data = { fileEntryId: fileEntryId, localFileVersionNumber: currentVersionNumber};
        return this.ServiceBase.post(config);
    }

    public DeleteMultiple(fileList: any): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/SoftDeleteMultiple';
        config.data = fileList;
        return this.ServiceBase.post(config);
    }

    public Restore(fileEntryId: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/UndoDelete';
        config.data = { fileEntryId: fileEntryId };
        return this.ServiceBase.post(config);
    }

    public Purge(fileEntryId: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/HardDelete';
        config.data = { fileEntryId: fileEntryId };
        return this.ServiceBase.post(config);
    }

    public PurgeMultiple(fileList: any): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'File/HardDeleteMultiple';
        config.data = fileList;
        return this.ServiceBase.post(config);
    }
}
angular.module('cloudSync').service('FileService', FileService);