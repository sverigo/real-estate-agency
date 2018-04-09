using System;
using System.Collections.Generic;
using DataParser.Models;
using DataParser.Constants.OLX;

namespace DataParser.DataConverters
{
    internal class OLXDataConverter : AbstractDataConverter
    {
        protected override Dictionary<string, string> ConvertVariativeFields(ref AdvertismentModel model, Dictionary<string, string> variativeFields)
        {
            var dictionary = new Dictionary<string, string>(variativeFields);

            if (dictionary.ContainsKey(OLXDictionaryConstants.AuthorKey))
            {
                model.Author = dictionary[OLXDictionaryConstants.AuthorKey];
                dictionary.Remove(OLXDictionaryConstants.AuthorKey);
            }

            if (dictionary.ContainsKey(OLXDictionaryConstants.ObjectType))
            {
                model.ObjectType = dictionary[OLXDictionaryConstants.ObjectType];
                dictionary.Remove(OLXDictionaryConstants.ObjectType);
            }

            if (dictionary.ContainsKey(OLXDictionaryConstants.RoomCountKey))
            {
                model.RoomCount = dictionary[OLXDictionaryConstants.RoomCountKey];
                dictionary.Remove(OLXDictionaryConstants.RoomCountKey);
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

            if (dictionary.ContainsKey(OLXDictionaryConstants.CategoryKey))
            {
                model.Category = dictionary[OLXDictionaryConstants.CategoryKey];
                dictionary.Remove(OLXDictionaryConstants.CategoryKey);
            }

            return dictionary;
        }
    }
}
