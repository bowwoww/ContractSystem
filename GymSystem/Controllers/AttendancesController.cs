using DataModel;
using DataModel.DTO;
using DataModel.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QRCoder;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymSystem.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly string _qrSecret;

        public AttendancesController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _qrSecret = config["QrSettings:SecretKey"];
            _qrSecret ??= "o2jfnai104ja";   //如果沒設定就給預設值
        }

        [HttpGet]
        [Authorize]
        // GET: Attendances
        public async Task<IActionResult> Index(string id)
        {
            var attendances = await _context.Attendances
                            .Include(a => a.Contract)
                                .ThenInclude(c => c.TrainingClass)
                            .Include(a => a.Trainer)
                            .Where(a => a.MemberID == id)
                            .ToListAsync();

            // 取得所有合約(Contract)的課程名稱(TrainingClass.ClassName)和訓練日期(TrainingDates)
            // 並組成 List<BookedDTO>
            var bookedDTOs = await _context.Contracts
                    .Include(c => c.Trainer)
                    .Include(c => c.TrainingClass)
                    .Include(c => c.TrainingDates)
                    .Where(c => c.MemberID == id && c.TrainingDates.Any(td => td.ClassDate > DateTime.Now.Date))
                    .Select(c => new BookedDTO
                    {
                        TrainerName = c.Trainer != null ? c.Trainer.MemberName : "未知教練",
                        ContractCreatDate = c.SignDate,
                        ContractClassName = c.TrainingClass != null ? c.TrainingClass.ClassName : "未知課程",
                        ClassDate = c.TrainingDates
                            .Where(td => td.ClassDate > DateTime.Now.Date)
                            .OrderBy(td => td.ClassDate)
                            .Select(td => td.ClassDate)
                            .ToList()
                    })
                    .ToListAsync();

            ViewData["BookedDTOs"] = bookedDTOs;


            return View(attendances);
        }

        // GET: Attendances/GenerateQr
        [HttpGet]
        [Authorize(Roles = "A")]
        public async Task<IActionResult> GenerateQr(string contractId)
        {
            var memberId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (memberId == null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "根本沒有登入者" });
            }
            var signTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            var expires = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-ddTHH:mm:ss");

            var key = QrHelper.GenerateQrKey(contractId, memberId, signTime, expires, _qrSecret);
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var qrUrl = $"{baseUrl}/Attendances/ScanQr?contractId={contractId}&memberId={memberId}&signTime={signTime}&expires={expires}&key={Uri.EscapeDataString(key)}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            await Task.CompletedTask; // 保持 async signature
            return File(qrCodeImage, "image/png");
        }

        // GET: Attendances/CheckIn
        [HttpGet]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> ScanQr(string contractId, string memberId, string signTime, string expires, string key)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Home", new { ReturnUrl = Request.Path + Request.QueryString });

            if (DateTime.Parse(expires) < DateTime.Now)
                return RedirectToAction("Error", "Home", new { errorMessage = "QR Code 已過期" });


            var expectedKey = QrHelper.GenerateQrKey(contractId, memberId, signTime, expires, _qrSecret);
            if (key != expectedKey)
                return RedirectToAction("Error", "Home", new { errorMessage = "QR Code 驗證失敗" });

            var alreadyCheckedIn = await _context.Attendances.AnyAsync(a =>
                a.ContractID == contractId &&
                a.MemberID == memberId &&
                a.AttendanceDate == DateTime.Parse(signTime));

            if (alreadyCheckedIn) 
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "此簽到已完成，請勿重複簽到。" });
            }

            var trainerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (trainerId == null)
            { return RedirectToAction("Error", "Home", new { errorMessage = "教練不存在或未登入" }); }
                var trainer = await _context.Members.FirstOrDefaultAsync(m => m.MemberID == trainerId);
            if (trainer == null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "教練不存在或未登入" });
            }
            if (trainer.MemberRole == "B")
            {
                var attendance = new Attendance
                {
                    AttendanceID = Guid.NewGuid().ToString(),
                    ContractID = contractId,
                    MemberID = memberId,
                    TrainerID = trainerId,
                    AttendanceDate = DateTime.Parse(signTime)
                };
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = attendance.AttendanceID });
            }
            //當掃描簽到者身分為後台時跳轉另外頁面選擇教練
            if (trainer.MemberRole == "C")
            {
                var coachList = await _context.Members
                    .Where(m => m.MemberRole == "B")
                    .Select(m => new { m.MemberID, m.MemberName })
                    .ToListAsync();

                string memberName = await _context.Members
                    .Where(m => m.MemberID == memberId)
                    .Select(m => m.MemberName)
                    .FirstOrDefaultAsync() ?? "未知會員";

                ViewData["MemberName"] = memberName;
                ViewData["ContractID"] = contractId;
                ViewData["MemberID"] = memberId;
                ViewData["SignTime"] = signTime;
                ViewData["CoachList"] = new SelectList(coachList, "MemberID", "MemberName");
                return View("SelectTrainer");
            }

            return Forbid();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> ConfirmTrainerSign(string ContractID, string MemberID, string SignTime, string TrainerID)
        {
            
            var attendance = new Attendance
            {
                AttendanceID = Guid.NewGuid().ToString(),
                ContractID = ContractID,
                MemberID = MemberID,
                TrainerID = TrainerID,
                AttendanceDate = DateTime.Parse(SignTime)
            };
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = attendance.AttendanceID });
        }

        [HttpGet]
        [Authorize(Roles = "C")]
        // GET: Attendances/Create
        public async Task<IActionResult> Create(string contractId)
        {
            if(string.IsNullOrEmpty(contractId))
            {
                return BadRequest("必須提供 ContractID 來建立簽到記錄");
            }
            var contract = await _context.Contracts.FindAsync(contractId);
            Attendance attendance = new Attendance();
            attendance.ContractID = contractId;
            if (contract != null) {
                attendance.Member = await _context.Members.FindAsync(contract.MemberID);
                attendance.Trainer = await _context.Members.FindAsync(contract.TrainerID);
            }

            return View(attendance);
        }

        // POST: Attendances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "C")]
        public async Task<IActionResult> Create([Bind("ContractID,MemberID,TrainerID,AttendanceDate")] Attendance attendance)
        {
            
            ModelState.Remove("AttendanceID"); // 移除該欄位的驗證
            if (ModelState.IsValid)
            {
                attendance.AttendanceID = Guid.NewGuid().ToString();
                _context.Add(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create","Attendances", new {contractId = attendance.ContractID});
            }

            return RedirectToAction("ShowContracts", "Contracts", new {memberId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value });
        }

        [HttpGet]
        [Authorize(Roles = "C")]
        // GET: Attendances/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }
            Contract contract = await _context.Contracts.FindAsync(attendance.ContractID);
            if (contract != null)
            {
                attendance.Member = await _context.Members.FindAsync(contract.MemberID);
                attendance.Trainer = await _context.Members.FindAsync(contract.TrainerID);
            }
            return View(attendance);
        }

        // POST: Attendances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "C")]
        public async Task<IActionResult> Edit(string id, [Bind("AttendanceID,ContractID,MemberID,TrainerID,AttendanceDate")] Attendance attendance)
        {
            if (id != attendance.AttendanceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceExists(attendance.AttendanceID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var memberId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                return RedirectToAction("Index", "Attendances", new { id = memberId });
            }

            ModelState.AddModelError("", "無法更新簽到記錄，請檢查輸入的資料。");

            return View(attendance);
        }


        // POST: Attendances/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "C")]
        public async Task<IActionResult> Delete(string id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
            }

            await _context.SaveChangesAsync();
            var memberId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return RedirectToAction("Index", "Attendances", new { id = memberId });
        }

        private bool AttendanceExists(string id)
        {
            return _context.Attendances.Any(e => e.AttendanceID == id);
        }
    }
}
