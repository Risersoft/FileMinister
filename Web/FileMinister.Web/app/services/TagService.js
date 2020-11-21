/// <reference path="../common/angular.d.ts"/>
var TagService = /** @class */ (function () {
    function TagService(ServiceBase) {
        this.ServiceBase = ServiceBase;
    }
    TagService.prototype.getByFileId = function (fileId) {
        var config = new RequestConfig();
        config.url = 'tag/file/' + fileId;
        return this.ServiceBase.get(config);
    };
    TagService.prototype.save = function (id, data) {
        var config = new RequestConfig();
        //config.url = 'tag/' + id;
        config.url = 'tag/savetags';
        //config.data = data;
        config.data = { item1: id, item2: data.AddedTags, item3: data.UpdatedTags, item4: data.RemovedTags };
        return this.ServiceBase.post(config);
    };
    return TagService;
}());
angular.module('cloudSync').service('TagService', TagService);
//# sourceMappingURL=TagService.js.map