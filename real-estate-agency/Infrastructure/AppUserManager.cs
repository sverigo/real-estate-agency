using System;
using real_estate_agency.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;

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

            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromHours(1);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            manager.PasswordValidator = new CustomPassValidator();
            manager.UserValidator = new CustomUserValidator(manager)
            {
                AllowOnlyAlphanumericUserNames = false
            };

            var provider = new DpapiDataProtectionProvider("real-estate-agency");
            manager.UserTokenProvider = new DataProtectorTokenProvider<AppUser>(
                provider.Create("TokenProvider"));

            return manager;
        }
    }
}