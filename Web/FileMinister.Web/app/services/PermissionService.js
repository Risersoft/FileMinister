/// <reference path="../common/angular.d.ts"/>
var PermissionService = /** @class */ (function () {
    function PermissionService(ServiceBase) {
        this.ServiceBase = ServiceBase;
    }
    PermissionService.prototype.getUsersAndGroupsByFileId = function (fileId) {
        var config = new RequestConfig();
        config.url = 'UserFilePermission/' + fileId + '/UsersGroups';
        return this.ServiceBase.get(config);
    };
    PermissionService.prototype.getPermissionByFileAndUser = function (fileId, userId) {
        var config = new RequestConfig();
        config.url = 'UserFilePermission/UPermission?fileId=' + fileId + '&userId=' + userId;
        return this.ServiceBase.get(config);
    };
    PermissionService.prototype.getPermissionByFileAndGroup = function (fileId, groupId) {
        var config = new RequestConfig();
        config.url = 'GroupFilePermission/GPermission?fileId=' + fileId + '&groupId=' + groupId;
        return this.ServiceBase.get(config);
    };
    PermissionService.prototype.getEditPermission = function (fileId) {
        var config = new RequestConfig();
        config.url = 'file/' + fileId + '/CheckEditAndChangeShareAdminPermission';
        return this.ServiceBase.get(config);
    };
    PermissionService.prototype.getShareAdminPermission = function (fileId) {
        var config = new RequestConfig();
        config.url = 'file/' + fileId + '/ChecksShareAdminPermission';
        return this.ServiceBase.get(config);
    };
    PermissionService.prototype.getEditTagPermission = function (fileId) {
        var config = new RequestConfig();
        config.url = 'file/' + fileId + '/CheckChangeTagPermission';
        return this.ServiceBase.get(config);
    };
    PermissionService.prototype.getDeletedFilesPermission = function () {
        var config = new RequestConfig();
        config.url = 'file/CheckViewDeletedFiles';
        return this.ServiceBase.get(config);
    };
    PermissionService.prototype.isAccountAdmin = function () {
        var config = new RequestConfig();
        config.url = 'file/AccountAdmin';
        return this.ServiceBase.get(config);
    };
    PermissionService.prototype.savePermissionByFileAndGroup = function (fileId, data) {
        var config = new RequestConfig();
        config.url = 'UserFilePermission/UpdateFilePermissionForUsersGroups';
        config.data = { item1: fileId, item2: data.UpdatedUsersAndGroupsPermissions, item3: data.AddedUsersAndGroupsPermissions, item4: data.RemovedUsersAndGroupsPermissions };
        return this.ServiceBase.post(config);
    };
    return PermissionService;
}());
angular.module('cloudSync').service('PermissionService', PermissionService);
//# sourceMappingURL=PermissionService.js.map