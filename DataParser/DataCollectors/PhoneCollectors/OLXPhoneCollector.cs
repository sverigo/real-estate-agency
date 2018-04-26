using System;
using System.Linq;
using DataParser.Constants.Common;
using HtmlAgilityPack;
using DataParser.Extensions;
using DataParser.DataCollectors.AdsCollectors;

namespace DataParser.DataCollectors.PhoneCollectors
{
    internal class OLXPhoneCollector : AbstractPhoneCollector
    {
        private int tryCount;
        private System.Windows.Forms.Timer localTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer secondTimer = new System.Windows.Forms.Timer();

        private static OLXPhoneCollector instance;

        public static OLXPhoneCollector Instanse {
            get
            {
                if (instance != null) return instance;

                instance = new OLXPhoneCollector();
                return instance;
            }
        }

        private bool ClickButton()
        {
            var browser = WebBrowserController.Instance.Browser;

            var contactButton = browser.Document.GetElementsByTagName("div")?.OfType<System.Windows.Forms.HtmlElement>()
                                .Where(x => x.GetAttribute("className").Contains("contact-button")).FirstOrDefault();

            contactButton?.InvokeMember("click");
            return contactButton != null;
        }

        protected override void Initialize()
        {
            tryCount = 0;
            localTimer.Interval = 5000;
            secondTimer.Interval = 200;
            localTimer.Tick += LocalTimerTick;
            secondTimer.Tick += SecondTimerTick;
            localTimer.Start();
            secondTimer.Start();
        }

        protected override void Dispose()
        {
            localTimer.Stop();
            secondTimer.Stop();
            localTimer.Tick -= LocalTimerTick;
            secondTimer.Tick -= SecondTimerTick;
        }

        private void LocalTimerTick(object sender, EventArgs e)
        {
            tryCount++;
            var browser = WebBrowserController.Instance.Browser;
            WebBrowserController.Instance.Browser.Invoke(new Action(() => browser.Refresh()));

            if (tryCount > HelperConstants.MaxTryCount)
                resetEvent.Set();
        }

        private void SecondTimerTick(object sender, EventArgs e)
        {
            var browser = WebBrowserController.Instance.Browser;

            if (browser.Url != new Uri(currentUrl)) return;
            try
            {
                var doc = browser.Document;

                if (!ClickButton()) return;

                mshtml.IHTMLDocument2 current = (mshtml.IHTMLDocument2)browser.Document.DomDocument;

                HtmlDocument docer = new HtmlDocument();
                docer.LoadHtml(current.body.innerHTML);

                var strongNode = docer.DocumentNode.SelectNodes("//div[@class]").FirstWhere("class", OLXAdsCollector.contactButtonRegex).
                    SelectSingleNode(".//strong");
                if (!strongNode.InnerText.ToLower().Contains("x"))
                {
                    var spans = strongNode.SelectNodes("./span");
                    if (spans != null)
                    {
                        spans.ToList().ForEach(span => phones.Add(span.InnerText.Trim()));
                    }
                    else
                    {
                        phones.Add(strongNode.InnerText.Trim());
                    }

                    browser.Stop();
                    resetEvent.Set();
                }
            }
            catch (Exception)
            {
                // Nothing to do
            }
        }
    }
}
