using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Models;
using DataParser;


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

        public ActionResult Details(int? id)
        {
            IEnumerable<Ad> ads = db.Ads;
            if (id == null)
            {
                return Redirect("/Home/Index");
            }
            var currentAd = from i in ads
                            where i.Id == id
                            select i;
            ViewBag.Ads = currentAd;
            return View();
        }

        public ActionResult Click()
        {
            var result = DataParser.DataParser.CollectFromOLX(1);
            foreach (var item in result)
            {
                var newAd = new Ad();
                newAd.Title = item.Title;
                newAd.Type = item.AdType;
                newAd.City = "Днепр";
                newAd.Address = item.Address;
                newAd.Area = item.Area;
                newAd.Author = item.AuthorName;
                newAd.Floors = item.Floor;
                newAd.FloorsCount = item.FloorCount;
                newAd.RoomsCount = item.RoomCount;
                //newAd.Images = item.Images;
                //newAd.Phone = item.Phones;
                newAd.Value = item.Price;
                newAd.Details = item.Details;
                using (var dbRea = new RealEstateDBEntities())
                {
                    dbRea.Ads.Add(newAd);
                    //dbRea.Entry(newAd).State = System.Data.Entity.EntityState.Added;
                    dbRea.SaveChanges();
                }
            }
            return View();
        }

    }
}
