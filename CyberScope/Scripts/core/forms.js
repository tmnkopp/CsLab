﻿import { RequestAsync } from './request.js'; 
import { stripScript } from './utils.js';

/*
const validator = new Validator({ target: ' form ' });
$('#formsubmit').on({
    click: async (e) => {
        await validator.ValidateAsync().then(r => {
            const isValid = validator.isValid;
            $('#err').html(`isValid: ${isValid}<br> ${r.Errors.map(i => i.message).join('<br>')}`);
        });
    }
}); 
 */
export class Validator {
    constructor({ target = 'form' } = {}) {
        this.target = target;
        this.isValid = true;
        this.dataValidations = []; 
        this.request = { resource: `~DBUtils.aspx/CbGet` };
        this.request.handler = 'GetValidations';
        RequestAsync(this.request).then(result => this.dataValidations = result);
    };
    async ValidateAsync() { 
        return await new Promise((resolve) => {
            this._applyValidationsAsync().then(r => {
                resolve(r);
            });
        });
    };
    async _applyValidationsAsync() {
        return await new Promise((resolve) => {
            this.isValid = true;
            this.validationResult = {};
            this.validationResult.Errors = [];
            let inputs = []; 
            $(`${this.target} *[data-validations]`).each((i, o) => {
                inputs.push(o);
            });
            
            inputs.forEach(o => {
                let inputValidations = $(o).attr('data-validations');
                let validationMess = $(o).attr('validation-message'); 
                if (typeof validationMess === 'undefined' || validationMess === false) {
                    validationMess = `Invalid ${stripScript($(o).attr('id'))}`;
                }
               
                let regex = new RegExp(inputValidations, 'gi')
                this.dataValidations.filter(i => regex.test(i.ValidationCode)).forEach(dv => {
                    regex = new RegExp(dv.Expression, 'gi');
                    const value = FormUtils.getValue(o); 
                    if (!regex.test(value)) { 
                        this.validationResult.Errors.push({ id: $(o).attr('id'), message: validationMess }); 
                    }
                });
               
            });
            let ojson = this.validationResult.Errors.map(JSON.stringify); 
            this.validationResult.Errors = [...new Set(ojson)].map(JSON.parse);
            this.validationResult.isValid = this.isValid = this.validationResult.Errors.length < 1;
            resolve(this.validationResult);
        });
    } 
};

export class DataBinder {
    constructor({
        container = 'form'
        , data = {}
        , bindableAttributes = ['data-bind', 'name', 'id']
    } = {}) {
        this.container = container;
        this.data = data;
        this.bindableAttributes = bindableAttributes
    };
    Bind() {
        for (let [k, v] of Object.entries(this.data)) {
            this.bindableAttributes.forEach((attr) => {
                let o = $(`*[${attr}='${stripScript(k)}']`);
                if (typeof o !== 'undefined') {
                    FormUtils.setValue(o, v);
                }
            });
        }
    }
    GetFormVals() {
        let d = {};
        $(`${this.container} *`).each((i, o) => {
            const type = new RegExp(/hidden|button|submit/, 'gi');
            if (type.test($(o).prop('type'))) {
                return;
            }
            const nodeName = new RegExp(/INPUT|TEXTAREA|SELECT|DATALIST/, 'gi');
            if (nodeName.test($(o).prop('nodeName'))) {
                this.bindableAttributes.forEach((attr) => {
                    let bindto = $(o).attr(attr);
                    if (typeof bindto !== 'undefined') {
                        d[bindto] = FormUtils.getValue(o);
                    }
                });
            }
        });
        return d;
    }
}

export class FormUtils {
    static getValue(o) {
        let returnVal = $(o).val();
        if ($(o).attr('type') == 'radio') {
            let name = $(o).attr('name');
            let group = $(`input[name='${stripScript(name)}']:checked`)
            let val = group.val();
            returnVal = (group.length < 1) ? '' : val;
        }
        if ($(o).attr('type') == 'checkbox') { 
            let id = $(o).attr('id'); 
            let isChecked = $(`input[id='${stripScript(id)}']:checked`).length != 0; 
            returnVal = (isChecked) ? $(o).val() : '';
        }
        if ($(o).prop('tagName') == 'SELECT') {
            let def = $(o).find(":selected").attr('default');
            returnVal = (typeof def === 'undefined') ? $(o).val() : '';
        }
        return stripScript(returnVal);
    }
    static setValue(o, val) {
        val = stripScript(val); 
        if ($(o).attr('type') == 'radio' && $(o).length > 1) {
            for (let rad of $(o)) {
                if ($(rad).val() == val) {
                    $(rad).prop('checked', true); return;
                }
            }
        }
        if ($(o).attr('type') == 'checkbox') {
            if ($(o).val() == val) {
                $(o).prop('checked', true);  return;
            }
        }
        if ($(o).prop('tagName') == 'SELECT') {
            $(o).val(val); return;
        }
        $(o).val(val);
    }
}

export const indentQuestions = ({ how={ '[a-z]$': 1 } } = {}) => {

    const span = parseInt($('.SectionHead').attr('colspan'));
    $('.ButtonDiv, .col_error').attr('colspan', span);

    $('tr[id^="r-m-"] > td').css({ 'width': '1%' });
    $('tr[id^="r-m-"] td.Question').css({ 'width': '99%' });
    $('tr[id^="r-m-"]').each((index, element) => {
        for (let [k, v] of Object.entries(how)) {
            if (RegExp(k).test($(element).prop('id'))) { 
                for (var i = 0; i < v; i++) {
                    $('<td class="LabelColumn"></td>').insertBefore($(element).find("td:first-child"));
                }
            };
        }
    });
    $('tr[id^="r-m-"]').each((index, element) => {
        let cols = $(element).find(" > td");
        if (cols.length < span) {
            $(element).find('.Question').attr('colspan', (span + 1) - cols.length);
        }
    });
}