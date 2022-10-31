import { FooComponent, DataTable } from '../core/component.js';

$(document).ready(async () => {
      
    const foo = new FooComponent({
        container: 'div#foo',
        onDataBinding: (sender, args) => {
            sender.request.parms = { UsageField: sender.re_foo_picklist.val() } 
        } 
    });    
    foo.onChange = (sender, args) => {
        const { source } = args;
        console.log(source);
    }
    await foo.Init(); 
    await foo.DataBind();
    await foo.Render();
    console.log(foo);
    
});