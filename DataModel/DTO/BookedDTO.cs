using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.DTO
{
    public class BookedDTO
    {
        public string ContractClassName { get; set; } = null!;

        public string TrainerName { get; set; } = null!;

        public DateTime ContractCreatDate { get; set; }

        public List<DateTime>? ClassDate { get; set; }
    }
}
