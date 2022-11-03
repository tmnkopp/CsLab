export class DataTable {
    constructor({ data = [{}], index = '' }) {
        this._data = data; 
        this._index = (index != '') ? index : Object.keys(this._data[0])[0]; 
    }
    get serialize() {
        return JSON.stringify(this._data);
    } 
    get index() {
        return this._index;
    }
    set index(field) {
        this._index = field;
    } 
    get columns() {
        return Object.keys(this._data[0]);
    }  
    async select(query) {
        return await new Promise((resolve) => {
            const result = this._data.filter((i) => {
                let found = false;
                for (let [key, value] of Object.entries(query)) {
                    found = new RegExp(value).test(i[key]);
                }
                return found;
            });
            resolve(new DataTable({ data: result, index: this.index }));
        });
    }
    distinct(field, keepcols = this.columns) {
        let keep = new Set(keepcols);  
        let set = this._data.reduce((r, i) => r.add(i[field]), new Set());
        const result = [...set].reduce((r, e) => {
            let row = this._data.find((i) => (i[field] == e));
            Object.keys(row).forEach((k) => {
                if (!keep.has(k)) delete row[k];
            })
            r.push(row);
            return r;
        }, []);
        return new DataTable({ data: result, index: field });
    } 
    orderby(by) {
        const field = Object.keys(by)[0]; 
        const dir = Object.values(by)[0];
        let fn = (a,b) => {};
        if (typeof this._data[0][field] === 'number') {
            fn = (a, b) => a[field] - b[field];
            if (dir.indexOf('d') == 0) {
                fn=(a, b) => b[field] - a[field]; 
            }
        } else {
            fn = (a, b) => a[field].localeCompare(b[field]);
            if (dir.indexOf('d') == 0) {
                fn = (a, b) => -1 * a[field].localeCompare(b[field]); 
            }
        }
        this._data.sort(fn);
    }   
    to_array() {
        return this._data;  
    } 
    to_html(container = 'body') {
        $(`${container} .dt_to_html`).remove();
        const table = document.createElement('table');
        table.setAttribute('class', 'dt_to_html');
        table.style.cssText = 'border-collapse: separate;border-spacing: 8px;';
        let thead = table.createTHead();
        let row = thead.insertRow();
        for (let c of this.columns) {
            let th = document.createElement("th"); 
            let text = document.createTextNode(c);
            th.appendChild(text);
            row.appendChild(th);
        }
        for (let element of this._data) {
            let row = table.insertRow();
            for (let key in element) {
                let cell = row.insertCell(); 
                let text = document.createTextNode(element[key]);
                cell.appendChild(text);
            }
        }
        $(container).append(table);
    }  
}
DataTable.prototype.to_string = function () {
    return JSON.stringify(this._data);
}; 
DataTable.prototype.drop_columns = function (columns = []) {
    columns = new Set(columns); 
    const result = this._data.reduce((r, row) => {
        Object.keys(row).forEach((k) => {
            if (columns.has(k)) delete row[k];
        }); 
        r.push(row);
        return r;
    }, []);
    console.log(result);
    this._data = result;
};
 
export const stripScript = (str) => {
    var regScript = /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi;
    if (typeof str == 'undefined' || str === undefined) {
        return str;
    }
    try {
        str = str.toString().replace(regScript, "");
    }
    catch (e) {
        console.log(e);
        return '';
    } 
    return str;
}
export const decodeHTML = input => {
    const textarea = document.createElement("textarea");
    textarea.innerHTML = input; 
    const safeValue = stripScript(textarea.value);
    return safeValue;
}
export class PDFExporter {
    constructor({ container = 'form', filename = document.title, wait = 250, options = {}} = {}  ){
        this.container = container;
        this.filename = filename;
        this.wait = wait;
        this.options = options;
    }
    static ExportAsync = async ({
        container = 'form',
        filename = document.title,
        wait = 250,
        options = {
            forcePageBreak: ".page-break",
            paperSize: "auto",
            margin: { left: "1cm", top: "1cm", right: "1cm", bottom: "1cm" }
        }
    }) => {
        return await new Promise((resolve, reject) => {
            setTimeout(() => {
                kendo.drawing.drawDOM($(container))
                    .then(function (group) {
                        return kendo.drawing.exportPDF(group, options);
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