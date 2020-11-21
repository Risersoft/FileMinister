/// <reference path="../common/angular.d.ts"/>
class TagService implements ITagService {
    constructor(public ServiceBase: ServiceBase) {
    }

    public getByFileId(fileId: string): ng.IPromise<any> {
        var config = new RequestConfig();
        config.url = 'tag/file/' + fileId;
        return this.ServiceBase.get(config);
    }

    public save(id: string, data: any): ng.IPromise<any> {
        var config = new RequestConfig();
        //config.url = 'tag/' + id;
        config.url = 'tag/savetags';
        //config.data = data;
        config.data = { item1: id, item2: data.AddedTags, item3: data.UpdatedTags, item4: data.RemovedTags };
        return this.ServiceBase.post(config);
    }
}
angular.module('cloudSync').service('TagService', TagService);