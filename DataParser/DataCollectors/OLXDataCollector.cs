using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser.Models;
using DataParser.DataCollectors.PagesCollectors;
using DataParser.DataCollectors.AdsCollectors;

namespace DataParser.DataCollectors
{
    internal class OLXDataCollector : IDataCollector
    {
        private const string startUri = @"https://www.olx.ua/nedvizhimost/kvartiry-komnaty/dnepr/";

        public IEnumerable<AdvertismentModel> Collect(int count = 0)
        {
            OLXPagesCollector pagesCollector = new OLXPagesCollector(startUri);
            var pages = pagesCollector.CollectPagesUri(count);

            OLXAdsCollector adsCollector = new OLXAdsCollector(pages);
            var collectedData = adsCollector.CollectAd();

            var models = collectedData.Select(data => data.ConvertToOLX()).ToList();

            return models;
        }
    }
}
