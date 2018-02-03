using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace real_estate_agency.Models.ViewModels
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "Введите логин или Email!")]
        public string LoginOrEmail { get; set; }

        [Required(ErrorMessage = "Введите пароль!")]
        public string Password { get; set; }
    }
}