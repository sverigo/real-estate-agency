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

        public ActionResult Details(int? id)
        {
            if (id == null)
                return Redirect("/Home/Index");

            var currentAd = adsManager.AllAds.Where(ad => ad.Id == id).FirstOrDefault();
            
            return View(currentAd);
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

        [Authorize]
        public ActionResult Add()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Add(Ad ad)
        {
            adsManager.AddNewAd(ad);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            Ad ad = adsManager.FindById(id);
            if (ad == null)
                return Redirect("/Home/Index");
            else
                return View(ad);
        }

        [HttpPost]
        public ActionResult Edit(Ad ad)
        {
            adsManager.Edit(ad);
            return RedirectToAction("Index");
        }

        public ActionResult Remove(int id)
        {
            try
            {
                adsManager.RemoveById(id);
            }
            catch(Exception ex)
            {
                //handle exception here
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
