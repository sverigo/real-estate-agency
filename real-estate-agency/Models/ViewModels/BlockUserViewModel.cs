using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace real_estate_agency.Models.ViewModels
{
    public class BlockUserViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Укажите причину")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Укажите длительность")]
        public BlockDuration Duration { get; set; }
    }

    public enum BlockDuration
    {
        [Display(Name = "Час")]
        Hour,
        [Display(Name = "День")]
        Day,
        [Display(Name = "Неделя")]
        Week,
        [Display(Name = "Месяц")]
        Month,
        [Display(Name = "Навсегда")]
        Forever
    }
}