using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser.DataConverters.Date
{
    internal interface IDateConverter
    {
        DateTime Convert(string data);
    }
}
