using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace real_estate_agency.Models
{
    public class MarkedAd
    {
        [Key, Column(Order = 0)]
        public string AppUserId { get; set; }

        [Key, Column(Order = 1)]
        public int AdId { get; set; }

        public virtual AppUser User { get; set; }

        public virtual Ad Ad { get; set; }
    }
}