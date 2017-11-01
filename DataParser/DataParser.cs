using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser.DataCollectors;
using DataParser.Models;

namespace DataParser
{
    public static class DataCollector
    {
        public static IEnumerable<AdvertismentModel> CollectFromOLX(int count = 0)
        {
            OLXDataCollector dataCollector = new OLXDataCollector();
            return dataCollector.Collect(count);
        }
    }
}
