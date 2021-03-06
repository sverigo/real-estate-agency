﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using real_estate_agency.Models;
using System;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace real_estate_agency.Infrastructure
{
    public class UserStatusDirectory
    {
        public struct Roles
        {
            public const string ADMINS = "Admins";
            public const string MODERATORS = "Moderators";
            public const string PREMIUM_USER = "PremiumUsers";
            public const string USERS = "Users";
        }

        private AppRoleManager RoleManager
        {
            get { return HttpContext.Current.GetOwinContext().GetUserManager<AppRoleManager>(); }
        }

        private AppUserManager UserManager
        {
            get { return HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        public UserStatus GetUserStatus(string userId)
        {
            AppUser user = UserManager.FindById(userId);
            return GetUserStatus(user);
        }

        public UserStatus GetUserStatus(IPrincipal user)
        {
            return GetUserStatus(user.Identity.GetUserId());
        }

        public UserStatus GetUserStatus(AppUser user)
        {
            if (user != null)
            {
                UserStatus status = new UserStatus();

                //blocked
                if (UserManager.IsLockedOut(user.Id))
                {
                    status.lockoutTime = user.LockoutEndDateUtc;
                    status.isBlocked = true;
                }

                //check premium
                Payment payment = user.Payments.SingleOrDefault(p => 
                {
                    DateTime endDate = DateTime.MinValue;
                    if (p.ConfirmedDate != null)
                        endDate = (DateTime)p.ConfirmedDate + TimeSpan.FromDays(p.Days);
                    return endDate > DateTime.UtcNow;
                });
                if (payment == null && UserManager.IsInRole(user.Id, Roles.PREMIUM_USER))
                {
                    UserManager.RemoveFromRole(user.Id, Roles.PREMIUM_USER);
                    Notifier notifier = new Notifier();
                    notifier.NotifyUser(user, "Срок премиум подписки истек.");
                }
                if (UserManager.IsInRole(user.Id, Roles.PREMIUM_USER))
                    status.isPremium = true;

                //check notifications
                status.notificationsCount = user.Notifications.Where(n => !n.Seen).Count();

                return status;
            }
            else
                throw new Exception("Невозможно получить статус пользователя!");
        }

        public static bool UserCanDeleteAd(IPrincipal user, Ad ad)
        {
            return (ad.UserAuthorId ?? "") == user.Identity.GetUserId() ||
                user.IsInRole(Roles.ADMINS) ||
                user.IsInRole(Roles.MODERATORS);
        }

        public static bool IsPremium(AppUser user, IOwinContext owin)
        {
            return IsPremium(user.Id, owin);
        }

        public static bool IsPremium(string id, IOwinContext owin)
        {
            return owin.GetUserManager<AppUserManager>().IsInRole(id, Roles.PREMIUM_USER);
        }

        public static bool IsOwnerOfAd(string userId, Ad ad)
        {
            return ad.UserAuthorId == userId;
        }

        public static bool IsOwnerOfAd(IPrincipal user, Ad ad)
        {
            if (string.IsNullOrEmpty(ad.UserAuthorId))
                return false;
            return ad.UserAuthorId == user.Identity.GetUserId();
        }

        public static bool IsAdmin(IPrincipal user)
        {
            return user.IsInRole(Roles.ADMINS);
        }

        public static bool IsModerator(IPrincipal user)
        {
            return user.IsInRole(Roles.MODERATORS);
        }
    }
}