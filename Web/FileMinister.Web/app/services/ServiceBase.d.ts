declare class ServiceBase {
    $http: ng.IHttpService;
    $q: ng.IQService;
    constructor($http: ng.IHttpService, $q: ng.IQService);
    get(config: ng.IRequestConfig): ng.IPromise<any>;
    post(config: ng.IRequestConfig): ng.IPromise<any>;
    put(config: ng.IRequestConfig): ng.IPromise<any>;
    delete(config: ng.IRequestConfig): ng.IPromise<any>;
    createRequest(config: ng.IRequestConfig): ng.IHttpPromise<any>;
}
