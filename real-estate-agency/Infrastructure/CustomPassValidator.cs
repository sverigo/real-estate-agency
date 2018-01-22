using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace real_estate_agency.Infrastructure
{
    public class CustomPassValidator : PasswordValidator
    {
        public override async Task<IdentityResult> ValidateAsync(string password)
        {
            IdentityResult result = await base.ValidateAsync(password);
            List<string> errors = new List<string>();

            if (password.Length < 6)
            {
                errors.Add("Длина пароля должна быть не менее 6 символов!");
            }

            if ((password.Where(ch => char.IsDigit(ch)).Count() == 0) ||
                (password.Where(ch => char.IsLetter(ch)).Count() == 0))
            {
                errors.Add("Пароль должен содержать и цифры, и буквы!");
            }

            if (errors.Any())
                return new IdentityResult(errors);
            else
                return result;
        }
    }
}