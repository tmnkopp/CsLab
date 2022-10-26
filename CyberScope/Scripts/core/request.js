
import { stripScript } from './utils.js';
export const RequestAsync = async (request) => {
    request.queryString = ParamService.GetParam('CSAM');
    request.referrer = window.location.href;
    let uri = request.resource;
    uri = uri.replace('~', Environment.GetBaseUrl());
    request = { request: request };
    request = JSON.stringify(request);
    request = stripScript(request);
    return await new Promise((resolve, reject) => { 
        $.ajax({
            url: uri,
            type: "POST",
            data: request,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (response) => {
                let _json = response.d;
                _json = JSON.parse(_json);
                _json = JSON.stringify(_json);
                _json = stripScript(_json);
                _json = JSON.parse(_json);
                console.debug({ request: JSON.parse(request), response: _json });
                resolve(_json);
            },
            failure: ((response) => {
                console.error(response);
                reject(stripScript(response));
            }),
            error: ((response) => {
                console.error(response);
                reject(stripScript(response));
            })
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