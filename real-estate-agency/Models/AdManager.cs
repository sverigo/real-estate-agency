using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.IO;
using real_estate_agency.Models;

namespace real_estate_agency.Models 
{
    public class AdManager
    {
        private RealEstateDBContext _db;
        private XmlSerializer formatter = new XmlSerializer(typeof(List<string>));

        public AdManager()
        {
            _db = new RealEstateDBContext();
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