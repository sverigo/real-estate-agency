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

namespace real_estate_agency.Controllers
{
    public class HomeController : Controller
    {
        AdsManager adsManager = new AdsManager();

        public ActionResult Index(int? page)
        {
            var adsList = adsManager.AllAds.Reverse();

            int pageNumber = page ?? 1;
            int pageSize = 10;

            return View(adsList.ToPagedList(pageNumber, pageSize));
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
