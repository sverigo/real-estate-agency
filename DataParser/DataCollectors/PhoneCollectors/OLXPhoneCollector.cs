using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataParser.Constants.Common;
using HtmlAgilityPack;
using DataParser.Extensions;
using DataParser.DataCollectors.AdsCollectors;

namespace DataParser.DataCollectors.PhoneCollectors
{
    internal class OLXPhoneCollector : AbstractPhoneCollector
    {
        private bool found;
        private int tryCount;

        protected override void DocumentCompletedHandler(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            var browser = sender as System.Windows.Forms.WebBrowser;
            var doc = browser.Document;

            try
            {
                var contactButton = doc.GetElementsByTagName("div").OfType<System.Windows.Forms.HtmlElement>()
                                 .Where(x => x.GetAttribute("className").Contains("contact-button")).FirstOrDefault();

                if (contactButton != null && !found)
                {
                    found = true;
                    contactButton.InvokeMember("click");
                }

                if (found)
                {
                    tryCount++;
                    mshtml.IHTMLDocument2 current = (mshtml.IHTMLDocument2)browser.Document.DomDocument;

                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument docer = new HtmlDocument();
                    docer.LoadHtml(current.body.innerHTML);
                    current = null;

                    var strongNode = docer.DocumentNode.SelectNodes("//div[@class]").FirstWhere("class", OLXAdsCollector.contactButtonRegex).
                        SelectSingleNode(".//strong");
                    if (!strongNode.InnerText.ToLower().Contains("x"))
                    {
                        List<string> numbers = new List<string>();
                        var spans = strongNode.SelectNodes("./span");
                        if (spans != null)
                        {
                            spans.ToList().ForEach(span => numbers.Add(span.InnerText.Trim()));
                        }
                        else
                        {
                            numbers.Add(strongNode.InnerText.Trim());
                        }

                        phones = numbers;
                        System.Windows.Forms.Application.Exit();
                    }
                    else
                    {
                        found = false;
                    }

                    if (tryCount > HelperConstants.MaxTryCount)
                        System.Windows.Forms.Application.Exit();
                }
            }
            catch (Exception)
            {
                // Nothing to do
            }
            
        }
    }
}
