import { RequestAsync } from './request.js';
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
        if (typeof this._DataBind !== 'function') { 
            return await RequestAsync(this._request).then(response => {
                this._data = response; 
                return this;
            });
        }  
        return await new Promise(async (resolve) => {  
            await this._DataBind(kwargs);
            resolve(this);
        });
    } 
    async Render(kwargs = { data: this._data}) {   
        this._data = kwargs.data;
        this.onRendering(this, {});
        if (typeof this._Render !== 'function') {
            console.error(this.constructor.name + ' _onRender() undefined exception');
            return;
        }
        return await new Promise(async (resolve) => { 
            await this._Render(kwargs); 
            resolve(this);
        });
    }
}