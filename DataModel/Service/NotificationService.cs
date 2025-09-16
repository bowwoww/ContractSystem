using DataModel.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Service
{
    public class NotificationService
    {
        private readonly AppDbContext _context;
        private readonly ContractDetailService _contractService;
        const int dayNotifity = 5;
        const int birthdayNotifityDays = 7;
        const int classExhaustNotifityCount = 5;

        public NotificationService(AppDbContext context, ContractDetailService contractService)
        {
            _context = context;
            _contractService = contractService;
        }

        public async Task<List<NotifityModel>> GetNotificationAsync(Member member)
        {
            int pageIndex = 1;
            int pageSize = 100;
            int temp = 0;

            var notifications = new List<NotifityModel>();

            var (contracts, totalCount) = await _contractService.GetContractsPagedAsync(pageIndex, pageSize, "0", "", 1);
            contracts = contracts.Where(c => c.UsedClassCount < c.MaxClassCount).ToList();

            while (totalCount > temp * pageSize)
            {
                temp++;
                // 課程預約提醒
                if(member.MemberRole == "A" || member.MemberRole == "B")
                    await GetBookNotification(contracts, member, notifications);

                // 角色B 教練
                if (member.MemberRole == "B")
                {
                    // 學生生日提醒
                    await GetBirthdayNotification(contracts, member, notifications);
                    // 合約到期提醒
                    await GetContractEndNotification(contracts, member, notifications);
                }

                if (totalCount > temp * pageSize)
                {
                    (contracts, totalCount) = await _contractService.GetContractsPagedAsync(temp + 1, pageSize, "0", "", 1);
                    contracts = contracts.Where(c => c.UsedClassCount < c.MaxClassCount).ToList();
                }
            }





            
            




            // 依 EstimateTime 排序
            notifications = notifications.OrderBy(n => n.EstimateTime).ToList();

            return notifications;
        }

        //課程預約提醒
        public async Task GetBookNotification(List<ContractDTO> contracts,Member member, List<NotifityModel> notifications)
        {

            if (contracts != null)
            {
                var contractIds = new List<string>();
                if (member.MemberRole == "A")
                    contractIds = contracts.Where(c => c.Member.MemberID == member.MemberID).Select(c => c.ContractID).ToList();
                else
                    contractIds = contracts.Select(c => c.ContractID).ToList();
                // 查詢該合約下的最近課程日期
                var query = _context.TrainingDates
                        .Where(t => contractIds.Contains(t.ContractID)
                            && t.ClassDate <= DateTime.Now.AddDays(dayNotifity));
                if(member.MemberRole == "B")
                    query = query.Where(t => t.TrainerID == member.MemberID);

                var bookdates = await query.OrderBy(t => t.ClassDate)
                                           .ToListAsync();

                // 如果有找到最近的課程日期，則建立通知訊息
                foreach (var bookdate in bookdates)
                {
                    var contract = contracts.FirstOrDefault(c => c.ContractID == bookdate.ContractID);
                    if (contract == null) continue;

                    string store = "健身寓本店";
                    string roleMessage = "";
                    if (member.MemberRole == "A")
                        roleMessage = $"教練為{contract.Trainer?.MemberName}";
                    else if (member.MemberRole == "B")
                        roleMessage = $"學生為{contract.Member?.MemberName}";
                    notifications.Add(new NotifityModel
                    {
                        Title = "預約提醒",
                        MessageUpper = $"您在{store}有一堂課程預約，{roleMessage}",
                        MessageBottom = $"預約時間為{bookdate.ClassDate}",
                        EstimateTime = bookdate.ClassDate
                    });
                }
            }
        }

        public async Task GetBirthdayNotification(List<ContractDTO> contracts ,Member trainer, List<NotifityModel> notifications)
        {

            // 生日提醒只通知一次
            var studentIds = contracts.Select(c => c.Member.MemberID)
                            .Where(m => m != null)
                            .Distinct()
                            .ToList();

            var students = await _context.Members
                                .Where(m => studentIds.Contains(m.MemberID) && m.MemberBirthday != null)
                                .ToListAsync();

            if (students != null && students.Count > 0)
            {
                foreach (var student in students)
                {
                    if (student == null || student.MemberBirthday == null)
                        continue; // 生日為 null，跳過

                    var now = DateTime.Now.Date;
                    var birthday = student.MemberBirthday.Value; // 取出 DateTime
                    var birthdayThisYear = new DateTime(now.Year, birthday.Month, birthday.Day);

                    // 若今年生日已過，則取明年生日
                    if (birthdayThisYear < now)
                        birthdayThisYear = birthdayThisYear.AddYears(1);

                    var daysUntilBirthday = (birthdayThisYear - now).TotalDays;
                    if (daysUntilBirthday >= 0 && daysUntilBirthday <= birthdayNotifityDays)
                    {
                        notifications.Add(new NotifityModel
                        {
                            Title = "學生生日提醒",
                            MessageUpper = $"學生 {student.MemberName} 的生日即將到來",
                            MessageBottom = $"生日日期：{birthdayThisYear:yyyy/MM/dd}",
                            EstimateTime = birthdayThisYear
                        });
                    }
                }
            }
        }

        public async Task GetContractEndNotification(List<ContractDTO> contracts, Member trainer, List<NotifityModel> notifications)
        {

            var nearEndContracts = contracts
                                    .Where(c => c.MaxClassCount - c.UsedClassCount <= classExhaustNotifityCount)
                                    .ToList();

            foreach (var contract in nearEndContracts)
            {
                var temp = new NotifityModel
                {
                    Title = "課程堂數提醒",
                    MessageUpper = $"學生{contract.Member.MemberName}的課程堂數即將用完",
                    MessageBottom = $"剩餘堂數：{contract.UsedClassCount} / {contract.MaxClassCount}堂",
                    EstimateTime = DateTime.Now,
                    NotifyValue = contract.MaxClassCount - contract.UsedClassCount
                };
                notifications.Add(temp);
            }
            await Task.CompletedTask;
        }
    }
}
