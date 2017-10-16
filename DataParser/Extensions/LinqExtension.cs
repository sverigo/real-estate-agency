using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DataParser.Extensions
{
    internal static class LinqExtension
    {
        internal static IEnumerable<HtmlNode> Where(this IEnumerable<HtmlNode> collection, string attributeName, string regexPattern)
        {
            return collection.Where(node =>
            {
                if (!node.HasAttributes) return false;
                var attribute = node.GetAttributeValue(attributeName, string.Empty);
                return !string.IsNullOrEmpty(attribute) && Regex.IsMatch(attribute, regexPattern);
            });
        }

        internal static HtmlNode FirstWhere(this IEnumerable<HtmlNode> collection, string attributeName, string regexPattern)
        {
            return collection.Where(attributeName, regexPattern).FirstOrDefault();
        }
    }
}
