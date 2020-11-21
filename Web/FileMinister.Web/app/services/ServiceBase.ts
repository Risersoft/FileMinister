class ServiceBase {
    //private baseUrl: string = "http://localhost:50648/api";

    constructor(public $http: ng.IHttpService, public $q: ng.IQService) {
    }

    public get(config: ng.IRequestConfig): ng.IPromise<any> {
        config.method = "GET";
        var d = this.createRequest(config)
        return d;
    }

    public post(config: ng.IRequestConfig): ng.IPromise<any> {
        config.method = "POST";
        var d = this.createRequest(config)
        return d;
    }

    public put(config: ng.IRequestConfig): ng.IPromise<any> {
        config.method = "PUT";
        var d = this.createRequest(config)
        return d;
    }

    public delete(config: ng.IRequestConfig): ng.IPromise<any> {
        config.method = "DELETE";
        var d = this.createRequest(config)
        return d;
    }

    createRequest(config: ng.IRequestConfig): ng.IHttpPromise<any>{
        //var url = "/api"; //localStorage.getItem("serviceUrl") +
        config.url = "/api/" + config.url;        
        return this.$http(config);
    }
}
angular.module('cloudSync').service('ServiceBase', ServiceBase);