import { RequestAsync } from '../core/request.js'; 
class CBComponent {
    constructor({
        container = `form` 
    } = {}) {  
        this.container = container; 
        this._InitComplete = false; 
        this.id = this.container.replace(/[^a-zA-Z]/g, "");
    }
    async Init() { 
        if (typeof this._onInit === 'function' && !this._InitComplete ) {
            await this._onInit();  
        }  
        this._InitComplete = true;
    } 
    async Render() { 
        await this.Init();
        if (typeof this._onRender === 'function') {
            await this._onRender();
        } else { console.error(this.constructor.name + ' Render() undefined exception') }   
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

  

        data = data.reduce((r, i) => r.add(i.UsageField), new Set()); 
        data.forEach((r) => $('#soc').append(`<option value="${r}">${r}</option> `)); 
        $('#soc').change(() => this.Render());
    }
    async _onRenders() {   
        let data = await RequestAsync({
            resource: `~DBUtils.aspx/GetDataTable`,
            SprocName: 'spPicklists',
            parms: { UsageField: $('#soc').val() || '' }
        }).then(response => response); 
        $(`
            <div class="row">
                ${JSON.stringify(data)}
            </div> 
        `).insertAfter($(`${this.container}`)); 
    }
}
window.FooComponent = FooComponent;