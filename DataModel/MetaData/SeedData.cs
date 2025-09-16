using DataModel.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.MetaData
{
    public static class SeedData
    {

        // 初始化種子資料 (這裡使用了 IServiceProvider 來獲取 DbContext 的實例)
        // MemberRole則直接在 AppDbContext 中初始化

        public static void Initialize(IServiceProvider serviceProvider)
        {
           using(var _context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                // 確保資料庫已建立
                _context.Database.EnsureCreated();

                // 檢查 TrainingClasses 是否已存在資料
                if (!_context.TrainingClasses.Any())
                {
                    // 添加種子資料 TrainingClasses
                    _context.TrainingClasses.AddRange(
                       new TrainingClass
                       {
                           ClassTypeID = "A01",
                           ClassName = "基礎/專項12堂",
                           ClassLength = 12,
                           ClassDescription = "根據會員需求教學各種普遍性的健身手法"
                       },
                       new TrainingClass
                       {
                           ClassTypeID = "A02",
                           ClassName = "基礎/專項24堂",
                           ClassLength = 24,
                           ClassDescription = "根據會員需求教學各種普遍性的健身手法"
                       },
                        new TrainingClass
                        {
                            ClassTypeID = "S01",
                            ClassName = "學生12堂",
                            ClassLength = 12,
                            ClassDescription = "適合學生的健身課程"
                        },
                        new TrainingClass
                        {
                            ClassTypeID = "M01",
                            ClassName = "整復推拿/筋膜按摩1堂",
                            ClassLength = 1,
                            ClassDescription = "推拿課程"
                        },
                        new TrainingClass
                        {
                            ClassTypeID = "M02",
                            ClassName = "整復推拿/筋膜按摩10堂",
                            ClassLength = 10,
                            ClassDescription = "推拿課程"
                        },
                        new TrainingClass
                        {
                            ClassTypeID = "E01",
                            ClassName = "樂齡30堂",
                            ClassLength = 30,
                            ClassDescription = "適合高齡長者維持健康的健身課程"
                        },
                        new TrainingClass
                        {
                            ClassTypeID = "E02",
                            ClassName = "樂齡36堂",
                            ClassLength = 36,
                            ClassDescription = "適合高齡長者維持健康的健身課程"
                        },
                        new TrainingClass
                        {
                            ClassTypeID = "Z00",
                            ClassName = "轉移課堂",
                            ClassLength = 0,
                            ClassDescription = "由其他人轉讓的課堂"
                        },
                        new TrainingClass
                        {
                            ClassTypeID = "X00",
                            ClassName = "自由定義",
                            ClassLength = 0,
                            ClassDescription = "預設為0，由後續增購選項設定課堂量"
                        }
                    );
                    // 儲存變更
                    _context.SaveChanges();
                }
                // 檢查 PayTypes 是否已存在資料
                if (!_context.PayTypes.Any())
                {
                    // 添加種子資料 PayType
                    _context.PayTypes.AddRange(
                        new PayType
                        {
                            PayTypeID = "A",
                            PayTypeName = "現金"
                        },
                        new PayType
                        {
                            PayTypeID = "B",
                            PayTypeName = "信用卡"
                        },
                        new PayType
                        {
                            PayTypeID = "C",
                            PayTypeName = "現金分期"
                        }
                    );

                    // 儲存變更
                    _context.SaveChanges();
                }

                //檢查KnowSource是否存在資料
                if (!_context.KnowSources.Any())
                {
                    _context.KnowSources.AddRange(

                        new KnowSource
                        {
                            SourceName = "其他"
                        },
                        new KnowSource
                        {
                            SourceName = "Google Map"
                        },
                        new KnowSource
                        {
                            SourceName = "Facebook"
                        },
                        new KnowSource
                        {
                            SourceName = "Instagram"
                        },
                        new KnowSource
                        {
                            SourceName = "朋友介紹"
                        },
                        new KnowSource
                        {
                            SourceName = "路過看到"
                        }

                    );

                    _context.SaveChanges();
                }

                // 檢查 Members 是否已存在資料
                if (!_context.Members.Any())
                {
                    // 添加種子資料 Members
                    _context.Members.AddRange(
                        new Member
                        {
                            MemberID = "A00001",
                            MemberName = "王小明",
                            MemberRole = "C",
                            MemberPassword = PasswordHelper.Hash("123456", new Member()),
                            MemberTel = "0912345678",
                            LineID = "xiaoming123",
                            MemberBirthday = new DateTime(1990, 1, 1),
                            MemberGender = true, // 男性
                            MemberAddress = "台北市中正區某街道1號",
                            MemberSource = 1,
                            MemberRemark = "測試用後台管理者",
                            IsActive = true

                        },
                        new Member
                        {
                            MemberID = "A00002",
                            MemberName = "李小華",
                            MemberRole = "B",
                            MemberPassword = PasswordHelper.Hash("123456", new Member()),
                            MemberTel = "0987654321",
                            LineID = "xiaohua456",
                            MemberBirthday = new DateTime(1995, 5, 15),
                            MemberGender = false, // 女性
                            MemberAddress = "新北市板橋區某街道2號",
                            MemberSource = 2,
                            MemberRemark = "測試用教練",
                            IsActive = true
                        },
                        new Member
                        {
                            MemberID = "A00003",
                            MemberName = "陳大文",
                            MemberRole = "A",
                            MemberPassword = PasswordHelper.Hash("123456", new Member()),
                            MemberTel = "0922334455",
                            LineID = "dawen789",
                            MemberBirthday = new DateTime(1988, 3, 20),
                            MemberGender = true, // 男性
                            MemberAddress = "台中市西屯區某街道3號",
                            MemberSource = 3,
                            MemberRemark = "測試用會員",
                            IsActive = true
                        },
                        new Member
                        {
                            MemberID = "A00004",
                            MemberName = "張美麗",
                            MemberRole = "A",
                            MemberPassword = PasswordHelper.Hash("123456", new Member()),
                            MemberTel = "0933445566",
                            LineID = "meili321",
                            MemberBirthday = new DateTime(1992, 7, 30),
                            MemberGender = false, // 女性
                            MemberAddress = "高雄市苓雅區某街道4號",
                            MemberSource = 4,
                            MemberRemark = "測試用會員",
                            IsActive = true
                        },
                        new Member
                        {
                            MemberID = "A00005",
                            MemberName = "劉小英",
                            MemberRole = "A",
                            MemberPassword = PasswordHelper.Hash("123456", new Member()),
                            MemberTel = "0944556677",
                            LineID = "xiaoying654",
                            MemberBirthday = new DateTime(1998, 11, 25),
                            MemberGender = false, // 女性
                            MemberAddress = "台南市東區某街道5號",
                            MemberSource = 3,
                            MemberRemark = "測試用會員",
                            IsActive = true
                        },
                        new Member
                        {
                            MemberID = "A00006",
                            MemberName = "吳志強",
                            MemberRole = "B",
                            MemberPassword = PasswordHelper.Hash("123456", new Member()),
                            MemberTel = "0955667788",
                            LineID = "zhiqiang987",
                            MemberBirthday = new DateTime(1985, 9, 15),
                            MemberGender = true, // 男性
                            MemberAddress = "新竹市東區某街道6號",
                            MemberSource = 2,
                            MemberRemark = "測試用教練",
                            IsActive = true
                        },
                        new Member
                        {
                            MemberID = "A00007",
                            MemberName = "林小青",
                            MemberRole = "C",
                            MemberPassword = PasswordHelper.Hash("123456", new Member()),
                            MemberTel = "0966778899",
                            LineID = "xiaoqing159",
                            MemberBirthday = new DateTime(1993, 12, 5),
                            MemberGender = false, // 女性
                            MemberAddress = "嘉義市西區某街道7號",
                            MemberSource = 1,
                            MemberRemark = "測試用後台管理者",
                            IsActive = true
                        },
                        new Member
                        {
                            MemberID = "A00008",
                            MemberName = "周志豪",
                            MemberRole = "A",
                            MemberPassword = PasswordHelper.Hash("test123", new Member()),
                            MemberTel = "0977888999",
                            LineID = "zhihao888",
                            MemberBirthday = new DateTime(1991, 4, 12),
                            MemberGender = true,
                            MemberAddress = "台北市信義區某街道8號",
                            MemberSource = 2,
                            MemberRemark = "測試用會員",
                            IsActive = true
                        },
                        new Member
                        {
                            MemberID = "A00009",
                            MemberName = "黃美珍",
                            MemberRole = "A",
                            MemberPassword = PasswordHelper.Hash("test123", new Member()),
                            MemberTel = "0988111222",
                            LineID = "meizhen999",
                            MemberBirthday = new DateTime(1987, 8, 23),
                            MemberGender = false,
                            MemberAddress = "新北市新店區某街道9號",
                            MemberSource = 1,
                            MemberRemark = "測試用會員",
                            IsActive = true
                        },
                        new Member
                        {
                            MemberID = "A00010",
                            MemberName = "林建宏",
                            MemberRole = "A",
                            MemberPassword = PasswordHelper.Hash("test123", new Member()),
                            MemberTel = "0911222333",
                            LineID = "jianhong1010",
                            MemberBirthday = new DateTime(1996, 2, 5),
                            MemberGender = true,
                            MemberAddress = "桃園市中壢區某街道10號",
                            MemberSource = 3,
                            MemberRemark = "測試用會員",
                            IsActive = true
                        },
                        new Member
                        {
                            MemberID = "A00011",
                            MemberName = "陳怡君",
                            MemberRole = "A",
                            MemberPassword = PasswordHelper.Hash("test123", new Member()),
                            MemberTel = "0922111333",
                            LineID = "yijun111",
                            MemberBirthday = new DateTime(1994, 6, 18),
                            MemberGender = false,
                            MemberAddress = "台南市北區某街道11號",
                            MemberSource = 4,
                            MemberRemark = "測試用會員",
                            IsActive = true
                        },
                        new Member
                        {
                            MemberID = "A00012",
                            MemberName = "李國強",
                            MemberRole = "A",
                            MemberPassword = PasswordHelper.Hash("test123", new Member()),
                            MemberTel = "0933111222",
                            LineID = "guoqiang112",
                            MemberBirthday = new DateTime(1989, 10, 30),
                            MemberGender = true,
                            MemberAddress = "高雄市三民區某街道12號",
                            MemberSource = 2,
                            MemberRemark = "測試用會員",
                            IsActive = true
                        }

                    );
                    // 儲存變更
                    _context.SaveChanges();
                }


                // 在 Contracts 種子資料區塊內，替換原本的 AddRange，生成 15 筆合約資料
                if (!_context.Contracts.Any())
                {
                    // 取得各角色的會員
                    var membersA = _context.Members.Where(m => m.MemberRole == "A").ToList();
                    var trainersB = _context.Members.Where(m => m.MemberRole == "B").ToList();
                    var handlersC = _context.Members.Where(m => m.MemberRole == "C").ToList();
                    // 排除 X00 和 Z00
                    var classTypes = _context.TrainingClasses
                        .Where(tc => tc.ClassTypeID != "X00" && tc.ClassTypeID != "Z00")
                        .ToList();

                    // 每個會員A的已用ClassTypeID
                    var memberUsedClassTypeIds = new Dictionary<string, HashSet<string>>();
                    var contracts = new List<Contract>();
                    int contractSerial = 1;
                    var random = new Random();
                    var payTypeIds = new[] { "A", "B", "C" };

                    int maxContracts = 15;
                    int createdContracts = 0;

                    // 盡量分配給不同會員，直到達到15筆或無法分配
                    while (createdContracts < maxContracts)
                    {
                        // 隨機選擇一個會員A
                        var member = membersA[random.Next(membersA.Count)];
                        if (!memberUsedClassTypeIds.ContainsKey(member.MemberID))
                            memberUsedClassTypeIds[member.MemberID] = new HashSet<string>();

                        // 找出此會員尚未使用過的ClassType
                        var availableClassTypes = classTypes
                            .Where(tc => !memberUsedClassTypeIds[member.MemberID].Contains(tc.ClassTypeID))
                            .ToList();

                        if (!availableClassTypes.Any())
                        {
                            // 該會員已用完所有ClassType，換下一個會員
                            // 若所有會員都用完則跳出
                            bool allUsedUp = membersA.All(m =>
                                memberUsedClassTypeIds.ContainsKey(m.MemberID) &&
                                memberUsedClassTypeIds[m.MemberID].Count >= classTypes.Count);
                            if (allUsedUp) break;
                            continue;
                        }

                        var classType = availableClassTypes[random.Next(availableClassTypes.Count)];
                        memberUsedClassTypeIds[member.MemberID].Add(classType.ClassTypeID);

                        // 隨機選擇一個教練B
                        var trainer = trainersB[random.Next(trainersB.Count)];
                        // 隨機選擇一個Handler C
                        var handler = handlersC[random.Next(handlersC.Count)];

                        // 生成合約ID
                        int year = 2025;
                        int month = 8;
                        string contractId = $"A{year}{month.ToString("D2")}{contractSerial.ToString("D2")}";

                        // 簽約日期
                        DateTime signDate = new DateTime(year, month, random.Next(1, 28));
                        // 結束日期 = 簽約日 + (課程長度 * 5天)
                        DateTime endDate = signDate.AddDays(classType.ClassLength * 5);

                        // 隨機選擇支付方式
                        string payTypeId = payTypeIds[random.Next(payTypeIds.Length)];

                        contracts.Add(new Contract
                        {
                            ContractID = contractId,
                            Signer = member.MemberName,
                            MemberID = member.MemberID,
                            TrainerID = trainer.MemberID,
                            HandlerID = handler.MemberID,
                            SignDate = signDate,
                            EndDate = endDate,
                            ClassTypeID = classType.ClassTypeID,
                            PayTypeID = payTypeId
                        });

                        contractSerial++;
                        createdContracts++;
                    }

                    _context.Contracts.AddRange(contracts);
                    _context.SaveChanges();
                }

                //檢查Attendance是否存在資料
                if (!_context.Attendances.Any())
                {
                    var contracts = _context.Contracts.ToList();
                    var membersA = _context.Members.Where(m => m.MemberRole == "A").ToList();
                    var trainersB = _context.Members.Where(m => m.MemberRole == "B").ToList();

                    var random = new Random();
                    var attendanceList = new List<Attendance>();
                    var contractAttendanceDates = new Dictionary<string, HashSet<DateTime>>();

                    // 取得合約的最大簽到次數 (依據ClassLength)
                    var contractClassLength = contracts.ToDictionary(
                        c => c.ContractID,
                        c => _context.TrainingClasses.FirstOrDefault(tc => tc.ClassTypeID == c.ClassTypeID)?.ClassLength ?? 0
                    );

                    int created = 0;
                    int maxAttendances = 30;

                    while (created < maxAttendances)
                    {
                        // 隨機選擇一個合約
                        var contract = contracts[random.Next(contracts.Count)];

                        // 檢查合約的MemberID和TrainerID角色
                        var member = membersA.FirstOrDefault(m => m.MemberID == contract.MemberID);
                        var trainer = trainersB.FirstOrDefault(t => t.MemberID == contract.TrainerID);
                        if (member == null || trainer == null) continue;

                        // 初始化此合約的已簽到日期
                        if (!contractAttendanceDates.ContainsKey(contract.ContractID))
                            contractAttendanceDates[contract.ContractID] = new HashSet<DateTime>();

                        // 檢查是否已達最大簽到次數
                        if (contractAttendanceDates[contract.ContractID].Count >= contractClassLength[contract.ContractID])
                            continue;

                        // 產生一個合法的簽到日期 (合約開始日+1 ~ 現在)
                        DateTime startDate = contract.SignDate.AddDays(1);
                        DateTime endDate = DateTime.Now;
                        int range = (endDate - startDate).Days;
                        if (range <= 0) continue;

                        DateTime attendanceDate;
                        int tryCount = 0;
                        do
                        {
                            // 隨機日期 + 隨機時間 (精確到秒)
                            attendanceDate = startDate.AddDays(random.Next(range))
                                .AddHours(random.Next(0, 24))
                                .AddMinutes(random.Next(0, 60))
                                .AddSeconds(random.Next(0, 60));
                            tryCount++;
                        } while (contractAttendanceDates[contract.ContractID].Contains(attendanceDate) && tryCount < 10);

                        if (contractAttendanceDates[contract.ContractID].Contains(attendanceDate))
                            continue;

                        // 新增簽到記錄
                        attendanceList.Add(new Attendance
                        {
                            AttendanceID = Guid.NewGuid().ToString(),
                            ContractID = contract.ContractID,
                            MemberID = member.MemberID,
                            TrainerID = trainer.MemberID,
                            AttendanceDate = attendanceDate
                        });

                        contractAttendanceDates[contract.ContractID].Add(attendanceDate);
                        created++;
                    }

                    _context.Attendances.AddRange(attendanceList);
                    _context.SaveChanges();
                }




                return;
            }
        }
    }
}
