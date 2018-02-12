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
        
        public ActionResult Add()
        {
            ViewBag.ReturnUrl = Request.UrlReferrer.ToString();
            return View();
        }

        [HttpPost]
        public ActionResult Add(Ad ad, string returnUrl)
        {
            adsManager.AddNewAd(ad);
            return Redirect(returnUrl);
        }

        public ActionResult Edit(int? id, string fromDetailsUrl)
        {
            Ad ad = adsManager.FindById(id);
            if (ad == null)
                return View("Error", new string[] { "Объявление с указанным id не существует!" });
            else
            {
                if(!string.IsNullOrEmpty(fromDetailsUrl))
                {
                    ViewBag.FromDetailsUrl = fromDetailsUrl;
                    ViewBag.ReturnUrl = null;
                }
                else
                    ViewBag.ReturnUrl = Request.UrlReferrer.ToString();
                return View(ad);
            }
        }

        [HttpPost]
        public ActionResult Edit(Ad ad, string returnUrl, string fromDetailsUrl)
        {
            adsManager.Edit(ad);
            if (!string.IsNullOrEmpty(fromDetailsUrl))
                return RedirectToAction("Details", new { id = ad.Id, fromDetailsUrl = fromDetailsUrl });
            else
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
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Request.UrlReferrer.ToString();
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public ActionResult Details(int? id, string fromDetailsUrl)
        {
            var currentAd = adsManager.FindById(id);
            if (currentAd == null)
                return View("Error", new string[] { "Объявление с указанным id не существует!" });

            if (!string.IsNullOrEmpty(fromDetailsUrl))
                ViewBag.ReturnUrl = fromDetailsUrl;
            else
            {
                ViewBag.FromDetailsUrl = Request.UrlReferrer.ToString();
                ViewBag.ReturnUrl = Request.UrlReferrer.ToString();
            }
            return View(currentAd);
        }
    }
}