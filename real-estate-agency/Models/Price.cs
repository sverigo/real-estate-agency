using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace real_estate_agency.Models
{
    public class Price
    {
        public int Id { get; set; }
        
        public int Days { get; set; }

        public decimal Amount { get; set; }
    }
}