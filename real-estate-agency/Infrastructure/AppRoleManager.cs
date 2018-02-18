using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using real_estate_agency.Models;
using System;
using System.Security.Principal;

namespace real_estate_agency.Infrastructure
{
    public static class RolesDirectory
    {
        public const string ADMINS = "Admins";
        public const string MODERATORS = "Moderators";
        public const string PREMIUM_USER = "PremiumUsers";
        public const string USERS = "Users";

        public static bool UserCanEditDeleteAd(IPrincipal user, Ad ad)
        {
            return (ad.UserAuthorId ?? "") == user.Identity.GetUserId() ||
                user.IsInRole(ADMINS) ||
                user.IsInRole(MODERATORS);
        }
    }

    public class AppRoleManager : RoleManager<AppRole>, IDisposable
    {
        public AppRoleManager(IRoleStore<AppRole, string> store) : base(store) { }

        public static AppRoleManager Create(IdentityFactoryOptions<AppRoleManager> options,
            IOwinContext context)
        {
            return new AppRoleManager(new RoleStore<AppRole>(context.Get<AppIdentityDBContext>()));
        }
    }
}