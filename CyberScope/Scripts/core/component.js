import { RequestAsync } from '../core/request.js';

export class DataTable {
    constructor({ data = [{}], primekey = '' }) {
        this._data = data;
        this.columns = Object.keys(data[0]);
        this.primekey = (primekey != '') ? primekey : Object.keys(this._data[0])[0];
        this.raw = this._data;
        this.get = (val) => this._data.find((i) => i[this.primekey] == val);
        this.serialize = JSON.stringify(this._data); 
    }
    distinct(field) {
        let set = this._data.reduce((r, i) => r.add(i[field]), new Set());
        const result = [...set].reduce((r, e) => {
            r.push(this._data.find((i) => (i[field] == e)))
            return r;
        }, []);
        return new DataTable({ data: result, primekey: field });
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
            resolve(new DataTable({ data: result, primekey:this.primekey}));
        });
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
    to_select({ textfield = '', valuefield = '', id = '', selected = ''}) {
        const select = document.createElement('select');
        select.id = (id == '') ? `${this.id}-select-${textfield}` : id;
        this._data.forEach((r) => {
            let option = document.createElement("option");
            option.value = r[valuefield];
            option.text = r[textfield];
            option.selected = (r[valuefield].toUpperCase() == selected.toUpperCase());
            select.appendChild(option);
        });
        return select;
    }
    to_html(container = 'body') {
        const table = document.createElement('table');
        table.style.cssText = 'background-color:#f4f4f4;border-collapse: separate; border-spacing: 3px;';
        let thead = table.createTHead();
        let row = thead.insertRow();
        for (let c of this.columns) {
            let th = document.createElement("th");
            th.style.cssText = 'padding:0em 1rem 1rem 0em ;background-color:#fff;';
            let text = document.createTextNode(c);
            th.appendChild(text);
            row.appendChild(th);
        }
        for (let element of this._data) {
            let row = table.insertRow();
            for (let key in element) {
                let cell = row.insertCell();
                cell.style.cssText = 'background-color:#fff; padding:2px; ';
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
DataTable.prototype.to_array = function () {
    return this._data;
};
 
export class CBComponent {
    constructor({
        container = `form`, 
        request = { },
        onRendering = (sender, args) => { },
        onDataBinding = (sender, args) => { },
        onInitializing = (sender, args) => { }
    }={})
    {
        this.data = {},
        this.request = request,
        this.container = container;   
        this.onRendering = onRendering;
        this.onDataBinding = onDataBinding;
        this.onInitializing = onInitializing;
        this.id = `${this.constructor.name}-${this.container.replace(/[^a-zA-Z_]/g, "-").replace(/^\-/, "")}`.toLowerCase();
    } 
    async Init() {
        this.onInitializing(this, {});
        if (typeof this._Init !== 'function' ) {
            console.error(this.constructor.name + ' _Init() undefined exception');
            return;
        }
        return await new Promise(async (resolve) => {
            await this._Init();
            resolve(this);
        });
    } 
    async DataBind() { 
        this.onDataBinding(this, {});
        if (typeof this._onDataBind !== 'function') {
            return await RequestAsync(this.request).then(response => {
                this.data = response;
                return this;
            });
        }  
        return await new Promise(async (resolve) => {  
            await this._onDataBind();
            resolve(this);
        });
    } 
    async Render() {
        this.onRendering(this, {});
        if (typeof this._onRender !== 'function') {
            console.error(this.constructor.name + ' _onRender() undefined exception');
            return;
        }
        return await new Promise(async (resolve) => {
            await this._onRender();
            resolve(this);
        });
    }
}
export class FooComponent extends CBComponent {
    constructor(options) {
        super(options);
        this.request = {
            resource: `~DBUtils.aspx/GetDataTable`,
            SprocName: 'spPicklists',
            parms: {}
        }  
    }
    async _Init() {
        $(`
            <label for="soc">SOC</label>
            <select id='soc'></select>
        `).insertBefore($(`${this.container}`));

        const data = await RequestAsync({
            resource: `~DBUtils.aspx/GetDataTable`,
            SprocName: 'spPicklists',
            parms: {}
        }).then(response => new DataTable({ data: response }));

        const UsageFields = data.distinct('UsageField');
        const SOC = await UsageFields.select({ UsageField: /.*/ });
      
        UsageFields.to_array().forEach((i) => $('#soc').append(`<option value='${i.UsageField}'>${i.UsageField}</option>`));
    
        $('#soc').change(async () => {
            await this.DataBind();
            await this.Render().then(r => {
                console.log(r)
            });
        });
        return this;
    }  
    async _onRender() { 
        $('#data-container').remove();
        $(`
            <div id="data-container">
                ${JSON.stringify(this.data)}
            </div> 
        `).insertAfter($(`${this.container}`)); 
        return this;
    }
}
window.FooComponent = FooComponent;
 
class CbCanvas extends HTMLElement  {
    constructor() { 
        super();
        this.attachShadow({ mode: "open" });
        const wrapper = document.createElement('div');
        this.shadowRoot.append(wrapper);
    }
    connectedCallback() {
 
    }  
}
customElements.define("cb-canvas", CbCanvas);

