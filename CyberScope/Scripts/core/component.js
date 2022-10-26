import { RequestAsync } from '../core/request.js'; 
class CBComponent {
    constructor({
        container = `form` 
    } = {}) {  
        this.container = container;  
        this.id = this.container.replace(/[^a-zA-Z]/g, ""); 
    }
    async Init() { 
        if (typeof this._onInit !== 'function' ) {
            console.error(this.constructor.name + ' _onInit() undefined exception');
            return;
        }
        return await new Promise(async (resolve) => {
            const result = await this._onInit();
            resolve(result);
        });
    } 
    async Render() {
        if (typeof this._onRender !== 'function') {
            console.error(this.constructor.name + ' _onRender undefined exception');
            return;
        }
        return await new Promise(async (resolve) => {
            const result = await this._onRender();
            resolve(result);
        });
    }
}
export class FooComponent extends CBComponent {
    constructor(options) { 
        super(options); 
    }
    async _onInit() {  
        $(`
            <div id='${this.id}-params' class="row params">
                <label for="soc">SOC</label>
                <select id="soc"></select>
            </div>

        `).insertBefore($(`${this.container}`));

        let data = await RequestAsync({
            resource: `~DBUtils.aspx/GetDataTable`,
            SprocName: 'spPicklists',
            parms: { }
        }).then(response => response);

        data = data.reduce((r, i) => r.add(i.UsageField), new Set());
    
        data.forEach((r) => $('#soc').append(`<option value="${r}">${r}</option> `)); 
        $('#soc').change(() => this.Render());
    }
    async _onRender() {   
        let data = await RequestAsync({
            resource: `~DBUtils.aspx/GetDataTable`,
            SprocName: 'spPicklists',
            parms: { UsageField: $('#soc').val() || '' }
        }).then(response => response);
        
        $(`
            <div>
                ${JSON.stringify(data)}
            </div> 
        `).insertAfter($(`${this.container}`)); 
    }
}
window.FooComponent = FooComponent;