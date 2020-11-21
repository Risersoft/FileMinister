declare class AccountController implements angular.IController {
    OrganizationService: IOrganizationService;
    ngDialog: angular.dialog.IDialogService;
    $scope: $Scope;
    $rootScope: $RootScope;
    static $: string[];
    userAccount: string;
    $onInit(): void;
    constructor(OrganizationService: IOrganizationService, ngDialog: angular.dialog.IDialogService, $scope: $Scope, $rootScope: $RootScope);
    GetUserAccounts(): void;
    SetSelectedAccount(account: any): void;
}
