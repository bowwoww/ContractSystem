using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class OperationLog
    {
        [Key]
        public int Id { get; set; } // 主鍵
        [Required]
        [StringLength(6)]
        public string MemberId { get; set; } = null!;
        [Required]
        [StringLength(100)]
        public string Action { get; set; } = null!;
        [Required]
        public string Device { get; set; } = null!;

        [StringLength(100)]
        public string? Target { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        [StringLength(45)]
        public string IpAddress { get; set; } = null!;
        public virtual Member? Operator { get; set; }
    }
}
