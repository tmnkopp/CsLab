import { FooComponent } from '../core/component.js';

$(document).ready(() => { 
    const foo = new FooComponent({
        container: 'div#foo' 
    });  
    foo.Render();  
     
    $('someEvent').change(() => {
        foo.Render();  
    });

});