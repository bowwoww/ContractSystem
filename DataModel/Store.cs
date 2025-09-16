using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Store
    {
        //分店編號為大寫英文字一碼
        [Key]
        [Display(Name ="分店編號")]
        [StringLength(1)]
        [RegularExpression("^[A-Z]$", ErrorMessage = "分店編號必須為一個大寫英文字母")]

        public string Id { get; set; } = null!;

        [Display(Name ="店家名稱")]
        [StringLength(20)]
        public string? Name { get; set; }

        [Display(Name="店家地址")]
        [StringLength(100, ErrorMessage = "地址長度不能超過 100 字元.")]
        public string? Address { get; set; }
    }
}
