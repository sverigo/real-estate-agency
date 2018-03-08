using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using real_estate_agency.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace real_estate_agency.Infrastructure
{
    public class Notifier
    {
        AppUserManager userManager;
        RealEstateDBContext data;

        public Notifier(AppUserManager userManager)
        {
            this.userManager = userManager;
            data = new RealEstateDBContext();
        }

        public IdentityResult NotifyUser(AppUser user, string message, AppUser sender = null)
        {
            Notification notification = new Notification
            {
                Sender = sender,
                User = user,
                Message = message,
                Date = DateTime.UtcNow
            };

            user.Notifications.Add(notification);
            return userManager.Update(user);
        }

        public IdentityResult SetNotificationsSeen(AppUser user)
        {
            //delete old seen
            List<Notification> oldNotifications = data.Notifications
                .Where(n => n.Seen && DbFunctions.DiffDays(n.Date, DateTime.UtcNow) > 14).ToList();
            if (oldNotifications.Count > 0)
            {
                data.Notifications.RemoveRange(oldNotifications);
                data.SaveChanges();
            }

            List<Notification> unseenNotifications = user.Notifications.Where(n => !n.Seen).ToList();
            if (unseenNotifications.Count == 0)
                return null;

            unseenNotifications.ForEach(n => n.Seen = true);
            return userManager.Update(user);
        }
    }
}