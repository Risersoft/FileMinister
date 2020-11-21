var ServiceBase = /** @class */ (function () {
    //private baseUrl: string = "http://localhost:50648/api";
    function ServiceBase($http, $q) {
        this.$http = $http;
        this.$q = $q;
    }
    ServiceBase.prototype.get = function (config) {
        config.method = "GET";
        var d = this.createRequest(config);
        return d;
    };
    ServiceBase.prototype.post = function (config) {
        config.method = "POST";
        var d = this.createRequest(config);
        return d;
    };
    ServiceBase.prototype.put = function (config) {
        config.method = "PUT";
        var d = this.createRequest(config);
        return d;
    };
    ServiceBase.prototype.delete = function (config) {
        config.method = "DELETE";
        var d = this.createRequest(config);
        return d;
    };
    ServiceBase.prototype.createRequest = function (config) {
        //var url = "/api"; //localStorage.getItem("serviceUrl") +
        config.url = "/api/" + config.url;
        return this.$http(config);
    };
    return ServiceBase;
}());
angular.module('cloudSync').service('ServiceBase', ServiceBase);
//# sourceMappingURL=ServiceBase.js.map