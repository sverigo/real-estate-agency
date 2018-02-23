using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using real_estate_agency.Infrastructure;
using real_estate_agency.Models.ViewModels;
using real_estate_agency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using real_estate_agency.Email;
using Microsoft.Owin.Security;
using System.Security.Claims;

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
                AppUser user = await UserManager.FindByEmailAsync(model.LoginOrEmail) ??
                    await UserManager.FindByNameAsync(model.LoginOrEmail);

                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        ModelState.AddModelError("", "Активируйте свою учетную запись. Проверьте почту!");
                        ViewBag.returnURL = returnURL;
                        return View(model);
                    }
                    user = await UserManager.FindAsync(user.UserName, model.Password);
                }
                if (user == null)
                    ModelState.AddModelError("", "Неверное имя или пароль!");
                else
                {
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                        DefaultAuthenticationTypes.ApplicationCookie);

                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = true }, ident);

                    if (string.IsNullOrEmpty(returnURL))
                        returnURL = Url.Action("Index", "Home");
                    return Redirect(returnURL);
                }
            }
            ViewBag.returnURL = returnURL;
            return View(model);
        }

        public ActionResult Logout()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(UserRegisterViewModel userInfo)
        {
            if(ModelState.IsValid)
            {
                if (userInfo.ConfirmPassword != userInfo.Password)
                    ModelState.AddModelError("", "Введенные пароли не одинаковы!");
                else
                {
                    AppUser user = new AppUser
                    {
                        UserName = userInfo.Login,
                        Name = userInfo.Name,
                        Email = userInfo.Email,
                    };

                    IdentityResult result = await UserManager.CreateAsync(user, userInfo.Password);
                    IdentityResult roleRes = await UserManager.AddToRoleAsync(user.Id, PermissionDirectory.USERS);
                    if (result.Succeeded && roleRes.Succeeded)
                    {
                        //send email
                        string token = UserManager.GenerateEmailConfirmationToken(user.Id);
                        string link = Url.Action("ConfirmEmail", "Account", new { id = user.Id, token = token },
                            Request.Url.Scheme);
                        EmailSender.SendConfirmEmail(user, link);
                        string info = $"На вашу почту {user.Email} было отправлено письмо с ссылкой для активации " +
                            $"учетной записи!";
                        return View("Info", null, info);
                    }
                    else
                    {
                        result.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
                        roleRes.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
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
                return RedirectToAction("Index", "Home");
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
                ModelState.AddModelError("", "Укажите Email!");
                return View(email);
            }

            AppUser user = await UserManager.FindByEmailAsync(email);
            if (user == null)
                ModelState.AddModelError("", "Пользователя с таким Email не существует");
            else
            {
                string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                string link = Url.Action("ResetPassword", "Account", new { id = user.Id, token = token }, 
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
                    ModelState.AddModelError("", "Введенные пароли не одинаковы!");
                else
                {
                    IdentityResult result = await UserManager.ResetPasswordAsync(model.Id, 
                        model.Token, model.Password);
                    if (result.Succeeded)
                        return View("Info", null, "Новый пароль успешно установлен!");
                    else
                        result.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
                }
            }

            return View(model);
        }
    }
}