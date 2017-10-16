using System.Collections.Generic;
using DataParser.DataConverters;
using DataParser.Models;

namespace DataParser.DataCollectors
{
    internal class CollectedData
    {
        internal Dictionary<string, string> CommonFields { get; private set; }
        internal Dictionary<string, string> VariativeFields { get; private set; }
        internal IEnumerable<string> Images { get; private set; }
        internal IEnumerable<string> Phones { get; private set; }
        internal string Url { get; private set; }

        internal CollectedData(Dictionary<string, string> commonFields, Dictionary<string, string> variativeFields,
            IEnumerable<string> images, IEnumerable<string> phones, string url)
        {
            CommonFields = commonFields;
            VariativeFields = variativeFields;
            Images = images;
            Phones = phones;
            Url = url;
        }

        internal AdvertismentModel ConvertToOLX()
        {
            return new OLXDataConverter().Convert(this);
        }
    }
}
