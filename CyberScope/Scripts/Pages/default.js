import { FooComponent } from '../core/component.js';

$(document).ready(async () => { 
    const foo = new FooComponent({
        container: '.body-content div#foo',
        onDataBinding: (sender, args) => {
            sender.request = {
                resource: `~DBUtils.aspx/GetDataTable`,
                SprocName: 'spPicklists',
                parms: { UsageField: $('#soc').val() }
            }
        }
    });   
    await foo.Init(); 
    await foo.DataBind();
    await foo.Render().then(r => r);
  
    $(window).resize(() => foo.Render());

});