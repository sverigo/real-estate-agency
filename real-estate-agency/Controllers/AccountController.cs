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
        public ActionResult Login()
        {
            if(User?.Identity.IsAuthenticated ?? false)
                return RedirectToAction("Index", "Home");
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
                    if (result.Succeeded)
                    {
                        //send email
                        string id = user.Id;
                        string token = UserManager.GenerateEmailConfirmationToken(id);
                        string link = Url.Action("ConfirmEmail", "Account", new { id = user.Id, token = token });
                        EmailSender.SendConfirmEmail(user, link);
                        return View("EmailSended", null, user.Email);
                    }
                    else
                        result.Errors.ToList().ForEach(err => ModelState.AddModelError("", err));
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

        //[HttpGet]
        //public ActionResult ResetPassword(string id, string token)
        //{

        //}

        //[HttpPost]
        //public ActionResult ResetPassword(ResetPasswordViewModel model)
        //{

        //}
    }
}