import { FooComponent } from '../core/component.js';

$(document).ready(async () => { 
    const foo = new FooComponent({ container: 'div#foo' });

    await foo.Init();
    await foo.DataBind();
    let render = await foo.Render().then(r=>r);
    console.log(render);

    $(window).resize(() => foo.Render());

});