var AccountController = /** @class */ (function () {
    function AccountController(OrganizationService, ngDialog, $scope, $rootScope) {
        this.OrganizationService = OrganizationService;
        this.ngDialog = ngDialog;
        this.$scope = $scope;
        this.$rootScope = $rootScope;
        this.GetUserAccounts();
        this.userAccount = this.$rootScope.selectedAccount && this.$rootScope.selectedAccount.accountId;
    }
    AccountController.prototype.$onInit = function () { };
    AccountController.prototype.GetUserAccounts = function () {
        var _this = this;
        this.OrganizationService.GetUserAccounts()
            .then(function (resp) {
            console.log(resp.data.Data);
            _this.$rootScope.userAccounts = resp.data.Data;
        }, function (resp) {
            console.log(resp);
        });
    };
    AccountController.prototype.SetSelectedAccount = function (account) {
        var accnt = $.grep(this.$rootScope.userAccounts, function (e) { return e.AccountId === account; });
        var name = accnt && accnt.length > 0 ? accnt[0]["AccountName"] : '';
        var acct = { accountId: account, accountName: name };
        var ngDialog = this.ngDialog;
        if (this.$rootScope.selectedAccount && this.$rootScope.selectedAccount.accountId === acct.accountId) {
            ngDialog.close(ngDialog["latestID"], null);
            return;
        }
        var organizationService = this.OrganizationService;
        var rootScope = this.$rootScope;
        this.$rootScope.setSelectedAccount(acct);
        this.OrganizationService.GetServiceURL(acct.accountId).then(function (resp) {
            //if (resp.data[0].serviceUrl && resp.data[0].servicerURL) {
            if (resp.data[0].servicerURL) {
                localStorage.setItem("serviceUrl", resp.data[0].serviceUrl);
                localStorage.setItem("servicerURL", resp.data[0].servicerURL);
                organizationService.GetUserDetail()
                    .then(function (res) {
                    rootScope.setUser(res.data);
                    ngDialog.close(ngDialog["latestID"], acct.accountId);
                }, function (res) {
                    console.log(res);
                });
            }
        });
    };
    AccountController.$ = ['JQuery'];
    return AccountController;
}());
angular.module('cloudSync').controller('AccountController', AccountController);
//# sourceMappingURL=AccountController.js.map