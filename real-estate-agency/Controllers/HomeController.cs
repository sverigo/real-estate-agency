using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Models;
using DataParser;
using System.Xml.Serialization;
using System.IO;


namespace real_estate_agency.Controllers
{
    public class HomeController : Controller
    {
        private RealEstateDBEntities db = new RealEstateDBEntities();
        string img, phone;

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
            ViewBag.Ads = currentAd.Reverse();
            return View();
        }

        public ActionResult Click()
        {
            //var result = DataCollector.CollectFromOLX(2);
            //foreach (var item in result)
            //{
            //    XmlSerializer formatter = new XmlSerializer(typeof(List<string>));
            //    using (StringWriter writer = new StringWriter())
            //    {
            //        formatter.Serialize(writer, item.Images.ToList());
            //        img = writer.ToString();
            //        //formatter.Serialize(writer, item.Phones.ToList());
            //        //phone = writer.ToString();
            //    }

            //    var newAd = new Ad()
            //    {
            //        Title = item.Title,
            //        Type = item.AdType,
            //        Address = item.Address,
            //        Area = item.Area,
            //        Author = item.AuthorName,
            //        Floors = item.Floor,
            //        FloorsCount = item.FloorCount,
            //        RoomsCount = item.RoomCount,
            //        Images = img, // Collection
            //        //Phone = phone, // Collection
            //        Value = item.Price,
            //        Details = item.Details
            //    };
            //    using (var dbRea = new RealEstateDBEntities())
            //    {
            //        //dbRea.Ads.Add(newAd);
            //        dbRea.Entry(newAd).State = System.Data.Entity.EntityState.Added;

            //        try
            //        {
            //            dbRea.SaveChanges();
            //        }
            //        catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            //        {

            //            throw;
            //        }

            //    }
            //}
            return View("Index", db.Ads);
        }

    }
}
