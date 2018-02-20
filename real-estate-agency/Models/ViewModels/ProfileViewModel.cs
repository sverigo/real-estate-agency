using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace real_estate_agency.Models.ViewModels
{
    public class ChangeProfileViewModel
    {
        public string IdProfile { get; set; }

        [Required(ErrorMessage = "Введите логин!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите имя!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина имени должна быть от 3 до 50 символов!")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Введите текущий пароль!")]
        //public string Password { get; set; }

        //[Required(ErrorMessage = "Введите новый пароль!")]
        //public string NewPassword { get; set; }

        //[Required(ErrorMessage = "Подтвердите новый пароль!")]
        //public string ConfirmNewPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public string IdPassword { get; set; }

        [Required(ErrorMessage = "Введите текущий пароль!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Введите новый пароль!")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Подтвердите новый пароль!")]
        public string ConfirmNewPassword { get; set; }
    }
}