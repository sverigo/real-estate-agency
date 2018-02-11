using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using real_estate_agency.Infrastructure;
using real_estate_agency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
            return View(user);
        }

        public PartialViewResult MyAds()
        {
            IEnumerable<Ad> ads = adsManager.GetAdsByUserAthorId(User.Identity.GetUserId());
            return PartialView(ads);
        }
    }
}