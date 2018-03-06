using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace real_estate_agency.Models.ViewModels
{
    public class RemoveAdViewModel
    {
        [Required]
        public string Message { get; set; }

        public int AdId { get; set; }

        public string ReturnURL { get; set; }
    }
}