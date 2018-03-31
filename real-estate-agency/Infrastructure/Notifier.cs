using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using real_estate_agency.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;

namespace real_estate_agency.Infrastructure
{
    public class Notifier
    {
        AppIdentityDBContext data;

        private AppUserManager UserManager
        {
            get { return HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        public Notifier()
        {
            data = new AppIdentityDBContext();
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
            return UserManager.Update(user);
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
            return UserManager.Update(user);
        }
    }
}