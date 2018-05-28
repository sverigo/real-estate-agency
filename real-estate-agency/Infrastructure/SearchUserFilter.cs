using System;
using System.Collections.Generic;
using System.Linq;
using real_estate_agency.Models;
using real_estate_agency.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace real_estate_agency.Infrastructure
{
    public class SearchUserFilter
    {
        AppUserManager userManager;
        AppRoleManager roleManager;

        public SearchUserFilter(AppUserManager userManager, AppRoleManager roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public List<AppUser> SearchUsers(SearchUserViewModel conditions)
        {
            string name = conditions.Name?.Trim().ToLower();
            string userName = conditions.Login?.Trim().ToLower();
            string email = conditions.Email?.Trim().ToLower();
            bool? isPremium = conditions.IsPremium;
            bool? isBlocked = conditions.IsBlocked;

            AppRole userRole = roleManager.FindByName(UserStatusDirectory.Roles.USERS);
            List<string> idsUsers = userRole.Users.Select(u => u.UserId).ToList();
            List<AppUser> result = userManager.Users.Where(u => idsUsers.Contains(u.Id))
                .ToList();

            if (!string.IsNullOrEmpty(userName))
            {
                result = result.Where(u => u.UserName.ToLower().Contains(userName)).ToList();
            }

            if (!string.IsNullOrEmpty(name))
            {
                result = result.Where(u => u.Name.ToLower().Contains(name)).ToList();
            }

            if (!string.IsNullOrEmpty(email))
            {
                result = result.Where(u => u.Email.ToLower().Contains(email)).ToList();
            }
            
            if (isPremium != null)
            {
                result = result.Where(u => userManager.IsInRole(u.Id, UserStatusDirectory.Roles.PREMIUM_USER)).ToList();
            }
            
            if (isBlocked != null)
            {
                if (isBlocked ?? false)
                    result = result.Where(u => u.LockoutEndDateUtc >= DateTime.UtcNow).ToList();
                else
                    result = result.Where(u =>
                    {
                        return u.LockoutEndDateUtc < DateTime.UtcNow ||
                        u.LockoutEndDateUtc == null;
                    }).ToList();
            }

            return result;
        }
    }
}