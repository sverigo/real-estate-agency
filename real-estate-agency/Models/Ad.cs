﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace real_estate_agency.Models
{
    public class Ad
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string Address { get; set; }

        public string Area { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Author { get; set; }

        public string Images { get; set; }

        public string Phone { get; set; }

        public string Value { get; set; }

        public string Details { get; set; }

        public int Floors { get; set; }

        public int FloorsCount { get; set; }

        public int RoomsCount { get; set; }

        public string PrevImage { get; set; }

        public string AdUrl { get; set; }

        public bool IsPremium { get; set; }
    }
}