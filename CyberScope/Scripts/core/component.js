import { RequestAsync } from '../core/request.js';

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
 
export class CBComponent {
    constructor({
        container = `form`, 
        request = { },
        onChange = (sender, args) => { },
        onRendering = (sender, args) => { },
        onDataBinding = (sender, args) => { },
        onInitializing = (sender, args) => { }
    }={})
    {
        this._data = [{}],
        this._request = request, 
        this.container = container;    
        this.onChange = onChange;
        this.onRendering = onRendering;
        this.onDataBinding = onDataBinding;
        this.onInitializing = onInitializing;
        this._id = `${this.constructor.name}-${this.container.replace(/[^a-zA-Z_]/g, "-").replace(/^\-/, "")}`.toLowerCase();
    }  
    set request(value) { 
        this._request = value; 
    } 
    get request() {
        return this._request;
    }
    register_element(element, id='') {
        id = id.replace(/[^a-zA-Z_]/g, "_");
        Object.defineProperty(this, `re_${id}`, {
            value: $(`#${element}`),
            writable: false
        }); 
    }
    async Init(kwargs = { id: this._id}) { 
        this.onInitializing(this, kwargs);
        return await new Promise(async (resolve) => { 
            if (typeof this._Init === 'function') {
                await this._Init();
            }
            resolve(this);
        }); 
    } 
    async DataBind(kwargs = { request: this._request }) {  
        this._request = kwargs.request;  
        this.onDataBinding(this, kwargs);
        if (typeof this._onDataBind !== 'function') { 
            return await RequestAsync(this._request).then(response => {
                this._data = response;
                return this;
            });
        }  
        return await new Promise(async (resolve) => {  
            await this._onDataBind(kwargs);
            resolve(this);
        });
    } 
    async Render(kwargs = { data: this._data}) {   
        this._data = kwargs.data;
        this.onRendering(this, {});
        if (typeof this._onRender !== 'function') {
            console.error(this.constructor.name + ' _onRender() undefined exception');
            return;
        }
        return await new Promise(async (resolve) => { 
            await this._onRender(kwargs); 
            resolve(this);
        });
    }
}
export class FooComponent extends CBComponent {
    constructor(kwargs = {}) {
        super(kwargs);
        this.request = {
            resource: `~DBUtils.aspx/GetDataTable`,
            SprocName: 'spPicklists',
            parms: {}
        }  
    } 
    async _Init(kwargs = {}) {  
        $(`${this.container}`).html('');
        $(` 
            <select id='foo-picklist'></select>
        `).insertBefore($(`${this.container}`));
        this.register_element('foo-picklist', 'foo-picklist'); 

        const data = await RequestAsync({
            resource: `~DBUtils.aspx/GetDataTable`,
            SprocName: 'spPicklists', parms: {}
        }).then(response => new DataTable({ data: response }));
        
        const UsageFields = data.distinct('UsageField', ['PK_PickListType', 'UsageField']); 
        UsageFields.to_array().forEach((i) => $('#foo-picklist').append(`<option value='${i.UsageField}'>${i.PK_PickListType} ${i.UsageField}</option>`));
    
        $('#foo-picklist').change(async () => { 
            await this.DataBind();
            await this.Render();
            this.onChange(this, { source: $('#foo-picklist')});
        });
        return this;
    }  
    async _onRender(kwargs = {}) { 	  
        $(`#foo-result`).remove();
        $(` 
            <div id='foo-result'></div>
        `).insertAfter($(`${this.container}`));

        let dt = new DataTable({ data: this._data}); 
        dt.to_html();

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

