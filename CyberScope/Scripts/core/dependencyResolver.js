export default class DependancyResolver { 
    constructor() {
        this.dependancies = [];
        this.useRegexValueToRequire = true;
    }; 
    resolve() {
        this.dependencies = this.getDependancies(); 
        this.dependencies.forEach(d => {
 
            const row_qd_question_master = $('tr#' + d.questionMaster);
            const row_qd_question = $('tr#' + d.question); 
            let val = this.get_metric_row_value(row_qd_question_master); 

            if (val != null) { 
                // if (d.question.indexOf('3_1_1') > -1) {
                //     console.log(d.question + '\n' + d.questionMaster + '\n' + val);
                // }
                const reg = new RegExp(d.valueToRequire, 'gi'); // gets expression from data-value_torequire attribute
       
                if (reg.test(val)) {
                    $(row_qd_question).show();
                } else {
                    $(row_qd_question).hide();
                    this.resetValue($(row_qd_question).prop('id'));
                }; 
            };
        }); 
        try {
            this.resetValues();
        }
        catch (e) {
            console.log(e);
        };
    };
    resetValue(rowId) {
        let row = $('#' + rowId);
       
        let inputsToReset = $(row).find($('.RadComboBox, input[type="radio"]:checked, input[type="checkbox"]:checked, select, textarea'));
        // if (rowId.indexOf('7_6_2') > -1 || rowId.indexOf('7_6_1') > -1 ) {
            //console.log(inputsToReset);
        // }
        for (let i = 0; i < inputsToReset.length; i++) {
            let id = $(inputsToReset[i]).prop('id');
            id = this.stripScript(id);
            if (inputsToReset[i].className.indexOf('RadComboBox') > -1) {// we have a multi to reset
                let ddl = $telerik.$("#" + id).get(0).control;
                ddl.get_checkedItems().forEach(function (c) {
                    c.set_checked(false);
                });
                return; 
            };
            if (row.prop('id').indexOf('7_6_2') > -1) { }
            if ($(inputsToReset[i]).prop('tagName') == 'SELECT') {
                let id = $(inputsToReset[i]).prop('id');
                id = this.stripScript(id);
                $('#' + id + ' option:selected').removeAttr("selected");
                return;
            }
            var attr = $(inputsToReset[i]).prop('checked');// we have a check box or radio button  to reset 
            attr = this.stripScript(attr);
            let ischeck = false;
            if (typeof attr !== 'undefined' && attr !== false) {
                $(inputsToReset[i]).prop('checked', false);
                ischeck = true;
            };
            attr = $(inputsToReset[i]).attr('checked');
            attr = this.stripScript(attr);
            if (typeof attr !== 'undefined' && attr !== false) { 
                $(inputsToReset[i]).attr('checked', false); 
                ischeck = true; 
            };
            if (ischeck) {
                // if (row.prop('id').indexOf('7_6_') > -1) { }
                return;
            }

            attr = $(inputsToReset[i]).prop('value');
            attr = this.stripScript(attr);
            if (typeof attr !== 'undefined' && attr !== false) {// we have a text to reset
                inputsToReset[i].value = '';
                return;
            };
            attr = $(inputsToReset[i]).val;
            attr = this.stripScript(attr);
            if (typeof attr !== 'undefined' && attr !== false) {// we have a text to reset
                $(inputsToReset[i]).val('');
                return;
            };
        };
    }
    resetValues() {
        let rowids = [];
        $('table tr[id^="r-m-"]').each(function (index, value) { 
            rowids.push($(this).prop('id')); 
        });
        rowids.forEach(id => { 
            if ($('#' + id).is(":visible") == true) {
                return;
            };
            this.resetValue(id);
        });
    };
    getDependancies() {
        let eles = [];
        $('table tr[id^="r-m-"]').each(function () {
            eles.push($(this)); 
        });
        let deps = [];
        eles.forEach(e => { 
            let attrMM = $(e).attr('data-question_master');
            attrMM = this.stripScript(attrMM);
            let attrCV = $(e).attr('data-value_torequire');
            attrCV = this.stripScript(attrCV);
            if (typeof attrMM !== 'undefined' && attrMM !== false) {
                let d = new QuestionDependancy();
                d.question = this.stripScript($(e).attr('id'));
                d.questionMaster = attrMM;
                d.valueToRequire = attrCV;
                deps.push(d);
            };
        }); 
        return deps;
    };
    get_metric_row_value(row_elm) {
        let eles = row_elm.find($('input[type="text"]:not([readonly]), .RadComboBox, .RadCheckBoxList, input[type="radio"]:checked, input[type="checkbox"]:checked, select'));
        //we are in EDIT MODE. Get the value from the control
        //if ($(row_elm).prop('id').indexOf('7_6_1') > -1) {
        //    console.log(eles[0]);
        //}
        for (let i = 0; i < eles.length; i++) {
            let attrValue = $(eles[i]).prop('value');
            attrValue = this.stripScript(attrValue);
            let attrId = $(eles[i]).attr('id');
            attrId = this.stripScript(attrId);

            if (eles[i].className.indexOf('RadComboBox') > -1) {
                let ret = '';
                let ddl = $telerik.$("#" + attrId).get(0).control;
                ddl.get_checkedItems().forEach(function (c) {
                    ret += c._text+','; 
                });
                ret = `,${ret}`;
                return ret; 
            };
            if (eles[i].className.indexOf('RadCheckBoxList') > -1) {
                let ret = '';
                let ddl = $telerik.$("#" + attrId).get(0).control;
                ddl.get_selectedItems().forEach(function (c) {
                    ret += c.get_text() + ','; 
                });
                ret = `,${ret}`;
                return ret;
            };
            //if ($(row_elm).prop('id').indexOf('7_6_1') > -1) {
            //    console.log(attrValue);
            //}
            if (typeof attrValue !== 'undefined' && attrValue !== false) { 
                return attrValue;
            };
        };
         
        // we are in READ MODE. Scrape the value from the html
        eles = row_elm.find($('*[class*="ControlColumn"]'));
        let hasInnerSpanTag = eles.find('span').length > 0
        if (hasInnerSpanTag) {
            eles = eles.find('span');
        }
        if (eles.length > 0) { 
             
            let elemval = eles[0].innerHTML.trim();
         
            if (row_elm[0].hasAttribute('class')) {
                let isMULTI = row_elm.attr('class').indexOf('ct_MULTICHECKBOX') > -1;
                let isPICK = row_elm.attr('class').indexOf('ct_PICK') > -1;
                let isYN = row_elm.attr('class').indexOf('ct_YN') > -1;
                if (isMULTI || isPICK) {
                    let selectedValues = elemval.split('<br>');
                    let defaultValues = elemval.split(' / ');
                    if (defaultValues.length > 1 && selectedValues.length < 2) {
                        return '~no_selection~';
                    }
                    elemval = elemval.split('<br>').join(',');
                    elemval = `,${elemval}`;
                    return elemval;
                }
            }
            //console.log(elemval);
             
            return elemval;
        } else {
            return null;
        } 
    };
    stripScript(str){
        var regScript = /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi;
        if (typeof str == 'undefined' || str === undefined) {
            return str;
        } else {
            try {
                var strNoScript = str.toString().replace(regScript, "");
                return strNoScript;
            }
            catch (e) {
                console.log(e); 
            }; 
            return '';
        }
    } 
}
 
export class QuestionDependancy {
    constructor() {
        this.question = '';
        this.questionMaster = '';
        this.valueToRequire = '';
    }
}
 