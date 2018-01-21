using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DataParser.DataCollectors.PhoneCollectors
{
    abstract class AbstractPhoneCollector
    {      
        protected IEnumerable<string> phones;
        
        protected AutoResetEvent resetEvent = new AutoResetEvent(false);
        private static object locker = new object();
        protected string currentUrl;

        protected AbstractPhoneCollector()
        {
            
        }

        internal IEnumerable<string> CollectPhone(string url)
        {
            lock(locker)
            {
                currentUrl = url;
                resetEvent.Reset();
                var controller = WebBrowserController.Instance;

                controller.Browser.DocumentCompleted += DocumentCompletedHandler;

                controller.Browser.Invoke(new Action(() =>
                    {
                        controller.Browser.Navigate(url);
                        Initialize();
                    }
                ));

                
                resetEvent.WaitOne();

                controller.Browser.Invoke(new Action(() =>
                {
                    controller.Browser.Stop();
                    Dispose();
                }
               ));

                controller.Browser.DocumentCompleted -= DocumentCompletedHandler;
            }
            
            return this.phones;
        }

        private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
           
            throw new NotImplementedException();
        }

        protected abstract void Initialize();
        protected abstract void Dispose();
        protected abstract void DocumentCompletedHandler(object sender, WebBrowserDocumentCompletedEventArgs e);
    }
}
