using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser.Constants.Common
{
    internal class HelperConstants
    {
        internal const int MaxTryCount = 5;
    }

    internal class DictionaryConstants
    {
        internal const string AuthorNameKey = "name";
        internal const string TitleKey = "title";
        internal const string AddressKey = "address";
        internal const string PriceKey = "price";
        internal const string DetailsKey = "details";
        internal const string HasPhoneKey = "hasPhone";
    }

    internal class RegexConstants
    {
        internal const string RegexFullWordPattern = @"(?i)\b{0}\b";
    }
}
