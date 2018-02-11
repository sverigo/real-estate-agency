using real_estate_agency.Infrastructure;
using real_estate_agency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace real_estate_agency.Controllers
{
    [Authorize]
    public class AdsController : Controller
    {
        AdsManager adsManager = new AdsManager();
        
        public ActionResult Add(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Add(Ad ad, string returnUrl)
        {
            adsManager.AddNewAd(ad);
            return Redirect(returnUrl);
        }

        public ActionResult Edit(int? id, string returnUrl)
        {
            Ad ad = adsManager.FindById(id);
            if (ad == null)
                return View("Error", new string[] { "Объявление с указанным id не существует!" });
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(ad);
            }
        }

        [HttpPost]
        public ActionResult Edit(Ad ad, string returnUrl)
        {
            adsManager.Edit(ad);
            return Redirect(returnUrl);
        }

        public ActionResult Remove(int id, string returnUrl)
        {
            try
            {
                adsManager.RemoveById(id);
            }
            catch (Exception ex)
            {
                //handle exception here
            }
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public ActionResult Details(int? id, string returnUrl)
        {
            var currentAd = adsManager.FindById(id);
            if (currentAd == null)
                return View("Error", new string[] { "Объявление с указанным id не существует!" });
            
            ViewBag.ReturnUrl = returnUrl;
            return View(currentAd);
        }
    }
}