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
        XmlSerializer formatter = new XmlSerializer(typeof(List<string>));

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
            var currentPhone = from j in ads where j.Id == id select j.Phone;

            List<string> imgList = new List<string>();
            List<string> phoneList = new List<string>();
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
            } catch
            {
                throw;
            }

            
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
                    Details = adModel.Details
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
    }
}
