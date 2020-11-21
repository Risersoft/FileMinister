/// <reference path="../common/angular.d.ts"/>
var Settings = /** @class */ (function () {
    function Settings(fileMinisterUrl, authorityUrl, redirectUrl, clientId, clientKey, webWorkSpaceId) {
        Settings.FileMinisterUrl = fileMinisterUrl;
        Settings.AuthorityUrl = authorityUrl;
        Settings.RedirectUrl = redirectUrl;
        Settings.ClientId = clientId;
        Settings.ClientKey = clientKey;
        Settings.WebWorkSpaceId = webWorkSpaceId;
    }
    return Settings;
}());
var OrganizationService = /** @class */ (function () {
    function OrganizationService($http, ServiceBase, $q) {
        this.$http = $http;
        this.ServiceBase = ServiceBase;
        this.$q = $q;
        //private baseUrl: string = "http://192.168.5.100:44301/api";
        /**theis will keep url of the authority service*/
        this.baseUrl = "";
        /**this will keep url of the fileminister service*/
        this.fileMinisterUrl = "";
        /**set here the authority service hosted url*/
        this.baseUrl = Settings.AuthorityUrl + "/api";
        /**set here the fileminister service hosted url*/
        this.fileMinisterUrl = Settings.FileMinisterUrl;
    }
    OrganizationService.prototype.RefreshToken = function (refreshToken) {
        var config = new RequestConfig();
        /**servicerURL is nothing but the authority service url*/
        var url = localStorage.getItem("serviceURL"); //+ "api";
        config.url = url + "/" + 'account/refresh?refreshToken=' + refreshToken;
        config.method = "GET";
        return this.$http(config);
    };
    OrganizationService.prototype.GetUserDetail = function () {
        /**servicerURL is nothing but the authority service url*/
        //var url = localStorage.getItem("servicerURL");//+ "api";
        //config.url = url + "/" + 'user/detail/';
        var config = new RequestConfig();
        config.url = "/api/common/user/";
        config.method = "GET";
        return this.$http(config);
    };
    OrganizationService.prototype.GetUserAccounts = function () {
        var config = new RequestConfig();
        //config.url = this.baseUrl + "/" + 'user/accounts/';
        config.url = '/api/common/accounts';
        config.method = "GET";
        return this.$http(config);
    };
    OrganizationService.prototype.GetAccountName = function () {
        var config = new RequestConfig();
        config.url = '/api/common/account';
        config.method = "GET";
        return this.$http(config);
    };
    OrganizationService.prototype.GetServiceURL = function (accountId) {
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
    };
    OrganizationService.prototype.GetAccountGroupUsers = function (accountId, searchText) {
        var config = new RequestConfig();
        //config.url = 'Organization/GetAccountGroupUsers';
        config.url = 'Common/usersgroups/' + searchText;
        //config.data = { accountId: accountId, searchText: searchText };
        return this.ServiceBase.get(config);
    };
    OrganizationService.prototype.GetUserLogedIn = function () {
        var config = new RequestConfig();
        config.url = '/api/common/check';
        config.method = "GET";
        return this.$http(config);
    };
    return OrganizationService;
}());
angular.module('cloudSync').service('OrganizationService', OrganizationService);
//# sourceMappingURL=OrganizationService.js.map