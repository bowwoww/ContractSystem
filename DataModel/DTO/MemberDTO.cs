using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.DTO
{
    public class MemberDTO
    {
        public string MemberID { get; set; } = null!;
        public string MemberName { get; set; } = null!;
        public string MemberRole { get; set; } = null!;
        public string? MemberTel { get; set; }
        public string? LineID { get; set; }
        public DateTime? MemberBirthday { get; set; }
        public bool MemberGender { get; set; } // true = 男, false = 女
        public string? MemberAddress { get; set; }
        public string? KnowHow { get; set; } // 會員來源
        public int OwnedContractsCount { get; set; } // 擁有的合約數量
        public bool IsActive { get; set; } = true; // 預設為啟用狀態


    }
}
