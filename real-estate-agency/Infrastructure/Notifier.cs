using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using real_estate_agency.Models;
using Microsoft.AspNet.Identity;

namespace real_estate_agency.Infrastructure
{
    public class Notifier
    {
        AppUserManager userManager;

        public Notifier(AppUserManager userManager)
        {
            this.userManager = userManager;
        }

        public IdentityResult NotifyUser(AppUser user, string message, AppUser sender = null)
        {
            Notification notification = new Notification
            {
                Sender = sender,
                User = user,
                Message = message,
                Date = DateTime.Now
            };

            user.Notifications.Add(notification);
            return userManager.Update(user);
        }

        public IdentityResult SetNotificationsSeen(AppUser user)
        {
            List<Notification> unseenNotifications = user.Notifications.Where(n => !n.Seen).ToList();
            if (unseenNotifications.Count == 0)
                return null;

            unseenNotifications.ForEach(n => n.Seen = true);
            return userManager.Update(user);
        }
    }
}