using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class PayType
    {

        [Key]
        [StringLength(1)]
        [Display(Name ="支付方式編號")]
        public string PayTypeID { get; set; } = null!;

        [Required]
        [Display(Name = "支付方式")]
        [StringLength(20,ErrorMessage ="長度需介於 1 到 20 之間")]
        public string PayTypeName { get; set; } = null!;

        public virtual ICollection<Contract>? Contracts { get; set; }

    }
}
