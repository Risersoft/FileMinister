/// <reference path="../common/angular.d.ts" />
declare class TagService implements ITagService {
    ServiceBase: ServiceBase;
    constructor(ServiceBase: ServiceBase);
    getByFileId(fileId: string): ng.IPromise<any>;
    save(id: string, data: any): ng.IPromise<any>;
}
