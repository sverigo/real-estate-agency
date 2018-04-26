using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataParser.Models;
using DataParser.DataCollectors;
using DataParser.Constants.Common;
using DataParser.Helpers;

namespace DataParser.DataConverters
{
    internal abstract class AbstractDataConverter
    {
        internal AdvertismentModel Convert(CollectedData data)
        {
            AdvertismentModel model = new AdvertismentModel();

            model.Url = data.Url;

            var imgList = data.Images.ToList();
            model.PreviewImg = imgList.FirstOrDefault();
            imgList.Remove(model.PreviewImg);

            model.Images = imgList;
            model.Phones = data.Phones;

            ConvertCommonParameters(ref model, data.CommonFields);

            try
            {
                model.OtherData = new SerializableDictionary<string, string>(ConvertVariativeFields(ref model, data.VariativeFields));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            

            return model;
        }

        private static void ConvertCommonParameters(ref AdvertismentModel model, Dictionary<string, string> commonFields)
        {
            model.AuthorName = commonFields[DictionaryConstants.AuthorNameKey];
            model.Title = commonFields[DictionaryConstants.TitleKey];
            model.Address = commonFields[DictionaryConstants.AddressKey];
            model.Price = commonFields[DictionaryConstants.PriceKey];
            model.Details = commonFields[DictionaryConstants.DetailsKey];
            model.HasPhone = bool.Parse(commonFields[DictionaryConstants.HasPhoneKey]);
        }

        protected abstract Dictionary<string, string> ConvertVariativeFields(ref AdvertismentModel model, Dictionary<string, string> variativeFields);
    }
}
