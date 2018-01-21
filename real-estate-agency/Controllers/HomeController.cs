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

namespace real_estate_agency.Controllers
{
    public class HomeController : Controller
    {
        AdManager db = new AdManager();
        XmlSerializer formatter = new XmlSerializer(typeof(List<string>));

        public ActionResult Index(int? page)
        {
            var Items = db.GetItems().Reverse().ToList();

            int pageNumber = page ?? 1;
            int pageSize = 10;

            return View(Items.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(int? id)
        {
            IEnumerable<Ad> ads = db.GetItems();
            if (id == null)
                return Redirect("/Home/Index");

            var currentAd = from i in ads where i.Id == id select i;

            return View(currentAd.FirstOrDefault());
        }

        public ActionResult Click()
        {
            var result = DataCollector.CollectFromOLX(1);
            var preparedEntities = result.Select(adModel =>
            {
                string xmlImages = string.Empty;
                string xmlPhones = string.Empty;
                using (StringWriter writer = new StringWriter())
                {
                    formatter.Serialize(writer, adModel.Images.ToList());
                    xmlImages = writer.ToString();
                }
                if (adModel.Phones == null)
                {
                    using (StringWriter phoneWriter = new StringWriter())
                    {
                        formatter.Serialize(phoneWriter, new List<string>());
                        xmlPhones = phoneWriter.ToString();
                    }
                }
                    
                var newAd = new Ad()
                {
                    Title = adModel.Title,
                    Type = adModel.AdType,
                    Address = adModel.Address,
                    Area = adModel.Area,
                    Author = adModel.AuthorName,
                    Floors = adModel.Floor,
                    FloorsCount = adModel.FloorCount,
                    RoomsCount = adModel.RoomCount,
                    Images = xmlImages,
                    Phone = xmlPhones,
                    Value = adModel.Price,
                    Details = adModel.Details,
                    PrevImage = adModel.PreviewImg,
                    AdUrl = adModel.Url
                };

                return newAd;
            });

            using (var dbRea = new RealEstateDBEntities())
            {
                dbRea.Ads.AddRange(preparedEntities);
                try
                {
                    dbRea.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    throw;
                }
            }

            return Redirect("/Home/Index");
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Ad ad)
        {
            RealEstateDBEntities dbb = new RealEstateDBEntities();
            List<string> phones = new List<string>();
            phones.Add(ad.Phone);
            var image = ad.Images;

            try
            {
                using (StringWriter writer = new StringWriter())
                {
                    formatter.Serialize(writer, phones);
                    ad.Phone = writer.ToString();
                }
                using (StringWriter writer = new StringWriter())
                {
                    formatter.Serialize(writer, image);
                    ad.Images = writer.ToString();
                }
            }
            catch {
                throw;
            }
            dbb.Entry(ad).State = EntityState.Added;
            dbb.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            IEnumerable<Ad> ads = db.GetItems();
            if (id == null)
                return Redirect("/Home/Index");

            var currentAd = from i in ads where i.Id == id select i;

            return View(currentAd.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult Edit(Ad ad)
        {
            RealEstateDBEntities dbb = new RealEstateDBEntities();
            List<string> phones = new List<string>();
            phones.Add(ad.Phone);
            var image = ad.Images;
            try
            {
                using (StringWriter writer = new StringWriter())
                {
                    formatter.Serialize(writer, phones);
                    ad.Phone = writer.ToString();
                }
                using (StringWriter writer = new StringWriter())
                {
                    formatter.Serialize(writer, image);
                    ad.Images = writer.ToString();
                }
            }
            catch {
                throw;
            }
            dbb.Entry(ad).State = EntityState.Modified;
            dbb.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Remove(int id)
        {
            RealEstateDBEntities db = new RealEstateDBEntities();
            Ad ad = new Ad { Id = id };
            db.Entry(ad).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetPhoneAjax(int id)
        {
            IEnumerable<Ad> ads = db.GetItems();
            var currentAd = (from i in ads where i.Id == id select i).FirstOrDefault();

            List<string> numbers = null;

            List<string> phonesList = null;
            
            using (StringReader reader = new StringReader(currentAd.Phone))
            {
                phonesList = (List<string>)formatter.Deserialize(reader);
            }

            if(phonesList.Count == 0)
            {
                var parserModel = new DataParser.Models.AdvertismentModel(currentAd.AdUrl);
                numbers = parserModel.CollectPhones().ToList();

                string xmlPhones = string.Empty;
                using (StringWriter writer = new StringWriter())
                {
                    formatter.Serialize(writer, numbers);
                    xmlPhones = writer.ToString();
                }

                currentAd.Phone = xmlPhones;

                RealEstateDBEntities dbb = new RealEstateDBEntities();
                dbb.Entry(currentAd).State = EntityState.Modified;
                dbb.SaveChanges();
            }
            else
            {
                numbers = phonesList;
            }
            
            return Json(numbers.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}
