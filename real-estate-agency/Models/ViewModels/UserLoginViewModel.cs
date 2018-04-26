using System.ComponentModel.DataAnnotations;
using real_estate_agency.Resources;

namespace real_estate_agency.Models.ViewModels
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessageResourceName = "ModelValidatorInputLoginOrEmail", ErrorMessageResourceType = typeof(Resource))]
        public string LoginOrEmail { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputPassword", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }
    }
}