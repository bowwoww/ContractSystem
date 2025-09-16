using DataModel.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.DTO
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "帳號(會員ID)")]
        public string MemberID { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        [PasswordValidation]
        [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*()_+=-]+$", ErrorMessage = "密碼只能包含字母、數字和特殊符號 !@#$%^&*()_+=-")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密碼長度必須介於 6 到 20 字元之間.")]
        public string MemberPassword { get; set; } = null!;
    }
}
