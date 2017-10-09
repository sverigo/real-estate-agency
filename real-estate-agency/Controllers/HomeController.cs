using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Models;

namespace real_estate_agency.Controllers
{
    public class HomeController : Controller
    {
        private RealEstateDBEntities db = new RealEstateDBEntities();

        public ActionResult Index()
        {
            var Items = db.Ads;
            return View(Items);
        }

        public ActionResult Details(int id)
        {
            IEnumerable<Ad> ads = db.Ads;
            var currentAd = from i in ads
                            where i.Id == id
                            select i;
            ViewBag.Ads = currentAd;
            return View();
        }
    }
}