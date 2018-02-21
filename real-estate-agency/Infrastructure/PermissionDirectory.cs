using Microsoft.AspNet.Identity;
using real_estate_agency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace real_estate_agency.Infrastructure
{
    public class PermissionDirectory
    {
        public const string ADMINS = "Admins";
        public const string MODERATORS = "Moderators";
        public const string PREMIUM_USER = "PremiumUsers";
        public const string USERS = "Users";

        public static bool UserCanDeleteAd(IPrincipal user, Ad ad)
        {
            return (ad.UserAuthorId ?? "") == user.Identity.GetUserId() ||
                user.IsInRole(ADMINS) ||
                user.IsInRole(MODERATORS);
        }

        public static bool IsOwnerOfAd(string userId, Ad ad)
        {
            return ad.UserAuthorId == userId;
        }

        public static bool IsOwnerOfAd(IPrincipal user, Ad ad)
        {
            return ad.UserAuthorId == user.Identity.GetUserId();
        }

        public static bool IsAdmin(IPrincipal user)
        {
            return user.IsInRole(ADMINS);
        }
    }
}