using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.IO;

namespace real_estate_agency.Models 
{
    public class AdManager
    {
        private RealEstateDBEntities _db;
        private XmlSerializer formatter = new XmlSerializer(typeof(List<string>));

        public AdManager()
        {
            _db = new RealEstateDBEntities();
        }

        public IEnumerable<Ad> GetItems()
        {
            return _db.Ads.ToList();
        }

        public void AddAd()
        {

        }
    }
}