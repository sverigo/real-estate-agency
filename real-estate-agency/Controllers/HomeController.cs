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

namespace real_estate_agency.Controllers
{
    public class HomeController : Controller
    {
        AdsManager adsManager = new AdsManager();

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult AdFilter(MainPageViewModel model)
        {
            var allAds = adsManager.AllAds.Reverse();
            int pageSize = 10;

            if (model == null)
                model = new MainPageViewModel();

            if (model.Page == 0)
                model.Page = 1;

            var options = new FilterOptions
            {
                Currency = model.Currency,
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
