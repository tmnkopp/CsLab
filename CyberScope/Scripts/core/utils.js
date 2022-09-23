
export class Environment {
    static GetBaseUrl() {
        let host = window.location.host;
        if (host.indexOf('dayman') > -1){
            return '/CyberScopeBranch/';
        }
        return '/';
    }
}

export const stripScript = (str) => {
    const regScript = /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi;
    const regHTML = /(<([^>]+)>)/gi;
    let strNoHTML = '';
    if (typeof str !== 'undefined') {
        var strNoScript = str.replace(regScript, "");
        strNoScript = str.replace(regScript, "");
        strNoHTML = strNoScript.replace(regHTML, " ");
    }
    return strNoHTML;
}
