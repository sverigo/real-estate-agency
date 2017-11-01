﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using DataParser.Constants.Common;
using DataParser.Extensions;

namespace DataParser.DataCollectors.PagesCollectors
{
    internal class OLXPagesCollector : AbstractPagesCollector
    {
        private static string regexPagerPattern = string.Format(RegexConstants.RegexFullWordPattern, "pager");
        private static string regexNextPattern = string.Format(RegexConstants.RegexFullWordPattern, "next");
        private static string regexOfferPattern = string.Format(RegexConstants.RegexFullWordPattern, "offer");

        internal OLXPagesCollector(IEnumerable<string> startUris) : base(startUris) { }
        internal OLXPagesCollector(string startUri) : base(startUri) { }

        internal override IEnumerable<string> CollectPagesUri(int count = 0)
        {
            HtmlWeb web = new HtmlWeb();
            List<string> links = new List<string>();

            System.Diagnostics.Debug.WriteLine("Collecting pages...");

            StartUris.ToList().ForEach(startPage =>
            {
                var htmlDoc = web.Load(startPage);
                bool isRunning = true;

                int previousNumber = GetPageNumber(GetNextNavLink(htmlDoc));

                while (isRunning)
                {
                    string nextLink = GetNextNavLink(htmlDoc);
                    int pageNumber = GetPageNumber(nextLink);

                    if (pageNumber < previousNumber)
                        break;

                    previousNumber = pageNumber;

                    htmlDoc?.DocumentNode.SelectSingleNode("//table[@id='offers_table']").SelectNodes("//td").Where("class", string.Format(regexOfferPattern, "offer")).ToList().ForEach(offer =>
                    {
                        var aNode = offer.SelectSingleNode(".//a");

                        if (count == 0 || links.Count < count)
                            links.Add(aNode.GetAttributeValue("href", string.Empty));
                        else
                            isRunning = false;
                    });

                    System.Diagnostics.Debug.WriteLine(nextLink);

                    if (string.IsNullOrEmpty(nextLink))
                        isRunning = false;
                    
                    if(isRunning)
                        htmlDoc = web.Load(nextLink);
                }
            });

            return links;
        }

        private static string GetNextNavLink(HtmlDocument htmlDoc)
        {
            var node = htmlDoc.DocumentNode.SelectNodes("//div[@class]").FirstWhere("class", regexPagerPattern)?.
                SelectNodes("./span[@class]").Where("class", regexNextPattern).FirstOrDefault()?.SelectSingleNode("./a[@href]");

            return node == null ? string.Empty : node.GetAttributeValue("href", string.Empty);
        }

        private static int GetPageNumber(string uri)
        {
            var queryString = new Uri(uri).Query;
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(queryString);

            string index = queryDictionary.Get("page");

            return int.Parse(index);
        }
    }
}
