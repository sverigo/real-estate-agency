using Microsoft.AspNet.Identity;
using real_estate_agency.Infrastructure;
using real_estate_agency.Models;
using real_estate_agency.Models.ViewModels;
using System;
using System.IO;
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
            var phones = HttpContext.Request.Form["Phone"];
            ad.Phone = String.Join("|", phones.Split(','));
            ad.PrevImage = "~/Content/images/nophoto.png";
            
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                /*add check for extensions for images*/
                if (upload != null && Path.GetExtension(upload.FileName) == ".jpg")
                {

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
                    ad.PrevImage = "~/Content/images/" + fileName;
                    upload.SaveAs(Server.MapPath(ad.PrevImage));
                }
            }
            adsManager.AddNewAd(ad);
            if (String.IsNullOrEmpty(returnUrl))
            {
                return Redirect("/Home/Index/");
            }
            return Redirect(returnUrl);
        }

        public ActionResult Edit(int? id, string fromDetailsUrl)
        {
            Ad ad = adsManager.FindById(id);
            string userId = User.Identity.GetUserId();

            if (ad == null)
                return View("Error", new string[] { "Объявление с указанным id не существует!" });
            else if (!PermissionDirectory.IsOwnerOfAd(User, ad))
                return View("Error", new string[] { "У вас нет прав редактировать это объявление!" });
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
            var phones = HttpContext.Request.Form["Phone"];
            ad.Phone = String.Join("|", phones.Split(','));
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                /*add check for extensions for images*/
                if (upload != null && Path.GetExtension(upload.FileName) == ".jpg")
                {

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
                    ad.PrevImage = "~/Content/images/" + fileName;
                    upload.SaveAs(Server.MapPath(ad.PrevImage));
                }
            }
            adsManager.Edit(ad);
            if (!string.IsNullOrEmpty(fromDetailsUrl))
                return RedirectToAction("Details", new { id = ad.Id, fromDetailsUrl = fromDetailsUrl });
            else if (String.IsNullOrEmpty(returnUrl))
            {
                return Redirect("/Home/Index/");
            }
            else {
                return Redirect(returnUrl);
            }
            
        }

        public ActionResult Remove(int id, string returnUrl)
        {
            Ad ad = adsManager.FindById(id);
            string userId = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Request.UrlReferrer.ToString();

            if (ad == null)
                return View("Error", new string[] { "Объявление с указанным id не существует!" });
            else if (!PermissionDirectory.UserCanDeleteAd(User, ad))
                return View("Error", new string[] { "У вас нет прав редактировать это объявление!" });
            else if (PermissionDirectory.IsOwnerOfAd(userId, ad))
                adsManager.RemoveById(id);
            else if (PermissionDirectory.IsAdmin(User) || PermissionDirectory.IsModerator(User))
            {
                if (string.IsNullOrEmpty(ad.UserAuthorId))
                    adsManager.RemoveById(id);
                else
                    return RedirectToAction("RemoveAd", "Moderator", new { adId = ad.Id, returnUrl = returnUrl });
            }

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
        
        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult Marks(int id)
        {
            if (User.Identity.IsAuthenticated == false)
                return new EmptyResult();
            string userId = User.Identity.GetUserId();
            Ad ad = adsManager.FindById(id);
            MarksViewModel model = new MarksViewModel
            {
                AdId = id,
                IsMarkedByUser = ad.UsersMarked.Select(user => user.AppUserId).Contains(userId)
            };

            return PartialView(model);
        }

        public PartialViewResult SetMark(int id)
        {
            string userId = User.Identity.GetUserId();
            adsManager.SetMark(id, userId);
            
            Ad ad = adsManager.FindById(id);
            MarksViewModel model = new MarksViewModel
            {
                AdId = id,
                IsMarkedByUser = ad.UsersMarked.Select(user => user.AppUserId).Contains(userId)
            };

            return PartialView("Marks", model);
        }

        public PartialViewResult RemoveMark(int id)
        {
            string userId = User.Identity.GetUserId();
            adsManager.RemoveMark(id, userId);

            Ad ad = adsManager.FindById(id);
            MarksViewModel model = new MarksViewModel
            {
                AdId = id,
                IsMarkedByUser = ad.UsersMarked.Select(user => user.AppUserId).Contains(userId)
            };

            return PartialView("Marks", model);
        }
    }
}