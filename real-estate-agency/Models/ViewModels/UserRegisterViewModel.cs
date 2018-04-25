using System.ComponentModel.DataAnnotations;
using real_estate_agency.Resources;

namespace real_estate_agency.Models.ViewModels
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessageResourceName = "ModelValidatorInputLogin", ErrorMessageResourceType = typeof(Resource))]
        public string Login { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputName", ErrorMessageResourceType = typeof(Resource))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "ModelValidatorLenName", ErrorMessageResourceType = typeof(Resource))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputEmail", ErrorMessageResourceType = typeof(Resource))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputPassword", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputConfirmationPassword", ErrorMessageResourceType = typeof(Resource))]
        public string ConfirmPassword { get; set; }
    }
}