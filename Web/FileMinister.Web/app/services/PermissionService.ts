/// <reference path="../common/angular.d.ts"/>

class PermissionService implements IPermissionService {
    constructor(public ServiceBase: ServiceBase) {
    }

    public getUsersAndGroupsByFileId(fileId: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'UserFilePermission/' + fileId + '/UsersGroups';
        return this.ServiceBase.get(config);
    }

    public getPermissionByFileAndUser(fileId: string, userId: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'UserFilePermission/UPermission?fileId=' + fileId + '&userId=' + userId;
        return this.ServiceBase.get(config);
    }

    public getPermissionByFileAndGroup(fileId: string, groupId: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'GroupFilePermission/GPermission?fileId=' + fileId + '&groupId=' + groupId;
        return this.ServiceBase.get(config);
    }

    public getEditPermission(fileId: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'file/' + fileId + '/CheckEditAndChangeShareAdminPermission';
        return this.ServiceBase.get(config);
    }

    public getShareAdminPermission(fileId: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'file/' + fileId + '/ChecksShareAdminPermission';
        return this.ServiceBase.get(config);
    }

    public getEditTagPermission(fileId: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'file/' + fileId + '/CheckChangeTagPermission';
        return this.ServiceBase.get(config);
    }

    public getDeletedFilesPermission(): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'file/CheckViewDeletedFiles';
        return this.ServiceBase.get(config);
    }

    public isAccountAdmin(): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'file/AccountAdmin';
        return this.ServiceBase.get(config);
    }

    public savePermissionByFileAndGroup(fileId: any, data: any): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'UserFilePermission/UpdateFilePermissionForUsersGroups';
        config.data = { item1: fileId, item2: data.UpdatedUsersAndGroupsPermissions, item3: data.AddedUsersAndGroupsPermissions, item4: data.RemovedUsersAndGroupsPermissions };
        return this.ServiceBase.post(config);
    }
}

angular.module('cloudSync').service('PermissionService', PermissionService);