/// <reference path="../common/angular.d.ts" />
declare class Settings {
    constructor(fileMinisterUrl: any, authorityUrl: any, redirectUrl: any, clientId: any, clientKey: any, webWorkSpaceId: any);
    static FileMinisterUrl: string;
    static AuthorityUrl: string;
    static RedirectUrl: string;
    static ClientId: string;
    static ClientKey: string;
    static WebWorkSpaceId: string;
}
interface IOrganizationService {
    GetUserDetail(): ng.IPromise<any>;
    GetUserAccounts(): ng.IPromise<any>;
    GetServiceURL(accountId: any): ng.IPromise<any>;
    RefreshToken(refreshToken: any): ng.IPromise<any>;
    GetAccountGroupUsers(accountId: string, searchText: string): ng.IPromise<any>;
    GetAccountName(): ng.IPromise<any>;
    GetUserLogedIn(): ng.IPromise<any>;
}
declare class OrganizationService {
    $http: ng.IHttpService;
    ServiceBase: ServiceBase;
    $q: ng.IQService;
    /**theis will keep url of the authority service*/
    private baseUrl;
    /**this will keep url of the fileminister service*/
    private fileMinisterUrl;
    constructor($http: ng.IHttpService, ServiceBase: ServiceBase, $q: ng.IQService);
    RefreshToken(refreshToken: any): ng.IPromise<any>;
    GetUserDetail(): ng.IPromise<any>;
    GetUserAccounts(): ng.IPromise<any>;
    GetAccountName(): ng.IPromise<any>;
    GetServiceURL(accountId: any): ng.IPromise<any>;
    GetAccountGroupUsers(accountId: string, searchText: string): ng.IPromise<any>;
    GetUserLogedIn(): ng.IPromise<any>;
}
