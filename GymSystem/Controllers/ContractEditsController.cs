using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DataModel;
using DataModel.Service;

namespace GymSystem.Controllers
{
    [Authorize(Roles = "C")]
    public class ContractEditsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDGenerateService _idService;
        private readonly LogService _logService;

        public ContractEditsController(AppDbContext context,IDGenerateService contractIDGenerateService, LogService logService)
        {
            _context = context;
            _idService = contractIDGenerateService;
            _logService = logService;
        }

        // GET: ContractEdits
        public async Task<IActionResult> Index(string contractId)
        {
            if (string.IsNullOrEmpty(contractId))
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "請提供合約編號" });
            }
            var contract = await _context.Contracts
                        .Include(c => c.TrainingClass)
                        .FirstOrDefaultAsync(c => c.ContractID == contractId);
            var editList = await _context.ContractEdits
                        .Include(c => c.Contract)
                            .ThenInclude(c => c.TrainingClass)
                        .Include(c => c.Handler)
                        .Where(c => c.ContractID == contractId)
                        .ToListAsync();

            ViewData["contractId"] = contractId;
            ViewData["signer"] = contract?.Signer ?? "";
            ViewData["className"] = contract?.TrainingClass?.ClassName ?? "";

            return View(editList);
        }


        // GET: ContractEdits/Create
        public IActionResult Create(string contractId)
        {
            var ce = new ContractEdit();
            if (!string.IsNullOrEmpty(contractId))
            {
                var contract = _context.Contracts
                    .FirstOrDefault(c => c.ContractID == contractId);
                if (contract != null)
                {
                    ce.ContractID = contract.ContractID;
                    ce.NewEndDate = contract.EndDate.AddMonths(1);
                    ViewData["signer"] = contract.Signer;
                }
            }
            
            ViewData["HandlerID"] = new SelectList(_context.Members.Where(m => m.MemberRole == "C"), "MemberID", "MemberName");
            ViewData["EditType"] = new SelectList(Enum.GetValues(typeof(EditType))
                .Cast<EditType>()
                .Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text");
            return View(ce);
        }

        // POST: ContractEdits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string signer, [Bind("Id,ContractID,HandlerID,EditDate,EditType,NewEndDate,AddClassCount,TransferToMemberID,TransferClassCount,Remarks")] ContractEdit contractEdit)
        {
            if (ModelState.IsValid)
            {
                // 根據 EditType 處理不同的邏輯
                if (contractEdit.EditType == EditType.延展結束日期) // 延展結束日期
                {
                    contractEdit.AddClassCount = null;
                    contractEdit.TransferToMemberID = null;
                    contractEdit.TransferClassCount = null;
                }
                else if (contractEdit.EditType == EditType.增加課堂數) // 增加課堂數
                {
                    contractEdit.NewEndDate = null;
                    contractEdit.TransferToMemberID = null;
                    contractEdit.TransferClassCount = null;
                }
                else if (contractEdit.EditType == EditType.轉讓課堂 && contractEdit.TransferToMemberID != null && contractEdit.TransferClassCount > 0) // 轉讓課堂
                {
                    contractEdit.NewEndDate = null;
                    contractEdit.AddClassCount = null;

                    //檢查是否存在轉讓課堂的合約
                    var existingContract = await _context.Contracts
                        .Include(c => c.TrainingClass)
                        .FirstOrDefaultAsync(c => c.ClassTypeID == "Z00" && c.MemberID == contractEdit.TransferToMemberID && c.EndDate > DateTime.Now);
                    if(existingContract == null)
                    {
                        // 若合約不存在，建立一個新的合約給轉讓對象
                        Contract contract = new Contract();
                        contract.ContractID = await _idService.GenerateNewContractID();
                        contract.Signer = signer;
                        contract.MemberID = contractEdit.TransferToMemberID;
                        contract.TrainerID = await _context.Contracts
                            .Where(c => c.ContractID == contractEdit.ContractID)
                            .Select(c => c.TrainerID)
                            .FirstOrDefaultAsync();
                        contract.HandlerID = contractEdit.HandlerID;
                        contract.SignDate = DateTime.Now;

                        int defaultClass = contractEdit.TransferClassCount ?? 0;

                        contract.EndDate = DateTime.Now.AddDays(defaultClass * 5); // 預設合約課堂數 * 5 天
                        contract.ClassTypeID = "Z00"; // Z00 代表轉讓課堂的課程類別
                        _context.Add(contract);
                    }
                }
                _logService.Log(contractEdit.ContractID);

                _context.Add(contractEdit);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "ContractEdits", new { contractId = contractEdit.ContractID});
            }
            else
            {
                //取得ModelState 回傳的錯誤訊息
                string errorMessages = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return RedirectToAction("Error", "Home", new {errorMessage =  errorMessages});
            }
        }

        // GET: ContractEdits/Edit/5

        private bool ContractEditExists(int id)
        {
            return _context.ContractEdits.Any(e => e.Id == id);
        }
    }
}
