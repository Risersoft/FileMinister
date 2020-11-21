/// <reference path="../common/angular.d.ts"/>
class Settings {
    constructor(fileMinisterUrl, authorityUrl, redirectUrl, clientId, clientKey, webWorkSpaceId) {
        Settings.FileMinisterUrl = fileMinisterUrl;
        Settings.AuthorityUrl = authorityUrl;
        Settings.RedirectUrl = redirectUrl;
        Settings.ClientId = clientId;
        Settings.ClientKey = clientKey;
        Settings.WebWorkSpaceId = webWorkSpaceId;
    }

    public static FileMinisterUrl: string;
    public static AuthorityUrl: string;
    public static RedirectUrl: string;
    public static ClientId: string;
    public static ClientKey: string;
    public static WebWorkSpaceId: string;
}

interface IOrganizationService {
    GetUserDetail(): ng.IPromise<any>,
    GetUserAccounts(): ng.IPromise<any>,
    GetServiceURL(accountId): ng.IPromise<any>,
    RefreshToken(refreshToken): ng.IPromise<any>,
    GetAccountGroupUsers(accountId: string, searchText: string): ng.IPromise<any>
    GetAccountName(): ng.IPromise<any>
    GetUserLogedIn():ng.IPromise<any>
}

class OrganizationService {

    //private baseUrl: string = "http://192.168.5.100:44301/api";

    /**theis will keep url of the authority service*/
    private baseUrl: string = "";

    /**this will keep url of the fileminister service*/
    private fileMinisterUrl: string = "";

    constructor(public $http: ng.IHttpService, public ServiceBase: ServiceBase, public $q: ng.IQService) {
        /**set here the authority service hosted url*/
        this.baseUrl = Settings.AuthorityUrl + "/api";

        /**set here the fileminister service hosted url*/
        this.fileMinisterUrl = Settings.FileMinisterUrl;
    }

    public RefreshToken(refreshToken): ng.IPromise<any> {
        var config = new RequestConfig();

        /**servicerURL is nothing but the authority service url*/
        var url = localStorage.getItem("serviceURL");//+ "api";
        config.url = url + "/" + 'account/refresh?refreshToken=' + refreshToken;
        config.method = "GET";

        return this.$http(config);
    }

    public GetUserDetail(): ng.IPromise<any> {
        /**servicerURL is nothing but the authority service url*/
        //var url = localStorage.getItem("servicerURL");//+ "api";
        //config.url = url + "/" + 'user/detail/';

        var config = new RequestConfig();
        config.url = "/api/common/user/";
        config.method = "GET";

        return this.$http(config);
    }

    public GetUserAccounts(): ng.IPromise<any> {
        var config = new RequestConfig();
        //config.url = this.baseUrl + "/" + 'user/accounts/';
        config.url = '/api/common/accounts';
        config.method = "GET";

        return this.$http(config);
    }

    public GetAccountName(): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = '/api/common/account';
        config.method = "GET";

        return this.$http(config);
    }

    public GetServiceURL(accountId): ng.IPromise<any> {
        //var config = new RequestConfig();
        //config.url = this.baseUrl + "/" + 'Account/' + accountId +'/uri?modulename=cloud%20sync';
        //config.method = "GET";
        //return this.$http(config);

        return this.$q.resolve({
            data: [{
                serviceUrl: this.fileMinisterUrl,
                servicerURL: this.baseUrl
            }]
        });
    }

    public GetAccountGroupUsers(accountId: string, searchText: string): ng.IPromise<any> {
        var config = new RequestConfig();
        //config.url = 'Organization/GetAccountGroupUsers';
        config.url = 'Common/usersgroups/' + searchText;
        //config.data = { accountId: accountId, searchText: searchText };
        return this.ServiceBase.get(config);
    }

    public GetUserLogedIn(): ng.IPromise<any>
    {
        var config = new RequestConfig();
        config.url = '/api/common/check';
        config.method = "GET";
        return this.$http(config);
    }

}
angular.module('cloudSync').service('OrganizationService', OrganizationService);
