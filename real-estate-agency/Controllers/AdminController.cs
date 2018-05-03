using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using real_estate_agency.Models.ViewModels;
using System.Threading.Tasks;
using real_estate_agency.Models;
using Microsoft.AspNet.Identity;
using real_estate_agency.Resources;

namespace real_estate_agency.Controllers
{
    [Authorize(Roles = UserStatusDirectory.Roles.ADMINS)]
    public class AdminController : Controller
    {
        AdsManager adManager = new AdsManager();
        PaymentManager payManager = new PaymentManager();
        
        private AppRoleManager RoleManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>(); }
        }

        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        // GET: Admin
        public ActionResult AdminPanel()
        {
            AppRole moderRole = RoleManager.FindByName(UserStatusDirectory.Roles.MODERATORS);
            List<string> modersIds = moderRole.Users.Select(u => u.UserId).ToList();
            List<AppUser> moders = UserManager.Users.Where(u => modersIds.Contains(u.Id)).ToList();
            return View(moders);
        }

        [HttpGet]
        public ActionResult CreateModerator()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateModerator(UserRegisterViewModel userInfo)
        {
            if (ModelState.IsValid)
            {
                if (userInfo.ConfirmPassword != userInfo.Password)
                    ModelState.AddModelError("", Resource.PasswordValidator1);
                else
                {
                    AppUser user = new AppUser
                    {
                        UserName = userInfo.Login,
                        Name = userInfo.Name,
                        Email = userInfo.Email,
                        EmailConfirmed = true,
                        LockoutEnabled = false
                    };

                    IdentityResult creationResult = await UserManager.CreateAsync(user, userInfo.Password);
                    IdentityResult roleResult = await UserManager.AddToRoleAsync(user.Id, UserStatusDirectory.Roles.MODERATORS);
                    if (creationResult.Succeeded && roleResult.Succeeded)
                        return RedirectToAction("AdminPanel");
                    else
                    {
                        creationResult.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
                        roleResult.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
                    }
                }
            }
            return View(userInfo);
        }

        public async Task<ActionResult> DeleteModerator(string id)
        {
            AppUser moder = await UserManager.FindByIdAsync(id);
            if (moder == null)
                return View("Error", new string[] { Resource.AdminValidator1 });

            //remove moder's marks
            List<Ad> ads = adManager.GetMarkedAdsByUserAthorId(moder.Id).ToList();
            ads.ForEach(ad => adManager.RemoveMark(ad.Id, moder.Id));

            //remove moder's ads
            ads = adManager.GetAdsByUserAthorId(moder.Id).ToList();
            ads.ForEach(ad => adManager.RemoveById(ad.Id));

            IdentityResult result = await UserManager.DeleteAsync(moder);
            if (result.Succeeded)
                return RedirectToAction("AdminPanel");
            else
                return View("Error", result.Errors);
        }
        
        public ActionResult PricesList()
        {
            return PartialView("PricesList", payManager.Prices);
        }

        [HttpPost]
        public ActionResult AddPrice(int days, decimal amount)
        {
            payManager.AddPrice(new Price
            {
                Amount = amount,
                Days = days
            });
            return PartialView("PricesList", payManager.Prices);
        }
        
        public ActionResult DeletePrice(int id)
        {
            payManager.DeletePriceById(id);
            return PartialView("PricesList", payManager.Prices);
        }
    }
}