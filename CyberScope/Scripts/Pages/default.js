import { FooComponent } from '../core/component.js';

$(document).ready(async () => { 
    const foo = new FooComponent({
        container: 'div#foo' 
    });
    await foo.Init();
    foo.Render();  
     
    $('#someEvent').change(() => {
        foo.Render();  
    });
    $('#someOtherEvent').change(() => {
        foo.Render();
    });
});