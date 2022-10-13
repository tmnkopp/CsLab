import { RequestAsync } from './request.js'; 
 
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