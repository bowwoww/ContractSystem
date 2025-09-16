using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class Contract
    {
        [Key]
        [StringLength(9)]
        [Required]
        [Display(Name = "合約 ID")]
        [RegularExpression(@"^[A-Z][0-9]{8}$", ErrorMessage = "ID首碼為分店代號 + 4碼西元年 + 2碼月份 + 2碼流水號.")]
        public string ContractID { get; set; } = null!;

        [Required]
        [Display(Name = "簽約人名稱")]
        [StringLength(50)]
        public string Signer { get; set; } = null!;

        [Required]
        [Display(Name = "會員ID")]
        [StringLength(6)]
        [RegularExpression(@"^[A-Z][0-9]{5}$", ErrorMessage = "ID首碼為分店代碼 + 5碼流水號.")]
        public string MemberID { get; set; } = null!;

        [Required]
        [Display(Name = "教練ID")]
        [StringLength(6)]
        [RegularExpression(@"^[A-Z][0-9]{5}$", ErrorMessage = "ID首碼為分店代碼 + 5碼流水號.")]
        public string TrainerID { get; set; } = null!;

        [Required]
        [Display(Name = "經手人員ID")]
        [StringLength(6)]
        [RegularExpression(@"^[A-Z][0-9]{5}$", ErrorMessage = "ID首碼為分店代碼 + 5碼流水號.")]
        public string HandlerID { get; set; } = null!;

        [DataType(DataType.DateTime)]
        [Display(Name = "簽約日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime SignDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "合約結束日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "課程類型")]
        [StringLength(3)]
        public string ClassTypeID { get; set; } = null!;

        [Display(Name = "支付方式")]
        [StringLength(1)]
        public string? PayTypeID { get; set; }

        [Display(Name = "課程")]
        public virtual TrainingClass? TrainingClass { get; set; }
        [Display(Name = "支付方式")]
        public virtual PayType? PayType { get; set; }

        [Display(Name ="會員")]
        public virtual Member? Member { get; set; }
        [Display(Name = "經手人")]
        public virtual Member? Handler { get; set; }
        [Display(Name = "教練")]
        public virtual Member? Trainer { get; set; }
        public virtual ICollection<TrainingDate>? TrainingDates { get; set; }
        public virtual ICollection<Attendance>? Attendances { get; set; }

        public virtual ICollection<ContractEdit>? ContractEdits { get; set; }

    }
}
