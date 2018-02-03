using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace real_estate_agency.Models
{
    public class AppUser: IdentityUser
    {
        public string Name { get; set; }
    }
}