using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace real_estate_agency.Models
{
    public class Payment
    {   
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int Days { get; set; }

        public decimal Amount { get; set; }

        public DateTime? ConfirmedDate { get; set; }
        
        public virtual AppUser User { get; set; }
    }
}