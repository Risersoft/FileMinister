interface ITagService {
    getByFileId(id: string): ng.IPromise<any>,
    save(id: string, data: any): ng.IPromise<any>
}