using System;
using System.Text.RegularExpressions;
using System.Globalization;
using DataParser.Constants.OLX;


namespace DataParser.DataConverters.Date
{
    internal class OLXDateConverter : IDateConverter
    {
        public DateTime Convert(string dateString)
        {
            DateTime dateTime = DateTime.Now;
            
            if(dateString.Contains(OLXDateConstants.Today))
            {
                Regex regex = new Regex(timeRegexPattern);
                string timeString = regex.Match(dateString).Value;
                var result = regex.Match(dateString);
                var timeSpan = TimeSpan.Parse(timeString, CultureInfo.InvariantCulture);
                var date = DateTime.Today;

                dateTime = date + timeSpan;
            }

            return dateTime;
        }

        private const string timeRegexPattern = "^(?:0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";
    }
}
