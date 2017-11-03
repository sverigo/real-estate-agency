using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Models;
using DataParser;
using System.Xml.Serialization;
using System.IO;
using PagedList;
using System.Data.Entity;

namespace real_estate_agency.Controllers
{
    public class HomeController : Controller
    {
        AdManager db = new AdManager();
        private string img, phone;
        XmlSerializer formatter = new XmlSerializer(typeof(List<string>));
        List<string> imgList = new List<string>();
        List<string> phoneList = new List<string>();

        public ActionResult Index(int ?page)
        {
            var Items = db.GetItems().ToList();
            var currentImg = from i in Items select i.Images;

            int pageNumber = page ?? 1;
            int pageSize = 10;

            return View(Items.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(int? id)
        {
            IEnumerable<Ad> ads = db.GetItems();
            if (id == null)
            {
                return Redirect("/Home/Index");
            }

            var currentAd = from i in ads where i.Id == id select i;
            var currentImg = from i in ads where i.Id == id select i.Images;
            var currentPhone = from j in ads where j.Id == id select j.Phone;

            using (StringReader reader = new StringReader(currentImg.FirstOrDefault()))
            {
                imgList = (List<string>)formatter.Deserialize(reader);
            }
            try
            {
                using (StringReader reader = new StringReader(currentPhone.FirstOrDefault()))
                {
                    phoneList = (List<string>)formatter.Deserialize(reader);
                }
            } catch {
                throw;
            }

            ViewBag.Ads = currentAd;
            ViewBag.ImgItems = imgList;
            ViewBag.PhoneItems = phoneList;
            return View();
        }

        public ActionResult Click()
        {
            var result = DataCollector.CollectFromOLX(2);
            foreach (var item in result)
            {
                using (StringWriter writer = new StringWriter())
                {
                    formatter.Serialize(writer, item.Images.ToList());
                    img = writer.ToString();
                }
                using (StringWriter phoneWriter = new StringWriter())
                {
                    formatter.Serialize(phoneWriter, item.Phones.ToList());
                    phone = phoneWriter.ToString();
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
                    Phone = phone, // Collection
                    Value = item.Price,
                    Details = item.Details
                };
                using (var dbRea = new RealEstateDBEntities())
                {
                    dbRea.Ads.Add(newAd);
                    //dbRea.Entry(newAd).State = System.Data.Entity.EntityState.Added;
                    try
                    {
                        dbRea.SaveChanges();
                    }  
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        throw;
                    }
                }
            }
            return View();
        }

        public ActionResult Remove(int id)
        {
            RealEstateDBEntities db = new RealEstateDBEntities();
            Ad ad = new Ad { Id = id };
            db.Entry(ad).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
