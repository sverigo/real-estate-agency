using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace real_estate_agency.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        [Required(ErrorMessage = "Введите пароль!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль!")]
        public string ConfirmPassword { get; set; }
    }
}