
export const RequestAsync = async (request) => {
    request.queryString = ParamService.GetParam('CSAM');
    request.referrer = window.location.href;
    const uri = request.resource;
    request = { request: request };
    console.debug(request);  
    return await new Promise((resolve, reject) => { 
        $.ajax({
            url: uri,
            type: "POST",
            data: JSON.stringify(request),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (response) => {
                const _json = JSON.parse(response.d); 
                resolve(_json);
            },
            failure: (response) => reject(response),
            error: (response) => reject(response)
        });
    });
} 
export class ParamService {  
    static GetParam(name) {
        let url = window.location.href;
        name = name.replace(/[\[\]]/g, '\\$&');
        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    }  
}
export class Environment {
    static GetBaseUrl() {
        let host = window.location.host;
        if (host.indexOf('dayman') > -1) {
            return '/CyberScopeBranch/';
        }
        return '/';
    }
}