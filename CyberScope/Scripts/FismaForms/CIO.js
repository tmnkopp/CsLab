import DependancyResolver, { QuestionDependancy } from '../core/dependencyResolver.js';

$(document).ready(function () {
     
    // return; // uncomment 'return;' here to disable all client dependancy checking
    const resolver = new DependancyResolver();
    resolver.resolve();  
    $('table tr[id^="r-m-"]').each(function () {
        let inputs = $(this).find($('.RadComboBox'));
        for (let i = 0; i < inputs.length; i++) {
            let id = $(inputs[i]).prop('id');
            $find(id).add_itemChecked(function (sender, args) {
                resolver.resolve();
            })
            $find(id).add_checkAllChecked(function (sender, args) {
                resolver.resolve();
            })
        }; 
        inputs = $(this).find($('input:not([type=hidden]), select, textarea[class*=qid]'));
        for (let i = 0; i < inputs.length; i++) {
            $(inputs[i]).on({
                change: function (e) { 
                    if (e.originalEvent.isTrusted) {
                        resolver.resolve(); 
                    };
                }
            });
        }; 
    });  
}); 
