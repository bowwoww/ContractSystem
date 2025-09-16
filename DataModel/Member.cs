using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel.Validation;

namespace DataModel
{
    public partial class Member
    {
        [Required]
        [Display(Name = "會員ID")]
        [StringLength(6)]
        [RegularExpression(@"^[A-Z][0-9]{5}$", ErrorMessage = "ID首碼為分店代碼 + 5碼流水號.")]
        public string MemberID { get; set; } = null!;

        [Required]
        [Display(Name = "會員姓名")]
        [StringLength(50, ErrorMessage = "姓名需介於 1 - 50 字")]
        public string MemberName { get; set; } = null!;

        [Required]
        [Display(Name = "會員密碼")]
        [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*()_+=-]+$", ErrorMessage = "密碼只能包含字母、數字和特殊符號 !@#$%^&*()_+=-")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密碼長度必須介於 6 到 20 字元之間.")]
        //驗證為前端驗證,密碼需透過加密儲存至資料庫
        [PasswordValidation]
        [DataType(DataType.Password)]
        public string MemberPassword { get; set; } = null!;

        [Required]
        [Display(Name = "會員權限")]
        [StringLength(1)]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "權限代碼必須為大寫字母 A-Z.")]
        public string MemberRole { get; set; } = null!;

        [Display(Name = "會員電話")]
        [StringLength(15, ErrorMessage = "電話號碼長度不能超過 15 字元.")]
        //電話號碼必為數字或者減號
        [RegularExpression(@"^[0-9\-]+$", ErrorMessage = "電話號碼必為數字或者減號")]
        public string? MemberTel { get; set; }

        [Display(Name = "Line ID")]
        [StringLength(20, ErrorMessage = "Line ID 長度不能超過 20 字元.")]
        public string? LineID { get; set; }

        [Display(Name = "會員生日")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? MemberBirthday { get; set; }

        [Required]
        [Display(Name = "性別")]
        public bool MemberGender { get; set; } // true = 男, false = 女

        [Display(Name = "會員地址")]
        [StringLength(100, ErrorMessage = "地址長度不能超過 100 字元.")]
        public string? MemberAddress { get; set; }

        [Display(Name = "認識來源")]
        public int? MemberSource { get; set; } // 例如：網路、朋友介紹、廣告等

        [Display(Name = "備註")]
        [StringLength(1000, ErrorMessage = "備註長度不能超過 1000 字元.")]
        public string? MemberRemark { get; set; }

        [Display(Name = "是否啟用")]
        public bool IsActive { get; set; } = true; // 預設為啟用狀態

        public virtual MemberRole? MemberRoleNavigation { get; set; } // 對應 MemberRole 類別
        public virtual ICollection<Contract>? Contracts { get; set; } // 一個會員可以有多個合約

        public virtual List<TrainingDate>? TrainingDates { get; set; } // 一個會員可以有多個預約訓練日期

        public virtual KnowSource? KnowSource { get; set; } // 對應認識來源
    }
}
