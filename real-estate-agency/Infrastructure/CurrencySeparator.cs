using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace real_estate_agency.Infrastructure
{
    public static class CurrencySepatator
    {
        public const string EUR = "€";
        public const string USD = "$";
        public const string UAH = "грн";

        public static int SeparateValue(string priceString)
        {
            string strValue = string.Join("", priceString.Where(ch =>
            {
                return char.IsDigit(ch) || ch == ',' || ch == '.';
            }).TakeWhile(ch => ch != ',' && ch != '.'));

            return Convert.ToInt32(strValue);
        }

        public static string SeparateCurrency(string priceString)
        {
            priceString = priceString.ToLower();

            if (priceString.Contains(EUR))
                return "EUR";
            else if (priceString.Contains(USD))
                return "USD";
            else
                return "UAH";
        }
    }
}