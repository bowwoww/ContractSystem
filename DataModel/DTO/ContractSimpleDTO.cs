using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.DTO
{
    public class ContractSimpleDTO
    {
        public string ContractID { get; set; } = null!;
        public string TrainingClassName { get; set; } = null!;
        public string MemberName { get; set; } = null!;
        public int AttendancedClass { get; set; }
        public int TotalClass { get; set; }
    }
}
