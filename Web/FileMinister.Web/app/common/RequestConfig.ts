class RequestConfig implements ng.IRequestConfig {
    method: string;
    url: string;
    params: any;

    // XXX it has it's own structure...  perhaps we should define it in the future
    headers: any;

    cache: any;
    withCredentials: boolean;

    // These accept multiple types, so let's define them as any
    data: any;
    transformRequest: any;
    transformResponse: any;
    timeout: any; // number | promise
    useXDomain: boolean;
}