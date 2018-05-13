using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace real_estate_agency.Infrastructure
{
    /// <summary>
    /// Класс для парсинга курса валют по данным НБУ
    /// </summary>
    public class CurrencyExchangeParser
    {
        static string url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";

        /// <summary>
        /// Возвращает значения курсов валют
        /// </summary>
        public static Dictionary<string, double> GetCurrencyExchangeRate()
        {
            Dictionary<string, double> course = new Dictionary<string, double>();
            string[] currencyList = { "EUR", "USD", "RUB" };
            try
            {
                CurrencyExchangeObject[] parsed = JsonConvert.DeserializeObject<CurrencyExchangeObject[]>(
                new System.Net.WebClient().DownloadString(url));
                foreach (CurrencyExchangeObject obj in parsed)
                {
                    foreach (string currency in currencyList)
                    {
                        if (obj.Cc == currency)
                        {
                            course[obj.Cc] = obj.Rate;
                        }
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                return null;
            }
            return course;
        }

    }

    public class CurrencyExchangeObject
    {
        public int R030 { get; set; }
        public string Txt { get; set; }
        public double Rate { get; set; }
        public string Cc { get; set; }
        public string ExchangeDate { get; set; }
    }

}