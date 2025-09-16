using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class MemberRole
    {
        [Key]
        [StringLength(1)]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "權限代碼必須為大寫字母 A-Z.")]
        public string MemberRoleID { get; set; } = null!;

        [Required]
        [Display(Name = "權限名稱")]
        [StringLength(20, ErrorMessage = "權限名稱長度需介於 1 到 20 字元之間")]
        public string MemberRoleName { get; set; } = null!;

        public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    }
}
