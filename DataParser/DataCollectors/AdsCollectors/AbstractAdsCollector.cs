using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser.DataCollectors.AdsCollectors
{
    internal abstract class AbstractAdsCollector
    {
        internal IEnumerable<string> pagesWithAds;

        internal AbstractAdsCollector(IEnumerable<string> pagesWithAds)
        {
            this.pagesWithAds = pagesWithAds;
        }

        internal AbstractAdsCollector(string pageWithAd) : this(new List<string> { pageWithAd }) { }

        internal abstract IEnumerable<CollectedData> CollectAd();
    }
}
