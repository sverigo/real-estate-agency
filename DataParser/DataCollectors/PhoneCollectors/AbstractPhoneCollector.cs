﻿using System;
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
        private WebBrowser browser;

        public virtual IEnumerable<string> CollectPhone(string url)
        {
            Console.WriteLine(url);
            var t = new Thread(() =>
            {
                try
                {
                    //System.Windows.Forms.Form form = new System.Windows.Forms.Form();

                    var thread = Thread.CurrentThread;
                    
                    browser = new WebBrowser();
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
                    var timer = new System.Windows.Forms.Timer();

                    timer.Interval = 5000;
                    timer.Tick += TimerTickHandler;
                    timer.Start();

                    Application.Run();

                    timer.Stop();
                    timer.Tick -= TimerTickHandler;
                    browser.DocumentCompleted -= DocumentCompletedHandler;
                    browser = null;
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

        private void TimerTickHandler(object sender, EventArgs handler)
        {
            if (phones == null)
                browser.Refresh();
        }

        protected abstract void DocumentCompletedHandler(object sender, WebBrowserDocumentCompletedEventArgs e);
    }
}
