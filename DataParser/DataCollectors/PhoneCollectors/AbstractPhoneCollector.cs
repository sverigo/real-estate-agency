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
        //[DllImport("psapi.dll")]
        //private static extern bool EmptyWorkingSet(IntPtr hProcess);

        protected IEnumerable<string> phones;

        public virtual IEnumerable<string> CollectPhone(string url)
        {
            Console.WriteLine(url);
            var t = new Thread(() =>
            {
                try
                {
                    //System.Windows.Forms.Form form = new System.Windows.Forms.Form();

                    var thread = Thread.CurrentThread;
                    
                    var browser = new WebBrowser();
                    browser.ScriptErrorsSuppressed = true;
                    //browser.Dock = DockStyle.Fill;
                    //form.Controls.Add(browser);
                    //form.FormClosed += (o, e) =>
                    //{

                    //    var d = browser.Document;

                    //    //string gth = browser.DocumentText;
                    //    mshtml.IHTMLDocument2 current = (mshtml.IHTMLDocument2)browser.Document.DomDocument;
                    //    string s = current.body.innerText;
                    //    System.Windows.Forms.Application.Exit();
                    //};
                    //form.Show();
                    browser.AllowNavigation = true;
                    browser.Navigate(url);
                    browser.Refresh();

                    browser.DocumentCompleted += DocumentCompletedHandler;

                    Application.Run();
                    browser.DocumentCompleted -= DocumentCompletedHandler;
                    //browser.Dispose();
                    //browser = null;
                    //System.GC.Collect();
                    //System.GC.WaitForPendingFinalizers();
                    //System.GC.Collect();

                    //EmptyWorkingSet(Process.GetCurrentProcess().Handle);
                }
                catch (Exception)
                {

                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            t.Join();

            return phones;
        }

        protected abstract void DocumentCompletedHandler(object sender, WebBrowserDocumentCompletedEventArgs e);
    }
}
