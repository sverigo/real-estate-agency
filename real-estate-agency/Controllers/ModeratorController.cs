using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Infrastructure;

namespace real_estate_agency.Controllers
{
    [Authorize(Roles = PermissionDirectory.ADMINS)]
    [Authorize(Roles = PermissionDirectory.MODERATORS)]
    public class ModeratorController : Controller
    {
        // GET: Moderator
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RemoveAd(int adId)
        {
            return View();
        }
    }
}