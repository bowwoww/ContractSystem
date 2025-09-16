using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.DTO
{
    public class QrScanModel
    {
        public string ContractID { get; set; } = null!;
        public string MemberID { get; set; } = null!;
        public DateTime SignTime { get; set; }
    }
}
