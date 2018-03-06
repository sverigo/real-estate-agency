using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace real_estate_agency.Models
{
    public class AppUser: IdentityUser
    {
        public AppUser()
        {
            MarkedAds = new HashSet<MarkedAd>();
        }

        public string Name { get; set; }

        public virtual ICollection<Ad> PublishedAds { get; set; }

        public HashSet<MarkedAd> MarkedAds { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}