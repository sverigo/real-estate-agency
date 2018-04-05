using Microsoft.AspNet.Identity;
using real_estate_agency.Infrastructure;
using real_estate_agency.Models;
using real_estate_agency.Models.ViewModels;
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
        UserStatusDirectory userStatDirect = new UserStatusDirectory();

        public ActionResult Add()
        {
            if (User?.Identity.IsAuthenticated ?? false)
            {
                try
                {
                    UserStatus status = userStatDirect.GetUserStatus(User);
                    if (status.isBlocked)
                        return RedirectToAction("Logout", "Account", new { lockoutTime = status.lockoutTime});
                    if (!string.IsNullOrEmpty(status.NotificationsCountSign))
                        ViewBag.NotifCount = status.NotificationsCountSign;
                }
                catch (Exception ex)
                {
                    return View("Error", new string[] { ex.Message });
                }
            }
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
            if (User?.Identity.IsAuthenticated ?? false)
            {
                try
                {
                    UserStatus status = userStatDirect.GetUserStatus(User);
                    if (status.isBlocked)
                        return RedirectToAction("Logout", "Account", new { lockoutTime = status.lockoutTime });
                    if (!string.IsNullOrEmpty(status.NotificationsCountSign))
                        ViewBag.NotifCount = status.NotificationsCountSign;
                }
                catch (Exception ex)
                {
                    return View("Error", new string[] { ex.Message });
                }
            }

            Ad ad = adsManager.FindById(id);
            string userId = User.Identity.GetUserId();

            if (ad == null)
                return View("Error", new string[] { "Объявление с указанным id не существует!" });
            else if (!UserStatusDirectory.IsOwnerOfAd(User, ad))
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
            adsManager.Edit(ad);
            if (!string.IsNullOrEmpty(fromDetailsUrl))
                return RedirectToAction("Details", new { id = ad.Id, fromDetailsUrl = fromDetailsUrl });
            else
                return Redirect(returnUrl);
        }

        public ActionResult Remove(int id, string returnUrl)
        {
            if (User?.Identity.IsAuthenticated ?? false)
            {
                try
                {
                    UserStatus status = userStatDirect.GetUserStatus(User);
                    if (status.isBlocked)
                        return RedirectToAction("Logout", "Account", new { lockoutTime = status.lockoutTime });
                    if (!string.IsNullOrEmpty(status.NotificationsCountSign))
                        ViewBag.NotifCount = status.NotificationsCountSign;
                }
                catch (Exception ex)
                {
                    return View("Error", new string[] { ex.Message });
                }
            }

            Ad ad = adsManager.FindById(id);

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Request.UrlReferrer.ToString();

            if (ad == null)
                return View("Error", new string[] { "Объявление с указанным id не существует!" });
            else if (!UserStatusDirectory.UserCanDeleteAd(User, ad))
                return View("Error", new string[] { "У вас нет прав редактировать это объявление!" });
            else if (UserStatusDirectory.IsOwnerOfAd(User, ad))
                adsManager.RemoveById(id);
            else if (UserStatusDirectory.IsAdmin(User) || UserStatusDirectory.IsModerator(User))
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