using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Infrastructure;
using Microsoft.AspNet.Identity.Owin;

namespace real_estate_agency.Controllers
{
    [Authorize(Roles = PermissionDirectory.ADMINS)]
    public class AdminController : Controller
    {

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
            return View();
        }


    }
}