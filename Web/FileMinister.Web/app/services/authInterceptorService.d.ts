/// <reference path="../common/angular.d.ts" />
declare class authInterceptorService {
    $q: ng.IQService;
    $window: ng.IWindowService;
    $rootScope: $RootScope;
    private _this;
    private redirectUrl;
    private navigateUrl;
    private $loading;
    constructor($q: ng.IQService, $window: ng.IWindowService, $rootScope: $RootScope);
    ShowLoader: () => void;
    HideLoader: () => void;
    request: (config: any) => any;
    responseError: (rejection: any) => ng.IPromise<any>;
    response: (resp: any) => any;
    GotoLogin(): void;
    Logout(): void;
}
