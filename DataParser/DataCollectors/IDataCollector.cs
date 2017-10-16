using System.Collections.Generic;
using DataParser.Models;

namespace DataParser.DataCollectors
{
    internal interface IDataCollector
    {
        IEnumerable<AdvertismentModel> Collect(int count);
    }
}
