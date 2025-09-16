using DataModel;
using DataModel.DTO;
using DataModel.Security;
using DataModel.Service;
using GymSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GymSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;
        public HomeController(AppDbContext context,NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public IActionResult Login(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {

                ViewData["ReturnUrl"] = returnUrl;
            }

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var member = _context.Members.FirstOrDefault(m => m.MemberID == model.MemberID);
                if(member != null)
                {
                    if(member.IsActive == false)
                    {
                        //ModelState.AddModelError("", "�|���w�Q���ΡA���p���޲z��");
                        ViewData["ErrorMessage"] = "1";
                        return View(model);
                    }

                    if (PasswordHelper.Verify(model.MemberPassword, member))
                    {
                        // ���Ҧ��\
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, member.MemberID),
                            new Claim(ClaimTypes.Name, member.MemberName),
                            new Claim(ClaimTypes.Role, member.MemberRole),
                            new Claim("MemberRole", member.MemberRole) // �̧A�� MemberRole ���
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                        // �n�J�ɱN������T�ǻ��ܫe��
                        var notifications = await _notificationService.GetNotificationAsync(member);
                        if (notifications != null && notifications.Count > 0)
                        { TempData["NotifityList"] = JsonConvert.SerializeObject(notifications); }
                        TempData["FromLogin"] = "1";
                        // �n�J���\��A���w�V���Ӫ�����
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);


                        // ���w�V�쭺��
                        return RedirectToAction("Index", "Home");
                    }
                }
                // ���ҥ���
                //ModelState.AddModelError("", "�|�����s�b�Ϊ̱K�X���~");    
                ViewData["ErrorMessage"] = "2";
            }
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> NotifityList()
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = await _context.Members.FindAsync(memberId);
            if(member == null)
                return RedirectToAction("Login", "Home");
            var notifications = await _notificationService.GetNotificationAsync(member);
            return View(notifications);
        }

        [Authorize]
        public async Task<IActionResult> Logout(string? returnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home", new { returnUrl });
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult NotifitySetting()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? errorMessage = "�S���T��")
        {

            ViewData["ErrorMessage"] = errorMessage;
            return View();
        }
    }
}
