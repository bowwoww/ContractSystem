using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.DTO
{
    public class ContractDTO
    {
        [Required]
        [Display(Name = "合約 ID")]
        [RegularExpression(@"^[A-Z][0-9]{8}$", ErrorMessage = "ID首碼為分店代號 + 4碼西元年 + 2碼月份 + 2碼流水號.")]
        public string ContractID { get; set; } = null!;

        [Required]
        [Display(Name = "簽約人名稱")]
        public string Signer { get; set; } = null!;

        [Required]
        [Display(Name = "會員")]
        public MemberSimpleDTO Member { get; set; } = null!;

        [Required]
        [Display(Name = "教練")]
        public MemberSimpleDTO Trainer { get; set; } = null!;

        [Required]
        [Display(Name = "經手人員")]
        public MemberSimpleDTO Handler { get; set; } = null!;

        [DataType(DataType.DateTime)]
        [Display(Name = "簽約日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime SignDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "合約結束日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name = "課程類型")]
        public string? ClassType { get; set; }

        [Display(Name = "支付方式")]
        public string? PayType { get; set; }

        public int UsedClassCount { get; set; }
        public int MaxClassCount { get; set; }

    }
}
