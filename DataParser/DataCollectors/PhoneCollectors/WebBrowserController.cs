using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DataParser.DataCollectors.PhoneCollectors
{
    internal class WebBrowserController
    {
        internal WebBrowser Browser { get; private set; }
        private static WebBrowserController instance;
        private Thread thread;

        internal static WebBrowserController Instance
        {
            get
            {
                if (instance != null) return instance;

                instance = new WebBrowserController();

                var local = new AutoResetEvent(false);
                var t = new Thread(() =>
                {
                    try
                    {
                        //System.Windows.Forms.Form form = new System.Windows.Forms.Form();

                        instance.thread = Thread.CurrentThread;

                        instance.Browser = new WebBrowser();
                        instance.Browser.ScriptErrorsSuppressed = true;
                        instance.Browser.Dock = DockStyle.Fill;
                        //form.Controls.Add(instance.Browser);
                        //form.FormClosed += (o, e) =>
                        //{

                        //    var d = browser.Document;

                        //    //string gth = browser.DocumentText;
                        //    mshtml.IHTMLDocument2 current = (mshtml.IHTMLDocument2)browser.Document.DomDocument;
                        //    string s = current.body.innerText;
                        //    System.Windows.Forms.Application.Exit();
                        //};
                        //form.Show();
                        instance.Browser.AllowNavigation = true;

                        local.Set();
                        Application.Run();
                    }
                    catch (Exception)
                    {

                    }
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                local.WaitOne();

                return instance;
            }
        }

        internal void Dispose()
        {
            instance.Browser.Invoke(new Action(() => Application.Exit()));
            instance = null;
        }
    }
}
