using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using real_estate_agency.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;


namespace real_estate_agency.Infrastructure
{
    public class AppUserManager: UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store): base(store)
        { }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options,
            IOwinContext context)
        {
            AppIdentityDBContext db = context.Get<AppIdentityDBContext>();
            AppUserManager manager = new AppUserManager(new UserStore<AppUser>(db));

            manager.PasswordValidator = new CustomPassValidator();
            manager.UserValidator = new UserValidator<AppUser>(manager)
            {
                RequireUniqueEmail = true
            };

            return manager;
        }
    }
}