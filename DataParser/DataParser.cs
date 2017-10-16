using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser.DataCollectors;
using DataParser.Models;

namespace DataParser
{
    public static class DataParser
    {
        public static IEnumerable<AdvertismentModel> CollectFromOLX(int count)
        {
            OLXDataCollector dataCollector = new OLXDataCollector();
            return dataCollector.Collect(count);
        }
    }
}
