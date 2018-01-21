using System;
using System.Collections.Generic;
using System.Linq;
using DataParser.DataCollectors.PhoneCollectors;

namespace DataParser.Models
{
    internal enum Resource { NULL = 0, OLX }
    public class AdvertismentModel
    {
        public AdvertismentModel()
        {

        }

        public AdvertismentModel(string url)
        {
            Url = url;
        }

        private string url;

        public string Url { get { return url; }
            set
            {
                if (value.Contains("olx.ua"))
                    Resource = Resource.OLX;

                url = value;
            }
        }

        public string Title { get; set; }
        public string Price { get; set; }
        public string Details { get; set; }
        public string AuthorName { get; set; }
        public string Address { get; set; }
        public string PreviewImg { get; set; }
        public IEnumerable<string> Images { get; set; }
        public IEnumerable<string> Phones { get; set; }

        public string Author { get; set; }
        public string AdType { get; set; }
        public string RoomCount { get; set; }
        public string LiveArea { get; set; }
        public string Area { get; set; }
        public string Floor { get; set; }
        public string FloorCount { get; set; }
        public string Type { get; set; }
        public string BeginData { get; set; }

        internal Resource Resource {get; set;}

        public IEnumerable<string> CollectPhones()
        {
            if (Phones != null && Phones.Count() > 0) return Phones;

            switch(Resource)
            {
                case Resource.OLX:
                    this.Phones = OLXPhoneCollector.Instanse.CollectPhone(this.url);
                    return this.Phones;
                default:
                    break;
            }

            throw new InvalidOperationException("Invalid resource");
        }
    }
}
