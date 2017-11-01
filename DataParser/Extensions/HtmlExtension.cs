using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DataParser.Extensions
{
    internal static class HtmlExtension
    {
        internal static HtmlNode GetInnermostNode(this HtmlNode node)
        {
            HtmlNode currentNode = node;
            while (currentNode.SelectSingleNode(".//node()[not(self::text())]") != null)
            {
                currentNode = currentNode.SelectSingleNode(".//node()[not(self::text())]");
            }

            return currentNode;
        }
    }
}
