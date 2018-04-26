using System.ComponentModel.DataAnnotations;
using real_estate_agency.Resources;

namespace real_estate_agency.Models.ViewModels
{
    public class ChangeProfileViewModel
    {
        public string IdProfile { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputLogin", ErrorMessageResourceType = typeof(Resource))]
        public string Login { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputName", ErrorMessageResourceType = typeof(Resource))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "ModelValidatorLenName", ErrorMessageResourceType = typeof(Resource))]
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

        [Required(ErrorMessageResourceName = "ModelValidatorInputCurrentPassword", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputNewPassword", ErrorMessageResourceType = typeof(Resource))]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorConfirmNewPassword", ErrorMessageResourceType = typeof(Resource))]
        public string ConfirmNewPassword { get; set; }
    }
}