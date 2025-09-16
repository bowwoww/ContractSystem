using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataModel;
using Microsoft.AspNetCore.Identity;
using DataModel.Security;
using DataModel.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GymSystem.Controllers
{
    public class MembersController : Controller
    {
        private readonly AppDbContext _context;

        public MembersController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "C")]
        // GET: Members
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Members.Include(m => m.KnowSource).Include(m => m.MemberRoleNavigation);
            return View(await appDbContext.ToListAsync());
        }

        [Authorize(Roles = "C")]
        // GET: Members/Create
        public async Task<IActionResult> Create()
        {
            //MemberID 先搜索最大值，並自動產生下一個ID
            string maxId;
            maxId = await _context.Members
                .OrderByDescending(m => m.MemberID)
                .Select(m => m.MemberID)
                .FirstOrDefaultAsync();
            if(string.IsNullOrEmpty(maxId))
            {
                maxId = "A00001"; //若無資料則從A00001開始
            }

            Member member = new Member();
            member.MemberID = "A" + (int.Parse(maxId.Substring(1)) + 1).ToString("D5");

            ViewData["MemberSource"] = new SelectList(_context.KnowSources, "Id", "SourceName");
            ViewData["MemberRole"] = new SelectList(_context.MemberRoles, "MemberRoleID", "MemberRoleName");
            return View(member);
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "C")]
        public async Task<IActionResult> Create([Bind("MemberID,MemberName,MemberRole,MemberTel,LineID,MemberBirthday,MemberGender,MemberAddress,MemberSource,MemberRemark,IsActive")] Member member)
        {
            ModelState.Remove("MemberPassword");
            if (ModelState.IsValid)
            {
                //預設密碼為手機號碼，並移除減號(若無電話則預設為123456)
                if (string.IsNullOrWhiteSpace(member.MemberTel))
                    { member.MemberPassword = "123456"; }
                else
                    { member.MemberPassword = member.MemberTel.Replace("-", ""); }

                member.MemberPassword = PasswordHelper.Hash(member.MemberPassword, member);
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberSource"] = new SelectList(_context.KnowSources, "Id", "SourceName", member.MemberSource);
            ViewData["MemberRole"] = new SelectList(_context.MemberRoles, "MemberRoleID", "MemberRoleID", member.MemberRole);
            return View(member);
        }

        // GET: Members/Edit/5
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(string id, string MemberID,string MemberName,string? MemberTel,string? LineID,string? MemberAddress)
        {
            if (id != MemberID)
            {
                return NotFound();
            }
            Member member = await _context.Members.FindAsync(id);
            member.MemberName = MemberName;
            member.MemberTel = MemberTel;
            member.LineID = LineID;
            member.MemberAddress = MemberAddress;
            member.MemberRemark = Request.Form["MemberRemark"];

            try
            {
                _context.Update(member);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(member.MemberID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return View(member);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel cpData)
        {
            //驗證會員身分和密碼正確
            Member member = await _context.Members.FindAsync(cpData.MemberID);
            string response = "密碼錯誤";
            if (ModelState.IsValid) {
                if (PasswordHelper.Verify(cpData.OldPassword, member))
                { 
                    member.MemberPassword = PasswordHelper.Hash(cpData.NewPassword, member);
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                    response = "密碼已成功更改";
                }
            }
            else
            {
                //取得ModelState錯誤訊息
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                response = string.Join(" ", errors.Select(e => e.ErrorMessage));
            }
            TempData["Response"] = response;
            return RedirectToAction(nameof(Edit), new { id = cpData.MemberID });
        }

        //僅後台人員權限可用該action
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "C")]
        public async Task<IActionResult> ResetDefaultPassword(string memberId)
        {
            Member member = await _context.Members.FindAsync(memberId);

            //預設密碼為手機號碼，並移除減號
            string defaultPassword = member.MemberTel?.Replace("-", "");
            if (string.IsNullOrEmpty(defaultPassword))
            {
                TempData["Response"] = "會員電話號碼不存在，無法重設密碼";
                return RedirectToAction(nameof(Edit), new { id = memberId });
            }
            member.MemberPassword = PasswordHelper.Hash(defaultPassword, member);
            _context.Update(member);
            await _context.SaveChangesAsync();
            TempData["Response"] = "密碼已重設為會員電話號碼（不含減號）";
            return RedirectToAction(nameof(Edit), new { id = memberId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="C")]
        public async Task<IActionResult> SetActiveStatus(string memberId)
        {
            Member member = await _context.Members.FindAsync(memberId);
            if (member == null) {
                return RedirectToAction("Error", "Home", new { errorMessage = "未找到會員" });
            }
            member.IsActive = !member.IsActive;
            _context.Update(member);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = memberId });
        }

        private bool MemberExists(string id)
        {
            return _context.Members.Any(e => e.MemberID == id);
        }

        [HttpGet]
        [Authorize]
        public JsonResult Search(string term)
        {
            var members = _context.Members
                .Where(m => m.MemberName.Contains(term))
                .Select(m => new { memberID = m.MemberID, memberName = m.MemberName })
                .Take(10)
                .ToList();
            return Json(members);
        }
    }
}
