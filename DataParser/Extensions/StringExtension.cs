using System.Text.RegularExpressions;

namespace DataParser.Extensions
{
    static class StringExtension
    {
        private const string PHONE_REGEX = "[()]";

        public static string SanitizePhone(this string phone)
        {
            var regex = new Regex(PHONE_REGEX);
            return regex.Replace(phone, string.Empty);
        }
    }
}
