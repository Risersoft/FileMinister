/// <reference path="../common/angular.d.ts"/>
'use strict';
var authInterceptorService = /** @class */ (function () {
    //private navigateUrl: string = "http://login.risersoft.com/OAuth/Authorize?response_type=code&client_id=7890ab&redirect_uri=http://localhost:58176/account";
    function authInterceptorService($q, $window, $rootScope) {
        var _this_1 = this;
        this.$q = $q;
        this.$window = $window;
        this.$rootScope = $rootScope;
        this.redirectUrl = "";
        this.navigateUrl = "";
        this.$loading = $('#loadingDiv');
        this.ShowLoader = function () {
            this.$loading.show();
            var loaderCount = parseInt(this.$loading.attr("loaderCount") || "0");
            this.$loading.show().attr("loaderCount", loaderCount + 1);
        };
        this.HideLoader = function () {
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
        };
        this.request = function (config) {
            config.headers = config.headers || {};
            var account = { accountId: '', accountName: '' };
            try {
                var sacct = _this_1.$rootScope.accountDetails; //localStorage["SelectedAccount"];
                if (sacct) {
                    _this_1.$rootScope.isUserAuthentic = true;
                    account = JSON.parse(sacct);
                }
            }
            catch (e) { }
            _this_1.ShowLoader();
            //alert('hello');
            config.headers.UserAccount = '';
            if (account) {
                //var headerUserAccount = btoa(JSON.stringify({ AccountId: account.accountId }));
                var headerUserAccount = btoa(JSON.stringify({ AccountName: account.accountName }));
                config.headers.UserAccount = headerUserAccount;
            }
            //config.headers.UserAccount = '';
            if (account) {
                config.headers.AccountName = account.accountName;
            }
            return config;
        };
        this.responseError = function (rejection) {
            _this_1.HideLoader();
            if (rejection.status === 401) {
                var refreshToken = localStorage["refresh_token"];
                var refreshTokenUrl = _this_1.redirectUrl + "/refresh?refreshToken=" + refreshToken;
                _this_1.$window.location.href = refreshTokenUrl;
                //this.$window.location.href = this.navigateUrl;
            }
            else {
                return _this_1.$q.reject(rejection);
            }
        };
        this.response = function (resp) {
            //console.log("resp:", resp);
            _this_1.HideLoader();
            return resp;
        };
        this._this = this;
        this.redirectUrl = Settings.RedirectUrl;
        this.navigateUrl = Settings.AuthorityUrl + "/OAuth/Authorize?response_type=code&client_id=" + Settings.ClientId + "&redirect_uri=" + this.redirectUrl;
    }
    //responseError(rejection: any): any {
    //    if (rejection.status === 401) {
    //        this.$window.location.href = "http://192.168.5.100:9091/login?return_url=http://localhost:58176/account";
    //    } else {
    //        return this.$q.reject(rejection);
    //    }
    //}
    authInterceptorService.prototype.GotoLogin = function () {
        this.$window.location.href = this.navigateUrl;
    };
    authInterceptorService.prototype.Logout = function () {
        //this.$window.location.href = Settings.AuthorityUrl + '/account/logout?ReturnUrl=' + this.$window.location.href;
        this.$window.location.href = '/account/logout';
    };
    return authInterceptorService;
}());
angular.module('cloudSync').service('authInterceptorService', ['$q', '$window', '$rootScope', authInterceptorService]);
//# sourceMappingURL=authInterceptorService.js.map