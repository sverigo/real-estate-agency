using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Infrastructure;
using System.Threading.Tasks;
using real_estate_agency.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using real_estate_agency.Models.ViewModels;

namespace real_estate_agency.Controllers
{
    [Authorize(Roles = PermissionDirectory.ADMINS+","+PermissionDirectory.MODERATORS)]
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
    }
}