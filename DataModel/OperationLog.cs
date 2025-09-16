using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class OperationLog
    {
        public int Id { get; set; }
        public string MemberId { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string Target { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; } = null!;
    }
}
