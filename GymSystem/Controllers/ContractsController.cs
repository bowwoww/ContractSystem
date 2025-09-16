using DataModel;
using DataModel.DTO;
using DataModel.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymSystem.Controllers
{
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDGenerateService _idService;
        private readonly ContractDetailService _contractDetailService;
        private readonly LogService _logService;

        public ContractsController(AppDbContext context,IDGenerateService contractIDGenerateService, ContractDetailService contractDetailService, LogService logService)
        {
            _context = context;
            _idService = contractIDGenerateService;
            _contractDetailService = contractDetailService;
            _logService = logService;
        }

        [Authorize]
        // GET: Contracts
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> ShowContracts(string memberId, int pageIndex = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "會員ID不可為空" });
            }
            //取得登入狀態的使用者id和role
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (currentUserId != memberId && currentUserRole != "B" && currentUserRole != "C")
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "您無權限查看此會員的合約" });
            }

            // 用service取得page的contractDTOs和totalCount , 如果是管理員則memberId設為0使service回傳所有合約 
            var temp = memberId;
            if (currentUserRole == "C" || currentUserRole == "B")
                temp = "0";
            var (contractDTOs, totalCount) = await _contractDetailService.GetContractsPagedAsync(pageIndex, pageSize, temp, "", 1);

            if (contractDTOs == null || contractDTOs.Count == 0)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "查無此會員的合約" });
            }

            // 若要在 View 顯示分頁資訊，可用 ViewBag 傳遞
            ViewBag.TotalCount = totalCount;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;

            return View(contractDTOs);
        }

        [Authorize]
        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Handler)
                .Include(c => c.Member)
                .Include(c => c.PayType)
                .Include(c => c.Trainer)
                .Include(c => c.TrainingClass)
                .FirstOrDefaultAsync(m => m.ContractID == id);

            if (contract == null)
            {
                return NotFound();
            }


            //傳遞課堂數相關資訊到 View
            ViewData["classLength"] = await _contractDetailService.GetClassLength(contract.ContractID);
            ViewData["attendancedClass"] = await _contractDetailService.GetAttendancedClass(contract.ContractID);
            ViewData["addClassCount"] = await _contractDetailService.GetAddClass(contract.ContractID);
            ViewData["TransferClassCount"] = await _contractDetailService.GetTransferClass(contract.ContractID);

            var newEndDate = await _contractDetailService.GetEndDate(contract.ContractID);

            ViewData["endDate"] = newEndDate;

            return View(contract);
        }

        [Authorize(Roles ="C")]
        // GET: Contracts/Creates
        public IActionResult Create()
        {

            string newContractID = _idService.GenerateNewContractID().Result;


            var contract = new Contract
            {
                ContractID = newContractID,
                SignDate = DateTime.Now
            };

            ViewData["PayTypeID"] = new SelectList(_context.PayTypes, "PayTypeID", "PayTypeName");
            ViewData["ClassTypeID"] = new SelectList(_context.TrainingClasses, "ClassTypeID", "ClassName");
            return View(contract);
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "C")]
        public async Task<IActionResult> Create([Bind("ContractID,Signer,MemberID,TrainerID,HandlerID,SignDate,ClassTypeID,PayTypeID")] Contract contract)
        {
            ModelState.Remove("EndDate");
            if (ModelState.IsValid)
            {
                int addDays = await _context.TrainingClasses
                            .Where(tc => tc.ClassTypeID == contract.ClassTypeID)
                            .Select(tc => tc.ClassLength)
                            .FirstOrDefaultAsync() * 5;  //預設合約有效期為課程堂數的5倍天數

                contract.EndDate = DateTime.Now.AddDays(addDays);
                _context.Add(contract);

                _logService.Log(contract.ContractID);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HandlerID"] = new SelectList(_context.Members.Where(m => m.MemberRole == "C"), "MemberID", "MemberName");
            ViewData["PayTypeID"] = new SelectList(_context.PayTypes, "PayTypeID", "PayTypeName" , contract.PayTypeID);
            ViewData["ClassTypeID"] = new SelectList(_context.TrainingClasses, "ClassTypeID", "ClassName", contract.ClassTypeID);
            return View(contract);
        }

        [Authorize(Roles = "B")]
        //預約上課時間
        public async Task<IActionResult> Book(string contractId)
        {
            
            if (string.IsNullOrEmpty(contractId))
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "合約ID不可為空" });
            }
            if (!ContractExists(contractId))
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "查無此合約" });
            }
            


            var collections = await _context.TrainingDates.Where(t => t.ContractID == contractId).ToListAsync();
            ViewData["ContractID"] = contractId;

            return View(collections);
        }

        //更新預約的上課時間
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "B")]
        public async Task<IActionResult> UpdateTrainingDates(string contractId, string selectedDates, string deletedDates,string trainerId)
        {
            var dates = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(selectedDates);
            var datesRemove = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(deletedDates);
            if (dates == null && datesRemove == null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "合約ID或日期不可為空" });
            }
            if (String.IsNullOrWhiteSpace(contractId) || String.IsNullOrWhiteSpace(trainerId))
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "查無此合約和教練" });
            }

            if (dates != null && dates.Count > 0)
            {
                foreach (var date in dates)
                {
                    if (DateTime.TryParse(date, out DateTime trainingDate))
                    {

                        string trainingDateIdStr = await _idService.GenerateNewTrainingDateID(trainingDate);

                        var newTrainingDate = new TrainingDate
                        {
                            TrainingDateID = trainingDateIdStr,
                            ClassDate = trainingDate,
                            ContractID = contractId,
                            TrainerID = trainerId
                        };
                        _context.TrainingDates.Add(newTrainingDate);
                    }
                }
            }

            // 刪除預約的上課時間
            if (datesRemove != null && datesRemove.Count > 0)
            {
                foreach (var dateStr in datesRemove)
                {
                    if (DateTime.TryParse(dateStr, out DateTime removeDate))
                    {
                        // 找出該合約下該日期的所有 TrainingDate
                        var trainingDatesToDelete = await _context.TrainingDates
                            .Where(td => td.ContractID == contractId && td.ClassDate.Date == removeDate.Date)
                            .ToListAsync();

                        if (trainingDatesToDelete != null && trainingDatesToDelete.Count > 0)
                        {
                            _context.TrainingDates.RemoveRange(trainingDatesToDelete);
                        }
                    }
                }
            }
            _logService.Log(contractId);
            await _context.SaveChangesAsync();
            return RedirectToAction("Book", new { contractId });
        }


        private bool ContractExists(string id)
        {
            return _context.Contracts.Any(e => e.ContractID == id);
        }

    }
}
