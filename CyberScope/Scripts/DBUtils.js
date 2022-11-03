import { RequestAsync } from './core/request.js';
import { DataTable } from './core/utils.js';
import { CBComponent } from './core/component.js';
 
 
$(document).ready(async () => { 
    const ddl = new ddlPicklist({
        container: 'div#my-container' 
    }); 
    ddl.onChange = async (sender, args) => {
        console.log(sender.val());
    } 
    await ddl.Init();
    await ddl.DataBind();
    await ddl.Render();

    const myComp = new MyComponent({
        container: 'div#my-container'
    });
    await myComp.Init();
    await myComp.DataBind();
    await myComp.Render();
    console.log(myComp);

})




export class MyComponent extends CBComponent {
    constructor(kwargs = {}) {
        super(kwargs);
        this._request = {
            resource: `~DBUtils.aspx/Picklist`, parms: { Description: '%HVA%' }
        }
    }
    async _Init(kwargs = {}) {
        $(`#my-result`).remove();
        $(` 
            <div id='my-result'></div>
        `).insertAfter($(`${this.container}`));
        return this;
    }
    async _Render(kwargs = {}) { 
        let dt = new DataTable({ data: this._data.PickList });
        dt.to_html('#my-result'); 
        return this;
    }
}
window.MyComponent = MyComponent;

export class ddlPicklist extends CBComponent {
    constructor(kwargs = {}) {
        super(kwargs);
        this._request = {
            resource: `~DBUtils.aspx/Picklist`, parms: { Description: '%HVA%'}
        }
    }
    async _Init(kwargs = {}) {
        $(`${this.container}`).html('');
        $(` 
            <select id='ddl-picklist'></select>
        `).insertBefore($(`${this.container}`));
        this.register_element('ddl-picklist', 'ddl-picklist');

        $('#ddl-picklist').change(() => { 
            this.onChange($('#ddl-picklist'), this);
        });
        return this;
    }
    async _Render(kwargs = {}) {
        console.log(this._data);
        return; 
        const dt = new DataTable({ data: this._data });
        const ddlData = dt.distinct('UsageField', ['PK_PickListType', 'UsageField']);
        ddlData.to_array().forEach((i) => {
            $('#ddl-picklist').append(`<option value='${i.UsageField}'>${i.UsageField}</option>`)
        });
        return this;
    }
}
window.ddlPicklist = ddlPicklist;
