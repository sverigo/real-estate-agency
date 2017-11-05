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
        public static IEnumerable<AdvertismentModel> CollectFromOLX(int count)
        {
            if (count == 0)
                throw new InvalidOperationException("Zero argument");

            OLXDataCollector dataCollector = new OLXDataCollector();
            return dataCollector.Collect(count);
        }

        /// <summary>
        /// Parses todays and yesterdays ads
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AdvertismentModel> CollectFromOLX()
        {
            OLXDataCollector dataCollector = new OLXDataCollector();
            return dataCollector.Collect(0);
        }
    }
}
