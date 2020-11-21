declare class RequestConfig implements ng.IRequestConfig {
    method: string;
    url: string;
    params: any;
    headers: any;
    cache: any;
    withCredentials: boolean;
    data: any;
    transformRequest: any;
    transformResponse: any;
    timeout: any;
    useXDomain: boolean;
}
