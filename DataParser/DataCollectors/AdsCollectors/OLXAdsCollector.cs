using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using DataParser.Models;
using HtmlAgilityPack;
using DataParser.Extensions;
using DataParser.Constants.Common;
using DataParser.DataCollectors.PhoneCollectors;


namespace DataParser.DataCollectors.AdsCollectors
{
    internal class OLXAdsCollector : AbstractAdsCollector
    {
        public OLXAdsCollector(IEnumerable<string> pagesWithAd) : base(pagesWithAd) { }
        public OLXAdsCollector(string pageWithAd) : base(pageWithAd) { }

        private IEnumerable<string> phones;

        internal override IEnumerable<CollectedData> CollectAd()
        {
            List<CollectedData> data = new List<CollectedData>();
            OLXPhoneCollector phoneCollector = new OLXPhoneCollector();
            HtmlWeb web = new HtmlWeb();

            System.Diagnostics.Debug.WriteLine("Collecting ads...");
            pagesWithAds.ToList().ForEach(page =>
            {
                System.Diagnostics.Debug.WriteLine(page);
                System.Diagnostics.Debug.WriteLine("Collecting data...");
                try
                {
                    HtmlDocument document = web.Load(page);

                    var commonFields = CollectCommonData(document);
                    var variativeFieds = CollectVariativeData(document);
                    var images = CollectPhotos(document);

                    if (HasPhoneButton(document))
                    {
                        System.Diagnostics.Debug.WriteLine("Collecting phones...");
                        this.phones = phoneCollector.CollectPhone(page);
                    }

                    data.Add(new CollectedData(commonFields, variativeFieds, images, this.phones, page));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(page);
                    Debug.WriteLine(ex.Message);
                } 
            });

            return data;
        }

        private Dictionary<string, string> CollectCommonData(HtmlDocument document)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            try
            {
                var titleDiv = document.DocumentNode.SelectNodes("//div").FirstWhere("class", titleDivRegex);
                string title = titleDiv.SelectSingleNode("//h1").InnerText.Trim();

                dictionary.Add(DictionaryConstants.TitleKey, title);

                string address = titleDiv.SelectNodes(".//a[@class]").FirstWhere("class", addressLabelRegex).
                    SelectSingleNode("./strong").InnerText;

                dictionary.Add(DictionaryConstants.AddressKey, address);

                var priceDiv = document.DocumentNode.SelectNodes("//div[@class]").FirstWhere("class", priceDivRegex);
                string price = priceDiv.SelectSingleNode("./strong").InnerText.Trim();

                dictionary.Add(DictionaryConstants.PriceKey, price);

                var detailsDiv = document.DocumentNode.SelectNodes("//div[@id]").FirstWhere("id", detailsDivRegex);
                var spoilersHidden = detailsDiv.SelectNodes(".//span[@class]")?.Where("class", spoilerHiddenRegex);
                if (spoilersHidden != null)
                {
                    this.phones = spoilersHidden.Select(span => span.GetAttributeValue("data-phone", string.Empty).Insert(0, "("));
                    spoilersHidden.ToList().ForEach(span =>
                    {
                        span.PreviousSibling.Remove();
                        span.Remove();
                    });
                }
                string details = detailsDiv.SelectSingleNode("./p").InnerText.Trim();

                dictionary.Add(DictionaryConstants.DetailsKey, details);

                var userDiv = document.DocumentNode.SelectNodes(".//div[@class]").FirstWhere("class", userDivRegex);
                string userName = userDiv.SelectSingleNode("./h4").GetInnermostNode().InnerText.Trim();

                dictionary.Add(DictionaryConstants.AuthorNameKey, userName);
            }
            catch (Exception)
            {
                throw;
            }
            

            return dictionary;
        }
      

        private static Dictionary<string, string> CollectVariativeData(HtmlDocument document)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            try
            {
                var tBodyNode = document.DocumentNode.SelectNodes("//table[@class]").FirstWhere("class", detailsTableRegex);
                var tables = tBodyNode.SelectNodes("./tr").SelectMany(row => row.SelectNodes(".//table[@class]"));

                tables.ToList().ForEach(table =>
                {
                    string key = table.SelectSingleNode(".//th").InnerText.Trim();
                    var td = table.SelectSingleNode(".//td").GetInnermostNode();
                    string value = td.InnerText.Trim();

                    data.Add(key, value);
                });
            }
            catch (Exception)
            {
                throw;
            }
            
            return data;
        }

        private static IEnumerable<string> CollectPhotos(HtmlDocument document)
        {
            try
            {
                List<string> imgUrls = new List<string>();
                var imgDivs = document.DocumentNode.SelectNodes("//div[@class]").Where("class", imgDivRegex).ToList();
                return imgDivs.Select(div => div.SelectSingleNode(".//img")?.GetAttributeValue("src", string.Empty)).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private static bool HasPhoneButton(HtmlDocument document)
        {
            try
            {
                return document.DocumentNode.SelectNodes("//div[@class]").FirstWhere("class", contactButtonRegex) != null;
            }
            catch (Exception)
            {

                throw;
            } 
        }

        private static string titleDivRegex = string.Format(RegexConstants.RegexFullWordPattern, "offer-titlebox");
        private static string priceDivRegex = string.Format(RegexConstants.RegexFullWordPattern, "price-label");
        private static string addressLabelRegex = string.Format(RegexConstants.RegexFullWordPattern, "show-map-link");
        private static string detailsDivRegex = string.Format(RegexConstants.RegexFullWordPattern, "textContent");
        private static string userDivRegex = string.Format(RegexConstants.RegexFullWordPattern, "offer-user__details");

        private static string imgDivRegex = string.Format(RegexConstants.RegexFullWordPattern, "img-item");

        private static string detailsTableRegex = string.Format(RegexConstants.RegexFullWordPattern, "details");

        internal static string contactButtonRegex = string.Format(RegexConstants.RegexFullWordPattern, "contact-button");
        internal static string spoilerHiddenRegex = string.Format(RegexConstants.RegexFullWordPattern, "spoilerHidden");
    }
}
