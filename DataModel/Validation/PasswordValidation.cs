using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Validation
{
    public class PasswordValidation : ValidationAttribute
    {
        public string? CompareProperty { get; set; }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string password)
            {
                if (password.Length < 6 || password.Length > 20)
                {
                    return new ValidationResult("密碼長度必須介於 6 到 20 字元之間.");
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"^[a-zA-Z0-9!@#$%^&*()_+=-]+$"))
                {
                    return new ValidationResult("密碼只能包含字母、數字和特殊符號 !@#$%^&*()_+=-");
                }

                //比對密碼欄位
                if (!string.IsNullOrEmpty(CompareProperty))
                {
                    var otherProperty = validationContext.ObjectType.GetProperty(CompareProperty);
                    if (otherProperty != null)
                    {
                        var otherValue = otherProperty.GetValue(validationContext.ObjectInstance) as string;
                        if (otherValue != null && password != otherValue)
                        {
                            return new ValidationResult("新密碼與確認密碼不一致");
                        }
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}