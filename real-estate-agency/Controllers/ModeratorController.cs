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
        public ActionResult RemoveAd(int adId)
        {
            return View();
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