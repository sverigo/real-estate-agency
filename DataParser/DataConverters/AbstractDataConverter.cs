using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser.Models;
using DataParser.DataCollectors;
using DataParser.Constants.Common;

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
            ConvertVariativeFields(ref model, data.VariativeFields);

            return model;
        }

        private static void ConvertCommonParameters(ref AdvertismentModel model, Dictionary<string, string> commonFields)
        {
            model.AuthorName = commonFields[DictionaryConstants.AuthorNameKey];
            model.Title = commonFields[DictionaryConstants.TitleKey];
            model.Address = commonFields[DictionaryConstants.AddressKey];
            model.Price = commonFields[DictionaryConstants.PriceKey];
            model.Details = commonFields[DictionaryConstants.DetailsKey];
        }

        protected abstract void ConvertVariativeFields(ref AdvertismentModel model, Dictionary<string, string> variativeFields);
    }
}
