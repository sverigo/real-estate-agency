using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser.DataCollectors.PagesCollectors
{
    internal abstract class AbstractPagesCollector
    {
        internal IEnumerable<string> StartUris { get; private set; }

        internal AbstractPagesCollector(IEnumerable<string> startUris)
        {
            StartUris = startUris;
        }

        internal AbstractPagesCollector(string startUri) : this(new List<string> { startUri }) { }

        internal abstract IEnumerable<string> CollectPagesUri(int count = 0);
    }
}
