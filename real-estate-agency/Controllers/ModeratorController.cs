using System;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Infrastructure;
using System.Threading.Tasks;
using real_estate_agency.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using real_estate_agency.Models.ViewModels;
using real_estate_agency.Resources;

namespace real_estate_agency.Controllers
{
    [Authorize(Roles = PermissionDirectory.ADMINS + "," + PermissionDirectory.MODERATORS)]
    public class ModeratorController : Controller
    {
        AdsManager adsManager = new AdsManager();

        private AppRoleManager RoleManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>(); }
        }

        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        [Authorize(Roles = PermissionDirectory.MODERATORS)]
        public ActionResult ModeratorPanel()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RemoveAd(int adId, string returnUrl)
        {
            RemoveAdViewModel model = new RemoveAdViewModel
            {
                ReturnURL = returnUrl,
                AdId = adId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> RemoveAd(RemoveAdViewModel model)
        {
            Ad deletedAd = adsManager.FindById(model.AdId);
            AppUser user = await UserManager.FindByIdAsync(deletedAd.UserAuthorId);
            AppUser sender = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            string text = $"Ваше объявление {deletedAd.Title} было удалено. " +
                $"Причина: {model.Message}";

            Notifier notifier = new Notifier(UserManager);
            IdentityResult result = notifier.NotifyUser(user, text, sender);

            if (result.Succeeded)
            {
                adsManager.RemoveById(deletedAd.Id);
                return Redirect(model.ReturnURL);
            }
            else
                return View("Error", result.Errors);
        }

        [HttpGet]
        public PartialViewResult SearchUserPanel()
        {
            SearchUserFilter filter = new SearchUserFilter(UserManager, RoleManager);
            SearchUserViewModel model = new SearchUserViewModel();
            model.ResultUsers = filter.SearchUsers(model);
            return PartialView(model);
        }

        [HttpPost]
        public PartialViewResult SearchUser(SearchUserViewModel model)
        {
            SearchUserFilter filter = new SearchUserFilter(UserManager, RoleManager);
            model.ResultUsers = filter.SearchUsers(model);
            return PartialView("SearchUserPanel", model);
        }

        [HttpGet]
        public ActionResult BlockUser(string id)
        {
            BlockUserViewModel model = new BlockUserViewModel
            {
                UserId = id
            };
            ViewBag.ReturnUrl = Request.UrlReferrer.ToString();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> BlockUser(BlockUserViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = UserManager.FindById(model.UserId);
                if (user == null)
                    return View("Error", new string[] { Resource.ModeratorValidator1 });

                TimeSpan duration = TimeSpan.FromDays(0);
                switch (model.Duration)
                {
                    case BlockDuration.Hour:
                        duration = TimeSpan.FromHours(1);
                        break;
                    case BlockDuration.Day:
                        duration = TimeSpan.FromDays(1);
                        break;
                    case BlockDuration.Week:
                        duration = TimeSpan.FromDays(7);
                        break;
                    case BlockDuration.Month:
                        duration = TimeSpan.FromDays(30);
                        break;
                    case BlockDuration.Forever:
                        duration = TimeSpan.FromDays(365 * 100);
                        break;
                }
                user.LockoutEndDateUtc = DateTime.UtcNow + duration;

                IdentityResult result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    AppUser sender = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    Email.EmailSender.SendUserBlockedMail(user, model.Duration, model.Message, sender);
                    return Redirect(returnUrl);
                }
                else
                    return View("Error", result.Errors);
            }

            return View(model);
        }

        public async Task<ActionResult> DeblockUser(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user == null)
                return View("Error", new string[] { Resource.ModeratorValidator1 });

            user.LockoutEndDateUtc = null;
            IdentityResult result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
                return Redirect(Request.UrlReferrer.ToString());
            else
                return View("Error", result.Errors);
        }
    }
}