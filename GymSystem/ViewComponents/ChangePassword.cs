using Microsoft.AspNetCore.Mvc;
using DataModel.DTO;

namespace GymSystem.ViewComponents
{
    public class ChangePasswordViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string memberId)
        {
            var model = new ChangePasswordModel { MemberID = memberId };
            return View(model);
        }
    }
}