
class AccountController implements angular.IController {

    static $ = ['JQuery'];
    public userAccount: string;
    $onInit() { }

    constructor(public OrganizationService: IOrganizationService, public ngDialog: angular.dialog.IDialogService, public $scope: $Scope, public $rootScope: $RootScope) {
        this.GetUserAccounts();
        this.userAccount = this.$rootScope.selectedAccount && this.$rootScope.selectedAccount.accountId;
    }

    GetUserAccounts() {
        this.OrganizationService.GetUserAccounts()
            .then((resp: any) => {
                console.log(resp.data.Data);
                this.$rootScope.userAccounts = resp.data.Data;
            },
            (resp: any) => {
                console.log(resp);
            });
    }

    SetSelectedAccount(account: any) {
        var accnt = $.grep(this.$rootScope.userAccounts, function (e) { return e.AccountId === account; });
        let name = accnt && accnt.length > 0 ? accnt[0]["AccountName"] : '';
        
        let acct = { accountId: account, accountName: name };
        let ngDialog = this.ngDialog
    
        if (this.$rootScope.selectedAccount && this.$rootScope.selectedAccount.accountId === acct.accountId)
        {
            ngDialog.close(ngDialog["latestID"], null);
            return;
        }

        let organizationService = this.OrganizationService
        let rootScope = this.$rootScope

        this.$rootScope.setSelectedAccount(acct);

        this.OrganizationService.GetServiceURL(acct.accountId).then(function (resp) {
            //if (resp.data[0].serviceUrl && resp.data[0].servicerURL) {
             if (resp.data[0].servicerURL) {
                localStorage.setItem("serviceUrl", resp.data[0].serviceUrl)
                localStorage.setItem("servicerURL", resp.data[0].servicerURL)
                organizationService.GetUserDetail()
                    .then((res: any) => {
                        rootScope.setUser(res.data);
                        ngDialog.close(ngDialog["latestID"], acct.accountId);
                    },
                    (res: any) => {
                        console.log(res);
                    });
            }
        })

    }

}
angular.module('cloudSync').controller('AccountController', AccountController);