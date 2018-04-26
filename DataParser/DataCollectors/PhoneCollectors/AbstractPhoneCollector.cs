using System;
using System.Collections.Generic;
using System.Threading;

namespace DataParser.DataCollectors.PhoneCollectors
{
    abstract class AbstractPhoneCollector
    {      
        protected List<string> phones;
        
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
                phones = new List<string>();
                currentUrl = url;
                resetEvent.Reset();
                var controller = WebBrowserController.Instance;

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
            }
            
            return this.phones;
        }

        protected abstract void Initialize();
        protected abstract void Dispose();
    }
}
