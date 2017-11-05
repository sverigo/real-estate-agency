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
        private int tryCount;
        
        protected IEnumerable<string> phones;
        
        protected AutoResetEvent resetEvent = new AutoResetEvent(false);
        private static object locker = new object();

        protected AbstractPhoneCollector()
        {
            
        }

        internal IEnumerable<string> CollectPhone(string url)
        {
            lock(locker)
            {
                resetEvent.Reset();
                var controller = WebBrowserController.Instance;

                controller.Browser.DocumentCompleted += DocumentCompletedHandler;
                controller.Timer.Interval = 6000;
                controller.Timer.Tick += TimerTickHandler;

                controller.Browser.Invoke(new Action(() =>
                {
                    controller.Browser.Navigate(url);
                    controller.Timer.Start();
                }
               ));

                resetEvent.WaitOne();

                controller.Browser.Invoke(new Action(() =>
                {
                    controller.Browser.Stop();
                    controller.Timer.Stop();
                }
               ));

                controller.Browser.DocumentCompleted -= DocumentCompletedHandler;
                controller.Timer.Tick -= TimerTickHandler;
            }
            
            return this.phones;
        }

        private void TimerTickHandler(object sender, EventArgs handler)
        {
            if (phones == null)
                WebBrowserController.Instance.Browser.Refresh();

            tryCount++;
            if (tryCount > Constants.Common.HelperConstants.MaxTryCount)
                resetEvent.Set();
        }

        protected abstract void DocumentCompletedHandler(object sender, WebBrowserDocumentCompletedEventArgs e);
    }
}
