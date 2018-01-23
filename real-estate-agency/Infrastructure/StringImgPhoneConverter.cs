using System.Collections.Generic;

namespace real_estate_agency.Infrastructure
{
    public static class StringImgPhoneConverter
    {
        const char SEPARATOR = '|';
        public static List<string> StringToList(string str)
        {
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(str))
                return list;

            list.AddRange(str.Split(SEPARATOR));
            return list;
        }

        public static string ListToString(List<string> list)
        {
            if (list == null)
                return string.Empty;
            string str = string.Join(SEPARATOR.ToString(), list.ToArray());
            return str;
        }
    }
}