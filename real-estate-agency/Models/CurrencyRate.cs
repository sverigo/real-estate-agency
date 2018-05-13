using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace real_estate_agency.Models
{
    public class CurrencyRate
    {
        public int Id { get; set; }

        public string NameCode { get; set; }

        public double Rate { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}