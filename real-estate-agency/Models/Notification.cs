using System;

namespace real_estate_agency.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }

        public virtual AppUser User { get; set; }

        public virtual AppUser Sender { get; set; }

        public bool Seen { get; set; }
    }
}