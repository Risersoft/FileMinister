/// <reference path="../common/angular.d.ts"/>
'use strict';

class authInterceptorService {
    private _this: authInterceptorService;

    private redirectUrl: string = "";
    private navigateUrl: string = "";
    private $loading: any = $('#loadingDiv');
    //private navigateUrl: string = "http://login.risersoft.com/OAuth/Authorize?response_type=code&client_id=7890ab&redirect_uri=http://localhost:58176/account";

    constructor(public $q: ng.IQService, public $window: ng.IWindowService, public $rootScope: $RootScope) {
        this._this = this
        this.redirectUrl = Settings.RedirectUrl;
        this.navigateUrl = Settings.AuthorityUrl + "/OAuth/Authorize?response_type=code&client_id=" + Settings.ClientId + "&redirect_uri=" + this.redirectUrl;
    }

    public ShowLoader = function () {
        this.$loading.show();
        var loaderCount = parseInt(this.$loading.attr("loaderCount") || "0");
        this.$loading.show().attr("loaderCount", loaderCount + 1);
    }

    public HideLoader = function () {
        var that = this;
        setTimeout(function () {
            var loaderCount = parseInt(that.$loading.attr("loaderCount") || "0");
            if (loaderCount <= 1) {
                that.$loading.hide();
                that.$loading.attr("loaderCount", 0);
            }
            else
                that.$loading.attr("loaderCount", loaderCount - 1);
        }, 300);
    }


    request = (config: any) => {
        config.headers = config.headers || {};
        let account = { accountId: '', accountName: '' };
        try {
            let sacct = this.$rootScope.accountDetails; //localStorage["SelectedAccount"];
            if (sacct) {
                this.$rootScope.isUserAuthentic = true;              
                account = JSON.parse(sacct);
            }
        }
        catch (e) { }

        this.ShowLoader();

        //alert('hello');
        config.headers.UserAccount = '';
        if (account) {
            //var headerUserAccount = btoa(JSON.stringify({ AccountId: account.accountId }));
            var headerUserAccount = btoa(JSON.stringify({ AccountName: account.accountName }));
            config.headers.UserAccount = headerUserAccount
        }
        //config.headers.UserAccount = '';
        if (account) {
            config.headers.AccountName = account.accountName;
        }
        return config;
    }

    responseError = (rejection: any) => {
        this.HideLoader();
        if (rejection.status === 401) {
            var refreshToken = localStorage["refresh_token"];
            var refreshTokenUrl = this.redirectUrl + "/refresh?refreshToken=" + refreshToken;
            this.$window.location.href = refreshTokenUrl;
            //this.$window.location.href = this.navigateUrl;
        } else {
            return this.$q.reject(rejection);
        }
    }

    response = (resp: any) => {
        //console.log("resp:", resp);
        this.HideLoader();
        return resp;
    }

    //responseError(rejection: any): any {
    //    if (rejection.status === 401) {
    //        this.$window.location.href = "http://192.168.5.100:9091/login?return_url=http://localhost:58176/account";
    //    } else {
    //        return this.$q.reject(rejection);
    //    }
    //}

    GotoLogin() {
        this.$window.location.href = this.navigateUrl;
    }

    Logout() {
        //this.$window.location.href = Settings.AuthorityUrl + '/account/logout?ReturnUrl=' + this.$window.location.href;
        this.$window.location.href = '/account/logout';
    }
}

angular.module('cloudSync').service('authInterceptorService', ['$q', '$window', '$rootScope', authInterceptorService]);