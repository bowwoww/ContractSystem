using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class TrainingClass
    {
        [Key]
        [StringLength(3)]
        [Required]
        [Display(Name = "課程 ID")]
        public string ClassTypeID { get; set; } = null!;

        [Required]
        [Display(Name = "課程名稱")]
        [StringLength(50, ErrorMessage ="名稱需介於 1 - 50 字")]
        public string ClassName { get; set; } = null!;

        [Required]
        [Display(Name = "課程長度")]
        [Range(0, 360, ErrorMessage = "課程長度必須在 0 到 360 堂之間.")]
        public int ClassLength { get; set; }

        [Display(Name = "課程介紹")]
        [StringLength(200, ErrorMessage = "介紹長度必須介於 1 到 200之間")]
        public string? ClassDescription { get; set; } = null!;

        public virtual List<Contract>? Contracts { get; set; }
    }
}
