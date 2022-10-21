
 
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
export class PDFExporter {
    constructor({ container = 'form', filename = document.title, wait = 250} = {}  ){
        this.container = container;
        this.filename = filename;
        this.wait = wait;
    }
    static ExportAsync = async ({ container = 'form', filename = document.title, wait = 250 }) => {
        return await new Promise((resolve, reject) => {
            setTimeout(() => {
                kendo.drawing.drawDOM($(container))
                    .then(function (group) {
                        return kendo.drawing.exportPDF(group, {
                            forcePageBreak: ".page-break",
                            paperSize: "auto",
                            margin: { left: "1cm", top: "1cm", right: "1cm", bottom: "1cm" }
                        });
                    })
                    .done(function (data) {
                        kendo.saveAs({
                            dataURI: data,
                            fileName: filename,
                        });
                        resolve(true);
                    });
            }, wait);
        })
    }
    _helper() { 
         // _private function   to be implemented
    }
}
export class Cookie {
    /*
    console.log(Cookie.Get('foo'));
    Cookie.Set('foo', new Date().getTime());
    console.log(Cookie.Get('foo'));
    Cookie.Remove('foo');
     */
    static Set(name, value, days = 365) { 
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    }
    static Get(name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }
    static Remove(name) {
        document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    }
} 