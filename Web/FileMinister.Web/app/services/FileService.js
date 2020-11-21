/// <reference path="../common/angular.d.ts"/>
var FileService = /** @class */ (function () {
    function FileService(ServiceBase) {
        this.ServiceBase = ServiceBase;
    }
    FileService.prototype.GetAll = function () {
        var config = new RequestConfig();
        config.url = 'FileVersion/';
        return this.ServiceBase.get(config);
    };
    FileService.prototype.GetAllUserShares = function () {
        var config = new RequestConfig();
        config.url = 'File/GetAllUserShares';
        return this.ServiceBase.get(config);
    };
    FileService.prototype.GetAllChildren = function (data) {
        var config = new RequestConfig();
        config.url = 'File/GetAllChildren';
        config.method = "POST";
        config.data = data;
        return this.ServiceBase.post(config);
    };
    FileService.prototype.GetAllDeletedChildren = function (data) {
        var config = new RequestConfig();
        config.url = 'File/GetAllDeletedChildren';
        config.method = "POST";
        config.data = data;
        return this.ServiceBase.post(config);
    };
    FileService.prototype.GetVersionHistory = function (id) {
        var config = new RequestConfig();
        config.url = 'FileVersion/GetVersionHistory?id=' + id;
        //todo: need to convert in async call
        return this.ServiceBase.get(config);
    };
    FileService.prototype.CheckOut = function (id, version) {
        var config = new RequestConfig();
        config.url = 'File/CheckOut';
        config.data = { fileEntryId: id, localFileVersionNumber: version, workSpaceId: Settings.WebWorkSpaceId };
        return this.ServiceBase.post(config);
    };
    FileService.prototype.CheckIn = function (id, version, size, blobName, fileHash) {
        var config = new RequestConfig();
        config.url = 'File/CheckInWeb';
        config.data = { id: id, version: version, fileSize: size, blobName: blobName, fileHash: fileHash };
        return this.ServiceBase.post(config);
    };
    FileService.prototype.UndoCheckout = function (id) {
        var config = new RequestConfig();
        config.url = 'File/UndoCheckOut';
        config.data = { fileEntryId: id };
        return this.ServiceBase.post(config);
    };
    FileService.prototype.IsFileExists = function (id, fileName) {
        var config = new RequestConfig();
        config.url = 'File/CheckFileExists';
        config.data = { id: id, fileName: fileName };
        return this.ServiceBase.post(config);
    };
    FileService.prototype.AddFileAndCheckout = function (id, blobName, fileName, fileSize) {
        var config = new RequestConfig();
        config.url = 'File/AddFileAndCheckoutWeb';
        config.method = "POST";
        config.data = { id: id, fileName: fileName, blobName: blobName, fileSize: fileSize };
        return this.ServiceBase.post(config);
    };
    FileService.prototype.GetBlobNameSasUrl = function (id, filesize) {
        var config = new RequestConfig();
        config.url = 'sync/' + id + '/' + filesize + '/SasUrlBlob';
        return this.ServiceBase.get(config);
    };
    FileService.prototype.UploadBlob = function (url, requestData) {
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
    };
    FileService.prototype.DownloadBlob = function (id, name, version) {
        var config = new RequestConfig();
        config.url = 'Sync/' + id + '/' + version + '/' + name + '/SasUrlBlob';
        return this.ServiceBase.get(config);
    };
    FileService.prototype.DownloadDeletedBlob = function (id, name, version) {
        var config = new RequestConfig();
        config.url = 'Sync/' + id + '/' + version + '/' + name + '/true/SasUrlBlob';
        return this.ServiceBase.get(config);
    };
    FileService.prototype.CreateFolder = function (id, name) {
        var config = new RequestConfig();
        config.url = 'File/AddFolderWeb';
        config.data = { id: id, name: name };
        return this.ServiceBase.post(config);
    };
    FileService.prototype.Rename = function (fileEntryId, versionNumber, newName) {
        var config = new RequestConfig();
        config.url = 'File/RenameFile';
        config.data = { fileEntryId: fileEntryId, versionNumber: versionNumber, newName: newName };
        return this.ServiceBase.post(config);
    };
    FileService.prototype.SyncServerData = function () {
        var config = new RequestConfig();
        config.url = 'Sync/server-data';
        return this.ServiceBase.post(config);
    };
    FileService.prototype.ConvertToMainGridModel = function (data) {
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
    };
    FileService.prototype.Move = function (fileEntryId, destinationFileEntryId, isReplaceExistingFile, newFileName) {
        var config = new RequestConfig();
        config.url = 'File/MoveFile';
        config.data = { fileEntryId: fileEntryId, destinationFileEntryId: destinationFileEntryId, isReplaceExistingFile: isReplaceExistingFile, newFileName: newFileName };
        return this.ServiceBase.post(config);
    };
    FileService.prototype.Delete = function (fileEntryId, currentVersionNumber) {
        var config = new RequestConfig();
        config.url = 'File/SoftDelete';
        config.data = { fileEntryId: fileEntryId, localFileVersionNumber: currentVersionNumber };
        return this.ServiceBase.post(config);
    };
    FileService.prototype.DeleteMultiple = function (fileList) {
        var config = new RequestConfig();
        config.url = 'File/SoftDeleteMultiple';
        config.data = fileList;
        return this.ServiceBase.post(config);
    };
    FileService.prototype.Restore = function (fileEntryId) {
        var config = new RequestConfig();
        config.url = 'File/UndoDelete';
        config.data = { fileEntryId: fileEntryId };
        return this.ServiceBase.post(config);
    };
    FileService.prototype.Purge = function (fileEntryId) {
        var config = new RequestConfig();
        config.url = 'File/HardDelete';
        config.data = { fileEntryId: fileEntryId };
        return this.ServiceBase.post(config);
    };
    FileService.prototype.PurgeMultiple = function (fileList) {
        var config = new RequestConfig();
        config.url = 'File/HardDeleteMultiple';
        config.data = fileList;
        return this.ServiceBase.post(config);
    };
    return FileService;
}());
angular.module('cloudSync').service('FileService', FileService);
//# sourceMappingURL=FileService.js.map