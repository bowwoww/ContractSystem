using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class Attendance
    {
        [Key]
        [Display(Name = "出席紀錄編號")]
        // GUID 格式的 ID,後續用來生成QR Code
        [StringLength(36, MinimumLength = 36)]
        public string AttendanceID { get; set; } = null!;

        [Required]
        [Display(Name = "合約ID")]
        [StringLength(9)]
        [RegularExpression(@"^[A-Z][0-9]{8}$", ErrorMessage = "ID首碼為分店代號 + 4碼西元年 + 2碼月份 + 2碼流水號.")]
        public string ContractID { get; set; } = null!;

        [Required]
        [Display(Name = "簽到人會員ID")]
        [StringLength(6)]
        [RegularExpression(@"^[A-Z][0-9]{5}$", ErrorMessage = "ID首碼為分店代碼 + 5碼流水號.")]
        public string MemberID { get; set; } = null!;

        [Required]
        [Display(Name = "教練會員ID")]
        [StringLength(6)]
        [RegularExpression(@"^[A-Z][0-9]{5}$", ErrorMessage = "ID首碼為分店代碼 + 5碼流水號.")]
        public string TrainerID { get; set; } = null!;

        [DataType(DataType.DateTime)]
        [Display(Name = "簽到時間")]
        //簽到時間格式精確到秒
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH-mm-ss}", ApplyFormatInEditMode = true)]
        public DateTime AttendanceDate { get; set; } = DateTime.Now;

        public virtual Contract? Contract { get; set; }
        public virtual Member? Member { get; set; }
        public virtual Member? Trainer { get; set; }
    }
}
