﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace real_estate_agency.Infrastructure
{
    public struct UserStatus
    {
        public bool isBlocked;
        public bool isPremium;
        public int notificationsCount;
        public DateTime? lockoutTime;

        public string NotificationsCountSign
        {
            get { return (notificationsCount > 0) ? " +" + notificationsCount : ""; }
        }
    }
}