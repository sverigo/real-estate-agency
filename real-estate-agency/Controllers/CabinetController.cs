using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using real_estate_agency.Infrastructure;
using real_estate_agency.Models;
using real_estate_agency.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Resources;

namespace real_estate_agency.Controllers
{
    [Authorize]
    public class CabinetController : Controller
    {
        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        AdsManager adsManager = new AdsManager();

        // GET: Cabinet
        public async Task<ActionResult> Index()
        {
            AppUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int newNotificationsCount = user.Notifications.Where(n => !n.Seen).Count();
            ViewBag.NotifCount = newNotificationsCount > 0 ? " +" + newNotificationsCount : "" ;
            return View(user);
        }

        public PartialViewResult MyAds()
        {
            IEnumerable<Ad> ads = adsManager.GetAdsByUserAthorId(User.Identity.GetUserId());
            return PartialView(ads);
        }

        public PartialViewResult MyMarks()
        {
            IEnumerable<Ad> markedAds = adsManager.GetMarkedAdsByUserAthorId(User.Identity.GetUserId());
            return PartialView(markedAds);
        }

        public ActionResult MyNotifications()
        {
            AppUser user = UserManager.FindById(User.Identity.GetUserId());
            Notifier notifier = new Notifier(UserManager);
            IdentityResult result = notifier.SetNotificationsSeen(user);
            if (result == null || result.Succeeded)
            {
                IEnumerable<Notification> notifications = user.Notifications.ToList();
                return PartialView(notifications.Reverse());
            }
            else
                return View("Error", result.Errors);
        }
        
        public PartialViewResult ProfileSettings()
        {
            return PartialView("ProfileSettings", User.Identity.GetUserId());
        }

        public PartialViewResult ChangeProfile(string userId)
        {
            AppUser user = UserManager.FindById(userId);
            ChangeProfileViewModel model = new ChangeProfileViewModel
            {
                IdProfile = user.Id,
                Name = user.Name,
                Login = user.UserName
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeProfile(ChangeProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindByIdAsync(model.IdProfile);
                user.Name = model.Name;
                user.UserName = model.Login;
                IdentityResult updateResult = await UserManager.UpdateAsync(user);
                if (updateResult.Succeeded)
                {
                    ViewBag.SuccessMessage = Resource.SavingValidator1;
                    return PartialView("ChangeProfile", model);
                }
                else
                    updateResult.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
            }

            return PartialView("ChangeProfile", model);
        }

        public PartialViewResult ChangePassword(string userId)
        {
            AppUser user = UserManager.FindById(userId);
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                IdPassword = user.Id
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewPassword != model.ConfirmNewPassword)
                    ModelState.AddModelError("", Resource.PasswordValidator2);
                else
                {
                    AppUser existingUser = await UserManager.FindByIdAsync(model.IdPassword);
                    AppUser user = await UserManager.FindAsync(existingUser.UserName, model.Password);
                    if (user == null)
                        ModelState.AddModelError("", Resource.PasswordValidator3);
                    else
                    {
                        IdentityResult result = await UserManager.PasswordValidator.ValidateAsync(model.NewPassword);
                        if (result.Succeeded)
                        {
                            user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.NewPassword);
                            IdentityResult updateResult = await UserManager.UpdateAsync(user);

                            if (updateResult.Succeeded)
                            {
                                ViewBag.SuccessMessage = Resource.SavingValidator1;
                                return PartialView("ChangePassword", model);
                            }
                            else
                                result.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
                        }
                        else
                            result.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
                    }
                }
            }
            return PartialView("ChangePassword", model);
        }
    }
}