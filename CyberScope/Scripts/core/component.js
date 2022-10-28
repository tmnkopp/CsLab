import { RequestAsync } from '../core/request.js';

export class DataTable {
    constructor({ data = [{}], primekey = '' }) {
        this._data = data;
        this.columns = Object.keys(data[0]);
        this.primekey = (primekey != '') ? primekey : Object.keys(this._data[0])[0];
        this.raw = this._data
        this.get = (val) => this._data.find((i) => i[this.primekey] == val),
        this.serialize = JSON.stringify(this._data) 
    }
    distinct(field) {
        let set = this._data.reduce((r, i) => r.add(i[field]), new Set());
        return [...set].reduce((r, e) => {
            r.push(this._data.find((i) => (i[field] == e)))
            return r;
        }, [])
    }
    async select (query) {
        return await new Promise((resolve) => { 
            const result = this._data.filter((i) => {
                let found = false;  
                for (let [key, value] of Object.entries(query)) { 
                    found = new RegExp(value).test(i[key]);
                } 
                return found;
            });
            resolve(result);
        });
    }
}
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
    }
    async _Init() {
        $(`
            <div id='${this.id}-params' class="row params">
                <label for="soc">SOC</label>
                <select id="soc"></select>
            </div> 
        `).insertBefore($(`${this.container}`));

        const data = await RequestAsync({
            resource: `~DBUtils.aspx/GetDataTable`,
            SprocName: 'spPicklists',
            parms: {}
        }).then(response => new DataTable({ data: response }));

        let sel = await data.select({ UsageField: /^SOC2$/ })
        console.log(data.distinct('UsageField') );
       
        data.distinct('UsageField').forEach((r) => {
            $('#soc').append(`<option value="${r.UsageField}">${r.PK_PickList}</option>}`)
        });

        $('#soc').change(async () => {
            await this.DataBind();
            await this.Render();
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

