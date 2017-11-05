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

        public ActionResult Index(int? page)
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
            }
            catch
            {
                throw;
            }

            ViewBag.Ads = currentAd;
            ViewBag.ImgItems = imgList;
            ViewBag.PhoneItems = phoneList;
            return View();
        }

        public ActionResult Click()
        {
            var result = DataCollector.CollectFromOLX(5);
            var preparedEntities = result.Select(adModel =>
            {
                string xmlImages = string.Empty;
                string xmlPhones = string.Empty;
                using (StringWriter writer = new StringWriter())
                {
                    formatter.Serialize(writer, adModel.Images.ToList());
                    xmlImages = writer.ToString();
                    //formatter.Serialize(writer, item.Phones.ToList());
                    //phone = writer.ToString();
                }
                using (StringWriter phoneWriter = new StringWriter())
                {
                    formatter.Serialize(phoneWriter, adModel.Phones.ToList());
                    xmlPhones = phoneWriter.ToString();
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
                    Images = xmlImages, // Collection
                    Phone = xmlPhones, // Collection
                    Value = adModel.Price,
                    Details = adModel.Details,
                    PrevImage = adModel.PreviewImg
                };

                return newAd;
            });

            using (var dbRea = new RealEstateDBEntities())
            {
                dbRea.Ads.AddRange(preparedEntities);
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

            return View();
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
            catch { throw; }
            dbb.Entry(ad).State = EntityState.Added;
            dbb.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
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
            }
            catch
            {
                throw;
            }
            ViewBag.Ads = currentAd;
            ViewBag.ImgItems = imgList;
            ViewBag.PhoneItems = phoneList;
            return View();
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
            catch { throw; }
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
    }
}
