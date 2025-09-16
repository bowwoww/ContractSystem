using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.DTO
{
    public class NotifityModel
    {
        public string Title { get; set; } = null!;
        public string MessageUpper { get; set; } = null!;

        public string MessageBottom { get; set; } = null!;
        
        public DateTime EstimateTime { get; set; }

        public int NotifyValue { get; set; }
    }
}
