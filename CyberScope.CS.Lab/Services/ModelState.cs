using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CyberScope.CS.Lab 
{
    public class CSModelStateError
    {
        public string Message { get; set; }
        public object Value { get; set; }
        public PropertyInfo Property { get; set; }
        public ValidationAttribute Validation { get; set; }
    }
    public static class CSModelState
    {
        public static bool IsValid => ModelStateErrors.Count == 0;
        public static List<CSModelStateError> ModelStateErrors { get; set; } 
        public static List<string> Errors => (from e in ModelStateErrors select e.Message).ToList();  
        public static void Validate(object Model){
            ModelStateErrors = new List<CSModelStateError>();
            foreach (PropertyInfo pi in Model.GetType().GetProperties())
            { 
                var validations = pi.GetCustomAttributes<ValidationAttribute>(true);
                foreach (ValidationAttribute vatt in validations)
                {
                    var val = pi.GetValue(Model);
                    var valid = vatt.IsValid(val);
                    if (!valid ){
                        ModelStateErrors.Add(new CSModelStateError{ 
                            Message = vatt.ErrorMessage,
                            Value = val,
                            Property = pi,
                            Validation= vatt
                        });
                    }     
                }
            } 
        } 
    }
}
