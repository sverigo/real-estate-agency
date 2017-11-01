using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser.Models;
using DataParser.Constants.OLX;

namespace DataParser.DataConverters
{
    internal class OLXDataConverter : AbstractDataConverter
    {
        protected override void ConvertVariativeFields(ref AdvertismentModel model, Dictionary<string, string> variativeFields)
        {
            var dictionary = new Dictionary<string, string>(variativeFields);

            if (dictionary.ContainsKey(OLXDictionaryConstants.AuthorKey))
            {
                model.Author = dictionary[OLXDictionaryConstants.AuthorKey];
                dictionary.Remove(OLXDictionaryConstants.AuthorKey);
            }

            if (dictionary.ContainsKey(OLXDictionaryConstants.AdTypeKey))
            {
                model.AdType = dictionary[OLXDictionaryConstants.AdTypeKey];
                dictionary.Remove(OLXDictionaryConstants.AdTypeKey);
            }

            if (dictionary.ContainsKey(OLXDictionaryConstants.RoomCountKey))
            {
                model.RoomCount = dictionary[OLXDictionaryConstants.RoomCountKey];
                dictionary.Remove(OLXDictionaryConstants.RoomCountKey);
            }

            if (dictionary.ContainsKey(OLXDictionaryConstants.LiveAreaKey))
            {
                model.LiveArea = dictionary[OLXDictionaryConstants.LiveAreaKey];
                dictionary.Remove(OLXDictionaryConstants.LiveAreaKey);
            }

            if (dictionary.ContainsKey(OLXDictionaryConstants.AreaKey))
            {
                model.Area = dictionary[OLXDictionaryConstants.AreaKey];
                dictionary.Remove(OLXDictionaryConstants.AreaKey);
            }

            if (dictionary.ContainsKey(OLXDictionaryConstants.FloorKey))
            {
                model.Floor = dictionary[OLXDictionaryConstants.FloorKey];
                dictionary.Remove(OLXDictionaryConstants.FloorKey);
            }

            if (dictionary.ContainsKey(OLXDictionaryConstants.FloorCountKey))
            {
                model.FloorCount = dictionary[OLXDictionaryConstants.FloorCountKey];
                dictionary.Remove(OLXDictionaryConstants.FloorCountKey);
            }

            if (dictionary.ContainsKey(OLXDictionaryConstants.TypeKey))
            {
                model.Type = dictionary[OLXDictionaryConstants.TypeKey];
                dictionary.Remove(OLXDictionaryConstants.TypeKey);
            }

            if (dictionary.ContainsKey(OLXDictionaryConstants.BeginDataKey))
            {
                model.BeginData = dictionary[OLXDictionaryConstants.BeginDataKey];
                dictionary.Remove(OLXDictionaryConstants.BeginDataKey);
            }

            if (dictionary.Count != 0)
                throw new InvalidOperationException("There is unknown key");
        }
    }
}
