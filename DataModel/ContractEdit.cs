using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel.Validation;

namespace DataModel
{
    public enum EditType
    {
        延展結束日期,
        增加課堂數,
        轉讓課堂
    }
    [ContractEditTypeRequiredValidation]
    public partial  class ContractEdit
    {
        [Key]
        [Display(Name = "合約編輯流水號")]
        public int Id { get; set; }

        [StringLength(9)]
        [Required]
        [Display(Name = "合約 ID")]
        [RegularExpression(@"^[A-Z][0-9]{8}$", ErrorMessage = "ID首碼為分店代號 + 4碼西元年 + 2碼月份 + 2碼流水號.")]

        public string ContractID { get; set; } = null!;

        [Required]
        [Display(Name = "經手人員ID")]
        [StringLength(6)]
        [RegularExpression(@"^[A-Z][0-9]{5}$", ErrorMessage = "ID首碼為分店代碼 + 5碼流水號.")]
        public string HandlerID { get; set; } = null!;

        [Required]
        [Display(Name ="修改時間")]
        public DateTime EditDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name ="修改類型")]

        public EditType EditType { get; set; }

        [Display(Name = "修改內容-展延結束時間")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        
        public DateTime? NewEndDate { get; set; }

        [Display(Name = "修改內容-增加課堂數")]
        [Range(0, int.MaxValue, ErrorMessage = "增加課堂數必須為正整數")]
        public int? AddClassCount { get; set; }

        [Display(Name = "修改內容-轉讓課堂對象")]
        [StringLength(6)]
        [RegularExpression(@"^[A-Z][0-9]{5}$", ErrorMessage = "ID首碼為分店代碼 + 5碼流水號.")]

        public string? TransferToMemberID { get; set; }

        [Display(Name = "修改內容-轉讓課堂數")]
        [Range(1, int.MaxValue, ErrorMessage = "轉讓課堂數必須為正整數")]

        public int? TransferClassCount { get; set; }

        [Display(Name = "備註")]
        [StringLength(500, ErrorMessage = "備註長度不能超過 500 字元.")]
        public string? Remarks { get; set; }

        public virtual Contract? Contract { get; set; }
        public virtual Member? Handler { get; set; }

    }
}
