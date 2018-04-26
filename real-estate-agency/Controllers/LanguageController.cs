using System;
using System.Threading;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace real_estate_agency.Controllers
{
    public class LanguageController : Controller
    {
        public ActionResult Change(String LanguageAbbreviation)
        {
            if (LanguageAbbreviation != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(LanguageAbbreviation);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageAbbreviation);
            }

            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = LanguageAbbreviation;
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Home");
        }
    }
}