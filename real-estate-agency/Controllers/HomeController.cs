using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using real_estate_agency.Models;
using PagedList;
using real_estate_agency.Infrastructure;
using real_estate_agency.Models.ViewModels;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Security.Principal;

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
            if(User?.Identity.IsAuthenticated ?? false)
            {
                try
                {
                    UserStatusDirectory userStatDirect = new UserStatusDirectory();
                    UserStatus status = userStatDirect.GetUserStatus(User);
                    if (status.isBlocked)
                        return RedirectToAction("Logout", "Account", new { lockoutTime = status.lockoutTime });
                    if (!string.IsNullOrEmpty(status.NotificationsCountSign))
                        ViewBag.NotifCount = status.NotificationsCountSign;
                }
                catch (Exception ex)
                {
                    return View("Error", new string[] { ex.Message });
                }
            }
            return View();
        }

        public PartialViewResult Ad(MainPageViewModel model)
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
                RentType = model.RentType,
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
