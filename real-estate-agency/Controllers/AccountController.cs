﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using real_estate_agency.Infrastructure;
using real_estate_agency.Models.ViewModels;
using real_estate_agency.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Email;
using Microsoft.Owin.Security;
using System.Security.Claims;
using real_estate_agency.Resources;
using System.Web.Configuration;

namespace real_estate_agency.Controllers
{
    public class AccountController : Controller
    {
        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        private IAuthenticationManager AuthManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        [HttpGet]
        public ActionResult Login(string returnURL)
        {
            if(User?.Identity.IsAuthenticated ?? false)
                return RedirectToAction("Index", "Home");
            ViewBag.returnURL = returnURL;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UserLoginViewModel model, string returnURL)
        {
            if (ModelState.IsValid)
            {
                AppUser tryingUser = await UserManager.FindByEmailAsync(model.LoginOrEmail) ??
                    await UserManager.FindByNameAsync(model.LoginOrEmail);

                if (tryingUser == null)
                    ModelState.AddModelError("", Resource.AccountValidator1);
                else
                {
                    if (!tryingUser.EmailConfirmed)
                    {
                        ModelState.AddModelError("", Resource.AccountValidator2);
                        ViewBag.returnURL = returnURL;
                        return View(model);
                    }

                    if (UserManager.IsLockedOut(tryingUser.Id))
                    {
                        ModelState.AddModelError("", Resource.AccountValidator3 + tryingUser.LockoutEndDateUtc?.ToLocalTime());
                        ViewBag.returnURL = returnURL;
                        return View(model);
                    }

                    AppUser user = await UserManager.FindAsync(tryingUser.UserName, model.Password);
                    if (user == null)
                    {
                        UserManager.AccessFailed(tryingUser.Id);

                        if (UserManager.IsLockedOut(tryingUser.Id))
                        {
                            string message = Resource.AccountValidator4 + Resource.AccountValidator5;
                            EmailSender.SendUserBlockedMail(tryingUser, BlockDuration.Hour, message);
                            ModelState.AddModelError("", Resource.AccountValidator3 + tryingUser.LockoutEndDateUtc?.ToLocalTime());
                        }
                        else
                        {
                            int attempts = UserManager.MaxFailedAccessAttemptsBeforeLockout - tryingUser.AccessFailedCount;
                            ModelState.AddModelError("", Resource.AccountValidator6 + attempts);
                        }
                    }
                    else
                    {
                        ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                            DefaultAuthenticationTypes.ApplicationCookie);

                        AuthManager.SignOut();
                        AuthManager.SignIn(new AuthenticationProperties { IsPersistent = true }, ident);
                        UserManager.ResetAccessFailedCount(user.Id);
                        if (string.IsNullOrEmpty(returnURL))
                            returnURL = Url.Action("Index", "Home");
                        return Redirect(returnURL);
                    }
                }
            }

            ViewBag.returnURL = returnURL;
            return View(model);
        }

        public ActionResult Logout(DateTime? lockoutTime)
        {
            AuthManager.SignOut();
            if (lockoutTime == null)
                return RedirectToAction("Index", "Home");
            else
                return View("Info", null, $"Учетная запись заблокирована до {lockoutTime}");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserRegisterViewModel userInfo)
        {
            if (ModelState.IsValid)
            {
                if (userInfo.ConfirmPassword != userInfo.Password)
                    ModelState.AddModelError("", Resource.PasswordValidator1);
                else
                {
                    AppUser user = new AppUser
                    {
                        UserName = userInfo.Login,
                        Name = userInfo.Name,
                        Email = userInfo.Email,
                    };

                    IdentityResult result = UserManager.Create(user, userInfo.Password);
                    IdentityResult roleRes = null;
                    if (result.Succeeded)
                        roleRes = UserManager.AddToRole(user.Id, UserStatusDirectory.Roles.USERS);
                    if (result.Succeeded && (roleRes?.Succeeded ?? false)) 
                    {
                        //send email
                        string token = UserManager.GenerateEmailConfirmationToken(user.Id);
                        string link;
                        if (Convert.ToBoolean(WebConfigurationManager.AppSettings["OnServer"]))
                            link = "http://" + HttpContext.Request.Url.Host + Url.Action("ConfirmEmail", "Account",
                                new { id = user.Id, token = token });
                        else
                            link = Url.Action("ConfirmEmail", "Account", new { id = user.Id, token = token },
                            Request.Url.Scheme);
                        EmailSender.SendConfirmEmail(user, link);
                        string info = $"На вашу почту {user.Email} было отправлено письмо со ссылкой для активации " +
                            $"учетной записи!";
                        return View("Info", null, info);
                    }
                    else
                    {
                        result.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
                        roleRes?.Errors?.ToList()?.ForEach(err => ModelState.AddModelError("", err));
                    }
                }
            }
            return View(userInfo);
        }

        public async Task<ActionResult> ConfirmEmail(string id, string token)
        {
            if (id == null || token == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(id, token);
            if (result.Succeeded)
            {
                string info = $"Ваша учетная запис была успешно активирована! " +
                    $"Теперь вы можете совершить вход в систему!";
                return View("Info", null, info);
            }

            return View("Error", result.Errors);
        }

        [HttpGet]
        public ActionResult PasswordResetRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PasswordResetRequest(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", Resource.AccountValidator7);
                return View(email);
            }

            AppUser user = await UserManager.FindByEmailAsync(email);
            if (user == null)
                ModelState.AddModelError("", Resource.AccountValidator8);
            else
            {
                string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                string link;
                if (Convert.ToBoolean(WebConfigurationManager.AppSettings["OnServer"]))
                    link = "http://" + HttpContext.Request.Url.Host + Url.Action("ResetPassword", "Account", 
                        new { id = user.Id, token = token });
                else
                    link = Url.Action("ResetPassword", "Account", new { id = user.Id, token = token },
                        Request.Url.Scheme);
                EmailSender.SendPasswordResetMail(user, link);
                string info = $"На вашу почту {user.Email} было отправлено письмо с ссылкой для " +
                            $"сброса пароля!";
                return View("Info", null, info);
            }

            return View(email);
        }

        [HttpGet]
        public async Task<ActionResult> ResetPassword(string id, string token)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user == null)
                return View("Error");

            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Id = id,
                Token = token,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                    ModelState.AddModelError("", Resource.PasswordValidator1);
                else
                {
                    IdentityResult result = await UserManager.ResetPasswordAsync(model.Id, 
                        model.Token, model.Password);
                    if (result.Succeeded)
                        return View("Info", null, Resource.AccountValidator9);
                    else
                        result.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
                }
            }

            return View(model);
        }
    }
}