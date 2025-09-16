using DataModel.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Service
{
    public class ContractDetailService
    {
        private readonly AppDbContext _context;
        public ContractDetailService(AppDbContext context)
        {
            _context = context;
        }

        // 回傳指定分店指定年月總和合約數量
        public async Task<int> GetMonthlyContractCount(string storeCode,int year, int month)
        {
            string searchString = $"{storeCode}{year}{month.ToString("D2")}";
            // 計算指定分店在特定年月的合約數量
            var count = await _context.Contracts
                .Where(c => c.ContractID.StartsWith(searchString))
                .CountAsync();
            return count;
        }

        //回傳指定合約的已簽到數
        public async Task<int> GetAttendancedClass(string contractId)
        {
            var contract = await _context.Contracts
                            .Include(c => c.Attendances)
                            .FirstOrDefaultAsync(c => c.ContractID == contractId);
            if (contract == null)
            {
                throw new ArgumentException("Invalid contract ID");
            }
            int attendancedClass = contract.Attendances?.Count ?? 0;

            return attendancedClass;
        }

        //回傳指定合約基礎堂數
        public async Task<int> GetClassLength(string contractId)
        {
            int totalClassLength = 0;
            var contract = await _context.Contracts
                            .Include(c => c.TrainingClass)
                            .FirstOrDefaultAsync(c => c.ContractID == contractId);
            if (contract == null)
            {
                throw new ArgumentException("Invalid contract ID");
            }
            if (contract.ClassTypeID == "Z00") //若為他人轉讓課堂
            {
                totalClassLength = await _context.ContractEdits
                            .Where(ce => ce.TransferToMemberID == contract.MemberID && ce.EditType == EditType.轉讓課堂)
                            .SumAsync(ce => ce.TransferClassCount) ?? 0;
            }
            else
                totalClassLength = contract.TrainingClass?.ClassLength ?? 0;
            return totalClassLength;
        }

        //回傳指定合約的總加購堂數
        public async Task<int> GetAddClass(string contractId)
        {
            var contract = await _context.Contracts
                            .Include(c => c.ContractEdits)
                            .FirstOrDefaultAsync(c => c.ContractID == contractId);
            if (contract == null)
            {
                throw new ArgumentException("Invalid contract ID");
            }
            int additionalClass = contract.ContractEdits?
                    .Where(e => e.EditType == EditType.增加課堂數)
                    .Sum(e => e.AddClassCount) ?? 0;
            return additionalClass;
        }

        //回傳指定合約轉讓出去的總課堂數
        public async Task<int> GetTransferClass(string contractId)
        {
            var contract = await _context.Contracts
                            .Include(c => c.ContractEdits)
                            .FirstOrDefaultAsync(c => c.ContractID == contractId);
            if (contract == null)
            {
                throw new ArgumentException("Invalid contract ID");
            }
            int transferedClass = contract.ContractEdits?
                    .Where(e => e.EditType == EditType.轉讓課堂)
                    .Sum(e => e.TransferClassCount) ?? 0;
            return transferedClass;
        }

        public async Task<string> GetFirstContractIdByMemberId(string memberId)
        {
            var contracts = await _context.Contracts
                .Where(c => c.MemberID == memberId)
                .OrderBy(c => c.SignDate)
                .ToListAsync();

            // 檢查每個合約是否有效
            foreach (var contract in contracts)
            {
                if (await IsContractActive(contract.ContractID))
                {
                    return contract.ContractID;
                }
            }

            return "該會員無有效合約";
        }

        public async Task<DateTime?> GetEndDate(string contractId)
        {
            var baseEndDate = await _context.Contracts
                .Where(c => c.ContractID == contractId)
                .Select(c => c.EndDate)
                .FirstOrDefaultAsync();
            var temp = baseEndDate;

            var extendDate = await _context.ContractEdits
                .Where(ce => ce.ContractID == contractId && ce.EditType == EditType.延展結束日期 && ce.NewEndDate != null)
                .OrderByDescending(ce => ce.NewEndDate)
                .Select(ce => ce.NewEndDate)
                .FirstOrDefaultAsync();

            var extendClassCount = await _context.ContractEdits
                .Where(ce => ce.ContractID == contractId && ce.EditType == EditType.增加課堂數)
                .SumAsync(ce => ce.AddClassCount);
            if (extendDate != null)
                temp = extendDate.Value;
            if(extendClassCount > 0)
                temp = temp.AddDays((int)extendClassCount * 5); // 每增加一堂課，合約延長5天

            return temp == baseEndDate? null : temp;
        }

        public async Task<bool> IsContractActive(string contractId)
        {
            // 一次查出所有需要的資料
            var contract = await _context.Contracts
                .Include(c => c.Attendances)
                .Include(c => c.TrainingClass)
                .Include(c => c.ContractEdits)
                .FirstOrDefaultAsync(c => c.ContractID == contractId);

            if (contract == null)
                throw new ArgumentException("Invalid contract ID");


            // 已簽到數
            int attendancedClass = contract.Attendances?.Count ?? 0;

            // 基礎堂數
            int baseClass = contract.ClassTypeID == "Z00"
                ? contract.ContractEdits?
                    .Where(ce => ce.TransferToMemberID == contract.MemberID && ce.EditType == EditType.轉讓課堂)
                    .Sum(ce => ce.TransferClassCount ?? 0) ?? 0
                : contract.TrainingClass?.ClassLength ?? 0;

            // 加購堂數
            int addClass = contract.ContractEdits?
                .Where(e => e.EditType == EditType.增加課堂數)
                .Sum(e => e.AddClassCount ?? 0) ?? 0;

            // 轉讓出去堂數
            int transferClass = contract.ContractEdits?
                .Where(e => e.EditType == EditType.轉讓課堂)
                .Sum(e => e.TransferClassCount ?? 0) ?? 0;

            // 課堂總數
            int totalClass = baseClass + addClass - transferClass;

            // 判斷是否有效
            bool isActive = attendancedClass < totalClass;

            return isActive;
        }

        public async Task<(List<ContractDTO> contracts, int totalCount)> GetContractsPagedAsync(int pageIndex,int pageSize,
                    string memberId,string? trainerId,int graduate)
        {
            var query = _context.Contracts.AsQueryable();

            if (memberId != "0")
                query = query.Where(c => c.MemberID == memberId);

            if(!string.IsNullOrEmpty(trainerId))
                query = query.Where(c => c.TrainerID == trainerId);

            int totalCount = await query.CountAsync();

            var contractDTOs = await query
                .OrderByDescending(c => c.SignDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ContractDTO
                {
                    ContractID = c.ContractID,
                    Signer = c.Signer,
                    Member = new MemberSimpleDTO
                    {
                        MemberID = c.Member.MemberID,
                        MemberName = c.Member.MemberName
                    },
                    Trainer = new MemberSimpleDTO
                    {
                        MemberID = c.Trainer.MemberID,
                        MemberName = c.Trainer.MemberName
                    },
                    Handler = new MemberSimpleDTO
                    {
                        MemberID = c.Handler.MemberID,
                        MemberName = c.Handler.MemberName
                    },
                    SignDate = c.SignDate,
                    EndDate = (
                        // 1. 取最新延展結束日期
                        c.ContractEdits
                            .Where(ce => ce.EditType == EditType.延展結束日期 && ce.NewEndDate != null)
                            .OrderByDescending(ce => ce.NewEndDate)
                            .Select(ce => ce.NewEndDate)
                            .FirstOrDefault()
                        // 2. 沒有延展則用原本 EndDate
                        ?? c.EndDate
                    )
                    // 3. 加購堂數，每堂課延長5天
                    .AddDays(
                        (int)(c.ContractEdits.Where(ce => ce.EditType == EditType.增加課堂數).Sum(ce => ce.AddClassCount ?? 0)) * 5
                    ),
                    ClassType = c.TrainingClass != null ? c.TrainingClass.ClassName : null,
                    PayType = c.PayType != null ? c.PayType.PayTypeName : null,
                    UsedClassCount = c.Attendances != null ? c.Attendances.Count() : 0,
                    MaxClassCount = (c.ClassTypeID == "Z00"
                            ? _context.ContractEdits
                                .Where(ce => ce.TransferToMemberID == c.MemberID && ce.EditType == EditType.轉讓課堂)
                                .Sum(ce => (int?)ce.TransferClassCount ?? 0)
                            : (c.TrainingClass != null ? c.TrainingClass.ClassLength : 0)
                        )
                        + c.ContractEdits.Where(e => e.EditType == EditType.增加課堂數).Sum(e => (int?)e.AddClassCount ?? 0)
                        - c.ContractEdits.Where(e => e.EditType == EditType.轉讓課堂).Sum(e => (int?)e.TransferClassCount ?? 0),
                }
            )
            .ToListAsync();

            if(graduate != 1) //將已上完課的合約過濾掉
            {
                contractDTOs = contractDTOs
                    .Where(c => c.UsedClassCount < c.MaxClassCount)
                    .ToList();
                totalCount = contractDTOs.Count;
            }


            return (contractDTOs, totalCount);
        }

        public async Task<List<ContractSimpleDTO>> GetActiveContractInfosByMemberId(string memberId)
        {
            var contracts = await _context.Contracts
                .Where(c => c.MemberID == memberId)
                .OrderByDescending(c => c.SignDate)
                .Select(c => new
                {
                    c.ContractID,
                    TrainingClassName = c.TrainingClass != null ? c.TrainingClass.ClassName : "",
                    MemberName = c.Member.MemberName,
                    AttendancedClass = c.Attendances.Count(),
                    ClassTypeID = c.ClassTypeID,
                    MemberID = c.MemberID,
                    TrainingClassLength = c.TrainingClass != null ? c.TrainingClass.ClassLength : 0,
                    ContractEdits = c.ContractEdits.ToList()
                })
                .ToListAsync();

            var result = new List<ContractSimpleDTO>();

            foreach (var c in contracts)
            {
                int baseClass = c.ClassTypeID == "Z00"
                    ? c.ContractEdits
                        .Where(ce => ce.TransferToMemberID == c.MemberID && ce.EditType == EditType.轉讓課堂)
                        .Sum(ce => ce.TransferClassCount ?? 0)
                    : c.TrainingClassLength;

                int addClass = c.ContractEdits
                    .Where(e => e.EditType == EditType.增加課堂數)
                    .Sum(e => e.AddClassCount ?? 0);

                int transferClass = c.ContractEdits
                    .Where(e => e.EditType == EditType.轉讓課堂)
                    .Sum(e => e.TransferClassCount ?? 0);

                int totalClass = baseClass + addClass - transferClass;

                if (c.AttendancedClass < totalClass)
                {
                    result.Add(new ContractSimpleDTO
                    {
                        ContractID = c.ContractID,
                        TrainingClassName = c.TrainingClassName,
                        MemberName = c.MemberName,
                        AttendancedClass = c.AttendancedClass,
                        TotalClass = totalClass
                    });
                }
            }

            return result;
        }
    }
}
