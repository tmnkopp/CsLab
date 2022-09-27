

export const stripScript = (str) => {
    var regScript = /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi;
    if (typeof str == 'undefined' || str === undefined) {
        return str;
    } else {
        try {
            var strNoScript = str.toString().replace(regScript, "");
            return strNoScript;
        }
        catch (e) {
            console.log(e);
        };
        return '';
    }
}
