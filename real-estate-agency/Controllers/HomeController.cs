using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using real_estate_agency.Models;
using DataParser;
using System.Xml.Serialization;
using System.IO;
using PagedList;
using System.Data.Entity;
using real_estate_agency.Infrastructure;
using real_estate_agency.Models.ViewModels;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace real_estate_agency.Controllers
{
    public class HomeController : Controller
    {
        AdsManager adsManager = new AdsManager();

        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        public ActionResult Index()
        {
            AppUser user = UserManager.FindById(User?.Identity.GetUserId() ?? "");
            if (user != null)
            {
                int notifCount = user.Notifications.Where(n => !n.Seen).Count();
                ViewBag.NotifCount = notifCount > 0 ? " +" + notifCount : "";
            }
            else
                ViewBag.NotifCount = "";
            return View();
        }

        public PartialViewResult AdFilter(MainPageViewModel model)
        {
            var allAds = adsManager.AllAds.Reverse();
            int pageSize = 15;

            if (model == null)
                model = new MainPageViewModel();

            if (model.Page == 0)
                model.Page = 1;

            var options = new FilterOptions
            {
                Currency = model.Currency,
                SortType = model.SortType,
                FlatRentType = model.FlatRentType,
                MaxArea = model.MaxArea ?? 0,
                MinArea = model.MinArea ?? 0,
                MaxFloor = model.MaxFloor ?? 0,
                MinFloor = model.MinFloor ?? 0,
                MaxQuantity = model.MaxQuantity ?? 0,
                MinQuantity = model.MinQuantity ?? 0,
                MaxRoomsCount = model.MaxRoomsCount ?? 0,
                MinRoomsCount = model.MinRoomsCount ?? 0
            };

            allAds = options.getQuantityFilter(allAds);
            allAds = options.getRoomsFilter(allAds);
            allAds = options.getFloorFilter(allAds);
            allAds = options.getSortedAds(allAds);
            allAds = options.getFlatRentAds(allAds);

            model.PagedListModel = allAds.ToPagedList(model.Page, pageSize);
            return PartialView(model);
        }

        public ActionResult Click()
        {
            try
            {
                adsManager.LoadAds();
            }
            catch(Exception ex)
            {
                //handle exception...
            }
            return RedirectToAction("Index");
        }
                
        [HttpGet]
        public JsonResult GetPhoneAjax(int id)
        {
            List<string> phones = adsManager.GetPhonesForAd(id);
            return Json(phones, JsonRequestBehavior.AllowGet);
        }
    }
}
