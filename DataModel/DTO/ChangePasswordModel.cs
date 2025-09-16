using DataModel.Validation;
using System.ComponentModel.DataAnnotations;

namespace DataModel.DTO
{
    public class ChangePasswordModel
    {
        [Required]
        public string MemberID { get; set; } = null!;
        [Required]
        [Display(Name = "舊密碼")]
        public string OldPassword { get; set; } = null!;
        [Required]
        [Display(Name = "新密碼")]
        [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*()_+=-]+$", ErrorMessage = "密碼只能包含字母、數字和特殊符號 !@#$%^&*()_+=-")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密碼長度必須介於 6 到 20 字元之間.")]
        [PasswordValidation]
        public string NewPassword { get; set; } = null!;
        [Required]
        [Display(Name = "確認新密碼")]
        [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*()_+=-]+$", ErrorMessage = "密碼只能包含字母、數字和特殊符號 !@#$%^&*()_+=-")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密碼長度必須介於 6 到 20 字元之間.")]
        [PasswordValidation(CompareProperty = "NewPassword")]
        public string ConfirmPassword { get; set; } = null!;
    }
}