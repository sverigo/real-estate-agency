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

        public ActionResult Add()
        {
            return View();
        }

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
            //IEnumerable<Ad> ads = db.GetItems();
            //var currentAd = (from i in ads where i.Id == id select i).FirstOrDefault();

            //List<string> numbers = null;

            //List<string> phonesList = null;

            //using (StringReader reader = new StringReader(currentAd.Phone))
            //{
            //    phonesList = (List<string>)formatter.Deserialize(reader);
            //}

            //if (phonesList.Count == 0)
            //{
            //    var parserModel = new DataParser.Models.AdvertismentModel(currentAd.AdUrl);
            //    numbers = parserModel?.CollectPhones()?.ToList();

            //    if (numbers == null)
            //    {
            //        numbers = new List<string>() { "" };
            //        return Json(null, JsonRequestBehavior.AllowGet);
            //    }

            //    string xmlPhones = string.Empty;
            //    using (StringWriter writer = new StringWriter())
            //    {
            //        formatter.Serialize(writer, numbers);
            //        xmlPhones = writer.ToString();
            //    }

            //    currentAd.Phone = xmlPhones;

            //    RealEstateDBContext dbb = new RealEstateDBContext();
            //    dbb.Entry(currentAd).State = EntityState.Modified;
            //    dbb.SaveChanges();
            //}
            //else
            //{
            //    numbers = phonesList;
            //}
            List<string> phones = adsManager.GetPhonesForAd(id);
            return Json(phones, JsonRequestBehavior.AllowGet);
        }
    }
}
