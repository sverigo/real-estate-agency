using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace real_estate_agency.Models.ViewModels
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "Введите логин!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите имя!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина имени должна быть от 3 до 50 символов!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите Email!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль!")]
        public string ConfirmPassword { get; set; }
    }
}