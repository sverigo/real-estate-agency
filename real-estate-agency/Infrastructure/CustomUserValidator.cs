using Microsoft.AspNet.Identity;
using real_estate_agency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Threading.Tasks;

namespace real_estate_agency.Infrastructure
{
    public class CustomUserValidator : UserValidator<AppUser>
    {
        UserManager<AppUser, string> userManager;

        public CustomUserValidator(UserManager<AppUser, string> manager) : base(manager)
        {
            userManager = manager;
        }

        public override async Task<IdentityResult> ValidateAsync(AppUser item)
        {
            IdentityResult result = await base.ValidateAsync(item);
            List<string> errors = new List<string>();

            //check email
            Func<bool> IsEmailValid = () =>
            {
                try
                {
                    var email = new MailAddress(item.Email);
                    return true;
                }
                catch
                {
                    return false;
                }
            };
            if (!IsEmailValid.Invoke())
                errors.Add("Неверный формат Email адреса!");

            //check unique emails
            {
                AppUser user = userManager.FindByEmail(item.Email);
                if (user != null && user.Id != item.Id)
                    errors.Add("Указанный Email уже используется другим пользоветелем!");
            }

            //check unique UserName
            {
                AppUser user = userManager.FindByName(item.UserName);
                if (user != null && user.Id != item.Id)
                    errors.Add("Пользователь с таким логином уже существует!");
            }

            if (errors.Any())
                result = new IdentityResult(errors);

            return result;
        }
    }
}