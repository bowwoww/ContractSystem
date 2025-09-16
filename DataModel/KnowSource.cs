using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class KnowSource
    {
        [Key]
        [Display(Name = "來源編號")]
        // 使用整數作為 ID , 資料庫自動生成

        public int Id { get; set; }

        [Required]
        [Display(Name = "來源名稱")]
        [StringLength(20, ErrorMessage = "來源名稱長度需介於 1 到 20 字元之間")]
        public string SourceName { get; set; } = null!;

        public virtual ICollection<Member> Members { get; set; } = new List<Member>();
    }
}
