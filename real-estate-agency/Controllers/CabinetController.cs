using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using real_estate_agency.Infrastructure;
using real_estate_agency.Models;
using real_estate_agency.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace real_estate_agency.Controllers
{
    [Authorize]
    public class CabinetController : Controller
    {
        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        AdsManager adsManager = new AdsManager();

        PaymentManager paymentManager = new PaymentManager();

        // GET: Cabinet
        public ActionResult Index()
        {
            if (User?.Identity.IsAuthenticated ?? false)
            {
                try
                {
                    UserStatusDirectory userStatDirect = new UserStatusDirectory();
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
            return View();
        }

        public PartialViewResult MyAds()
        {
            IEnumerable<Ad> ads = adsManager.GetAdsByUserAthorId(User.Identity.GetUserId());
            return PartialView(ads);
        }

        public PartialViewResult MyMarks()
        {
            IEnumerable<Ad> markedAds = adsManager.GetMarkedAdsByUserAthorId(User.Identity.GetUserId());
            return PartialView(markedAds);
        }

        public ActionResult MyNotifications()
        {
            AppUser user = UserManager.FindById(User.Identity.GetUserId());
            Notifier notifier = new Notifier();
            IdentityResult result = notifier.SetNotificationsSeen(user);
            if (result == null || result.Succeeded)
            {
                IEnumerable<Notification> notifications = user.Notifications.ToList();
                return PartialView(notifications.Reverse());
            }
            else
                return View("Error", result.Errors);
        }

        public ActionResult MyPayments()
        {
            AppUser user = UserManager.FindById(User.Identity.GetUserId());
            return PartialView("MyPayments", user.Payments.ToList());
        }
        
        public ActionResult BuyPremium()
        {
            if (User?.Identity.IsAuthenticated ?? false)
            {
                try
                {
                    UserStatusDirectory userStatDirect = new UserStatusDirectory();
                    UserStatus status = userStatDirect.GetUserStatus(User);
                    if (status.isBlocked)
                        return RedirectToAction("Logout", "Account", new { lockoutTime = status.lockoutTime });
                    if (!string.IsNullOrEmpty(status.NotificationsCountSign))
                        ViewBag.NotifCount = status.NotificationsCountSign;
                    if (status.isPremium)
                        ViewBag.IsPremium = true;
                }
                catch (Exception ex)
                {
                    return View("Error", new string[] { ex.Message });
                }
            }
            return View("BuyPremium", paymentManager.Prices);
        }

        [HttpPost]
        public ActionResult BuyPremium(int days, decimal amount)
        {
            AppUser user = UserManager.FindById(User.Identity.GetUserId());
            string callBackUrl, resultUrl;
            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["OnServer"]))
            {
                callBackUrl = "http://" + HttpContext.Request.Url.Host + Url.Action("ConfirmPayment", "Cabinet");
                resultUrl = HttpContext.Request.Url.Host + Url.Action("Index", "Cabinet");
            }
            else
            {
                callBackUrl = Url.Action("ConfirmPayment", "Cabinet", null, Request.Url.Scheme);
                resultUrl = Url.Action("Index", "Cabinet", null, Request.Url.Scheme);
            }
            PaymentData paymentData;
            try
            {
                paymentData = paymentManager.CreatePayment(user, days, amount,
                    callBackUrl, resultUrl);
                return View("ConfirmPayment", paymentData);
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message.Split('|'));
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ConfirmPayment(string data, string signature)
        {
            paymentManager.ConfirmPayment(data, signature);
            return new EmptyResult();
        }

        public PartialViewResult ProfileSettings()
        {
            return PartialView("ProfileSettings", User.Identity.GetUserId());
        }

        public PartialViewResult ChangeProfile(string userId)
        {
            AppUser user = UserManager.FindById(userId);
            ChangeProfileViewModel model = new ChangeProfileViewModel
            {
                IdProfile = user.Id,
                Name = user.Name,
                Login = user.UserName
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeProfile(ChangeProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindByIdAsync(model.IdProfile);
                user.Name = model.Name;
                user.UserName = model.Login;
                IdentityResult updateResult = await UserManager.UpdateAsync(user);
                if (updateResult.Succeeded)
                {
                    ViewBag.SuccessMessage = "Изменения успешно сохранены!";
                    return PartialView("ChangeProfile", model);
                }
                else
                    updateResult.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
            }

            return PartialView("ChangeProfile", model);
        }

        public PartialViewResult ChangePassword(string userId)
        {
            AppUser user = UserManager.FindById(userId);
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                IdPassword = user.Id
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewPassword != model.ConfirmNewPassword)
                    ModelState.AddModelError("", "Новые пароли не совпадают, подтвердите еще раз!");
                else
                {
                    AppUser existingUser = await UserManager.FindByIdAsync(model.IdPassword);
                    AppUser user = await UserManager.FindAsync(existingUser.UserName, model.Password);
                    if (user == null)
                        ModelState.AddModelError("", "Неверный текущий пароль!");
                    else
                    {
                        IdentityResult result = await UserManager.PasswordValidator.ValidateAsync(model.NewPassword);
                        if (result.Succeeded)
                        {
                            user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.NewPassword);
                            IdentityResult updateResult = await UserManager.UpdateAsync(user);

                            if (updateResult.Succeeded)
                            {
                                ViewBag.SuccessMessage = "Изменения успешно сохранены!";
                                return PartialView("ChangePassword", model);
                            }
                            else
                                result.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
                        }
                        else
                            result.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
                    }
                }
            }
            return PartialView("ChangePassword", model);
        }
    }
}