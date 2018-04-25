using System.Collections.Generic;

namespace real_estate_agency.Models.ViewModels
{
    public class SearchUserViewModel
    {
        public string Login { get; set; }
        
        public string Name { get; set; }
        
        public string Email { get; set; }

        public bool? IsPremium { get; set; }

        public bool? IsBlocked { get; set; }

        public List<AppUser> ResultUsers { get; set; }
    }
}