using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    //訓練日期僅作為行事曆顯示和自動提醒使用,並不作為課程的實際上課日期。
    //簽到記錄的日期時間才是實際上課日期,可用於計算課程的實際上課次數。
    public partial class TrainingDate
    {

        [Key]
        [Display(Name = "訓練日期編號")]
        [Required]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "4碼年 + 4碼月日 + 4碼流水號")]
        public string TrainingDateID { get; set; } = null!;

        [Required]
        [Display(Name = "預期課程日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ClassDate { get; set; }

        [Required]
        [Display(Name = "合約 ID")]
        [StringLength(9)]
        [RegularExpression(@"^[A-Z][0-9]{8}$", ErrorMessage = "ID首碼為分店代號 + 4碼西元年 + 2碼月份 + 2碼流水號.")]
        public string ContractID { get; set; } = null!;
        [Required]
        [Display(Name = "教練ID")]
        [StringLength(6)]
        [RegularExpression(@"^[A-Z][0-9]{5}$", ErrorMessage = "ID首碼為分店代碼 + 5碼流水號.")]
        public string TrainerID { get; set; } = null!;

        public virtual Contract? Contract { get; set; }

        public virtual Member? Member { get; set; }
    }
}
