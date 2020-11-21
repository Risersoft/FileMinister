interface IPermissionService {
    getUsersAndGroupsByFileId(fileId: string): ng.IPromise<any>;
    getPermissionByFileAndUser(fileId: string, userId: string): ng.IPromise<any>;
    getPermissionByFileAndGroup(fileId: string, userId: string): ng.IPromise<any>;
    savePermissionByFileAndGroup(fileId: any, data: any): ng.IPromise<any>;
    getEditPermission(fileId: string): ng.IPromise<any>;
    getShareAdminPermission(fileId: string): ng.IPromise<any>;
    getEditTagPermission(fileId: string): ng.IPromise<any>;
    getDeletedFilesPermission(): ng.IPromise<any>;
    isAccountAdmin(): ng.IPromise<any>;
}
