using DataModel.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Service
{
    public class MemberDetailService
    {
        private readonly AppDbContext _context;

        public MemberDetailService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetMemberNameByIdAsync(string memberId)
        {
            var member = await _context.Members
                .Where(m => m.MemberID == memberId)
                .Select(m => m.MemberName)
                .FirstOrDefaultAsync();
            return member ?? string.Empty;
        }

        public async Task<List<MemberSimpleDTO>> GetMembersByRoleAsync(string role)
        {
            return await _context.Members
                .Where(m => m.MemberRole == role)
                .Select(m => new MemberSimpleDTO
                {
                    MemberID = m.MemberID,
                    MemberName = m.MemberName
                })
                .ToListAsync();
        }

        public async Task<(List<MemberDTO> members, int totalCount)> GetMembersListAsync(int pageIndex, int pageSize,
            string role, int isActive, string sortBy, int order)
        {
            var query = _context.Members.AsQueryable();
            // 篩選 權限 包含A、B、C (空字串為全部)
            if (!string.IsNullOrEmpty(role))
                query = query.Where(m => role.Contains(m.MemberRole));
            // 篩選 是否啟用 (-1 全部, 0 停用, 1 啟用)
            if (isActive != -1)
                query = query.Where(m => m.IsActive == (isActive == 1));

            // 排序
            query = (sortBy, order) switch
            {
                ("MemberID", 0) => query.OrderBy(m => m.MemberID),
                ("MemberID", 1) => query.OrderByDescending(m => m.MemberID),
                ("MemberName", 0) => query.OrderBy(m => m.MemberName),
                ("MemberName", 1) => query.OrderByDescending(m => m.MemberName),
                ("IsActive", 0) => query.OrderBy(m => m.IsActive),
                ("IsActive", 1) => query.OrderByDescending(m => m.IsActive),
                ("MemberBirthday", 0) => query.OrderBy(m => m.MemberBirthday),
                ("MemberBirthday", 1) => query.OrderByDescending(m => m.MemberBirthday),
                ("KnowHow", 0) => query.OrderBy(m => m.KnowSource.SourceName),
                ("KnowHow", 1) => query.OrderByDescending(m => m.KnowSource.SourceName),
                ("MemberGender", 0) => query.OrderBy(m => m.MemberGender),
                ("MemberGender", 1) => query.OrderByDescending(m => m.MemberGender),
                ("MemberTel", 0) => query.OrderBy(m => m.MemberTel),
                ("MemberTel", 1) => query.OrderByDescending(m => m.MemberTel),
                ("LineID", 0) => query.OrderBy(m => m.LineID),
                ("LineID", 1) => query.OrderByDescending(m => m.LineID),
                _ => query.OrderBy(m => m.MemberID),
            };

            var totalCount = await query.CountAsync();

            var members = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MemberDTO
                {
                    MemberID = m.MemberID,
                    MemberName = m.MemberName,
                    MemberRole = m.MemberRole,
                    IsActive = m.IsActive,
                    MemberTel = m.MemberTel,
                    LineID = m.LineID,
                    MemberBirthday = m.MemberBirthday,
                    MemberGender = m.MemberGender,
                    MemberAddress = m.MemberAddress,
                    KnowHow = m.KnowSource != null ? m.KnowSource.SourceName : "",
                    OwnedContractsCount = m.MemberRole == "B"
                        ? _context.Contracts.Count(c => c.TrainerID == m.MemberID)
                        : (m.Contracts != null ? m.Contracts.Count : 0)
                }).ToListAsync();

            // OwnedContractsCount 排序（如需支援）
            //if (sortBy == "OwnedContractsCount")
            //    members = order == 0
            //        ? members.OrderBy(m => m.OwnedContractsCount).ToList()
            //        : members.OrderByDescending(m => m.OwnedContractsCount).ToList();

            return (members, totalCount);

        }
    }
}