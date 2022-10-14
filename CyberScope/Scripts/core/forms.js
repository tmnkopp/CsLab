import { RequestAsync } from './request.js'; 

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
        this.request = { resource: `~DBUtils.aspx/GetValidations` };
        RequestAsync(this.request).then(result => this.dataValidations = result);
    };
    async ValidateAsync() {
        return await new Promise((resolve) => {
            this.applyValidationsAsync().then(r => {
                resolve(r);
            });
        });
    };
    async applyValidationsAsync() {
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
                    validationMess = `Invalid ${$(o).attr('id')}`;
                } 
                let regex = new RegExp(inputValidations, 'gi')
                this.dataValidations.filter(i => regex.test(i.ValidationCode)).forEach(dv => {
                    regex = new RegExp(dv.Expression, 'gi');
                    const value = this.getValue(o); 
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
    };
    getValue(o) {
        if ($(o).attr('type') == 'radio') {
            let name = $(o).attr('name');
            let group = $(`input[name='${name}']:checked`)
            let val = group.val();
            return (group.length < 1) ? '' : val;
        }
        if ($(o).prop('tagName') == 'SELECT') {
            let def = $(o).find(":selected").attr('default'); 
            return (typeof def === 'undefined') ? $(o).val() : '';
        }
        return $(o).val();
    }
};

export class DataBinder {
    constructor({ target = 'form', data = {} } = {}) {
        this.target = target;
        this.data = data;
    };
    Bind() {
        for (let [k, v] of Object.entries(this.data)) {
            let o = $(`*[name='${k}']`);
            if (typeof o !== 'undefined') {
                this.setValue(o, v);
            }
        }
    }
    GetForm() {
        for (let [k, v] of Object.entries(this.data)) {
            let o = $(`*[name='${k}']`);
            if (typeof o !== 'undefined') {
                this.data[k] = this.getValue(o);
            }
        }
        return this.data;
    }
    setValue(o, val) {
        if ($(o).attr('type') == 'radio' && $(o).length > 1) {
            for (let rad of $(o)) {
                if ($(rad).val() == val) {
                    $(rad).prop('checked', true);
                    return;
                }
            }
        }
        if ($(o).attr('type') == 'checkbox') {
            if ($(o).val() == val) {
                $(o).prop('checked', true);
                return;
            }
        }
        if ($(o).prop('tagName') == 'SELECT') {
            $(o).val(val);
            return;
        }
        $(o).val(val);
    }
    getValue(o) {
        if ($(o).attr('type') == 'radio') {
            let name = $(o).attr('name');
            let group = $(`input[name='${name}']:checked`)
            let val = group.val();
            return (group.length < 1) ? '' : val;
        }
        if ($(o).prop('tagName') == 'SELECT') {
            let def = $(o).find(":selected").attr('default');
            return (typeof def === 'undefined') ? $(o).val() : '';
        }
        return $(o).val();
    }
}