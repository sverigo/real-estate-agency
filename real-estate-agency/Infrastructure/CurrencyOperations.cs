﻿using System;
using System.Collections.Generic;
using System.Linq;
using real_estate_agency.Models;

namespace real_estate_agency.Infrastructure
{
    public class CurrencyOperations
    {
        public static List<CurrencyRate> GetCurrentExchangeRate()
        {
            List<CurrencyRate> result = new List<CurrencyRate>();
            AppIdentityDBContext db = new AppIdentityDBContext();

            result = db.CurrencyRates.ToList();
            
            return result;
        }
        
        public static void UpdateCurrentExchangeRate()
        {
            AppIdentityDBContext db = new AppIdentityDBContext();
            Dictionary<string, double> parsed = CurrencyExchangeParser.GetCurrencyExchangeRate();
            
            if (parsed != null)
            {
                // clear old data
                db.CurrencyRates.RemoveRange(db.CurrencyRates);

                foreach (string key in parsed.Keys)
                {
                    db.CurrencyRates.Add(new CurrencyRate
                    {
                        NameCode = key,
                        Rate = parsed[key],
                        LastUpdate = DateTime.Now
                    });
                    db.SaveChanges();
                }
            }
        }

        public static double GetRateForCurrency(string currency)
        {
            AppIdentityDBContext db = new AppIdentityDBContext();
            List<CurrencyRate> list = db.CurrencyRates.ToList();

            foreach(CurrencyRate c in list)
            {
                if(c.NameCode == currency)
                {
                    return c.Rate;
                }
            }
            return 1;
        }

        public static double GetСoefficient(string fromCurrency, string toCurrency)
        {
            double from = GetRateForCurrency(fromCurrency);
            double to = GetRateForCurrency(toCurrency);
            
            return from/to;
            
        }
    }
}