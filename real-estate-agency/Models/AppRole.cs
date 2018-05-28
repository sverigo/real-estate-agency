﻿using Microsoft.AspNet.Identity.EntityFramework;

namespace real_estate_agency.Models
{
    public class AppRole: IdentityRole
    {
        public AppRole() : base() { }

        public AppRole(string name) : base(name) { }
    }
}