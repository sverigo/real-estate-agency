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
        AdsManager forImages = new AdsManager();

        public ActionResult Add()
        {
            if (Request.UrlReferrer != null)
            {
                ViewBag.ReturnUrl = Request.UrlReferrer.ToString();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Add(Ad ad, string returnUrl)
        {
            List<string> phonesList = new List<string>();
            foreach (string field in Request.Form)
            {
                if (field.Contains("Phone"))
                {
                    phonesList.Add(Request.Form[field]);
                }
            }
            ad.Phone = String.Join("|", phonesList);
            
            ad.PrevImage = "~/Content/images/nophoto.png";
            ad.Images = "";

            var upload = Request.Files["uPrevImage"];
            if (upload != null && upload.ContentType.Contains("image/"))
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
                ad.PrevImage = "~/Content/images/" + fileName;
                upload.SaveAs(Server.MapPath(ad.PrevImage));
            }


            List<string> imagesList = new List<string>();
            foreach (string field in Request.Files)
            {
                if (field.Contains("Images"))
                {
                    var up = Request.Files[field];
                    if (up != null && up.ContentType.Contains("image/"))
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(up.FileName);
                        string addr = "~/Content/images/" + fileName;
                        imagesList.Add(addr);
                        up.SaveAs(Server.MapPath(addr));
                    }
                }
            }
            ad.Images = String.Join("|", imagesList);

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
                if (!string.IsNullOrEmpty(fromDetailsUrl))
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
            //Телефоны
            List<string> phonesList = new List<string>();
            foreach (string field in Request.Form)
            {
                if (field.Contains("Phone"))
                {
                    phonesList.Add(Request.Form[field]);
                }
            }
            ad.Phone = String.Join("|", phonesList);

            // Обновить картинку на превью
            var upload = Request.Files["uPrevImage"];
            if (upload != null && upload.ContentType.Contains("image/"))
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
                ad.PrevImage = "~/Content/images/" + fileName;
                upload.SaveAs(Server.MapPath(ad.PrevImage));
            }

            ad.Images = "";
            List<string> imagesList = new List<string>();

            //Обработка существующих картинок (если выключен чекбокс - удалить)
            string existingImagesSet = forImages.FindById(ad.Id).Images;
            if (!String.IsNullOrEmpty(existingImagesSet))
            {
                string[] existingImages = existingImagesSet.Split('|');
                foreach (string field in Request.Form)
                {
                    //выбрать все чекбоксы
                    if (field.Contains("remove_check"))
                    {
                        var q = Request.Form[field];
                        int number = Int32.Parse(field.Split('-')[1]);
                        // выбрать выключенные чекбоксы
                        if (q == "false")
                        {
                            //удалить с сервера
                            System.IO.File.Delete(Server.MapPath(existingImages[number]));
                        }
                        else
                        {
                            //оставить в списке
                            imagesList.Add(existingImages[number]);
                        }
                    }
                }
            }
            
            //Получаем новые картинки из полей загрузки файлов
            foreach (string field in Request.Files)
            {
                if (field.Contains("Images"))
                {
                    var up = Request.Files[field];
                    if (up != null && up.ContentType.Contains("image/"))
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(up.FileName);
                        string addr = "~/Content/images/" + fileName;
                        imagesList.Add(addr);
                        up.SaveAs(Server.MapPath(addr));
                    }
                }
            }
            ad.Images = String.Join("|", imagesList);
            
            adsManager.Edit(ad);
            if (!string.IsNullOrEmpty(fromDetailsUrl))
                return RedirectToAction("Details", new { id = ad.Id, fromDetailsUrl = fromDetailsUrl });
            else if (String.IsNullOrEmpty(returnUrl))
            {
                return Redirect("/Home/Index/");
            }
            else
            {
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