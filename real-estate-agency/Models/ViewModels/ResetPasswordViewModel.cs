using System.ComponentModel.DataAnnotations;
using real_estate_agency.Resources;

namespace real_estate_agency.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputPassword", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputConfirmationPassword", ErrorMessageResourceType = typeof(Resource))]
        public string ConfirmPassword { get; set; }
    }
}