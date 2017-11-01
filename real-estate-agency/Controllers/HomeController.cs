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
        AdManager db = new AdManager();
        private string img, phone;
        XmlSerializer formatter = new XmlSerializer(typeof(List<string>));
        List<string> test = new List<string>();

        public ActionResult Index()
        {
            var Items = db.GetItems();
            var currentImg = from i in Items select i.Images;
            return View(Items);
        }

        public ActionResult Details(int? id)
        {
            IEnumerable<Ad> ads = db.GetItems();
            if (id == null)
            {
                return Redirect("/Home/Index");
            }
            var currentAd = from i in ads
                            where i.Id == id
                            select i;
            ViewBag.Ads = currentAd;
            var currentImg = from i in ads where i.Id == id select i.Images;
            using (StringReader reader = new StringReader(currentImg.FirstOrDefault()))
            {
                test = (List<string>)formatter.Deserialize(reader);
            }
            ViewBag.ImgItems = test;
            return View();
        }

        public ActionResult Click()
        {
            var result = DataParser.DataParser.CollectFromOLX(1);
            foreach (var item in result)
            {
                using (StringWriter writer = new StringWriter())
                {
                    formatter.Serialize(writer, item.Images.ToList());
                    img = writer.ToString();
                    //formatter.Serialize(writer, item.Phones.ToList());
                    //phone = writer.ToString();
                }


                
                using (StringReader reader = new StringReader(img))
                {
                    test = (List<string>)formatter.Deserialize(reader);
                }

                var newAd = new Ad()
                {
                    Title = item.Title,
                    Type = item.AdType,
                    Address = item.Address,
                    Area = item.Area,
                    Author = item.AuthorName,
                    Floors = item.Floor,
                    FloorsCount = item.FloorCount,
                    RoomsCount = item.RoomCount,
                    Images = img, // Collection
                    //Phone = phone, // Collection
                    Value = item.Price,
                    Details = item.Details
                };
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
