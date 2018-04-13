using System;
using System.Collections.Generic;
using System.Linq;
using DataParser.DataCollectors.PhoneCollectors;
using DataParser.Helpers;

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

        public string Title { get; internal set; }
        public string Price { get; internal set; }
        public string Details { get; internal set; }
        public string AuthorName { get; internal set; }
        public string Address { get; internal set; }
        public string PreviewImg { get; internal set; }
        public IEnumerable<string> Images { get; internal set; }
        public IEnumerable<string> Phones { get; internal set; }
        public SerializableDictionary<string, string> OtherData{ get; internal set;}

        public string Author { get; internal set; }
        public string ObjectType { get; internal set; }
        public string RoomCount { get; internal set; }
        public string Area { get; internal set; }
        public string Floor { get; internal set; }
        public string FloorCount { get; internal set; }
        public string Category { get; internal set; }

        internal Resource Resource {get; set;}
        internal bool HasPhone { get; set; }

        public IEnumerable<string> CollectPhones()
        {
            if (Phones != null && Phones.Count() > 0) return Phones;

            if (!HasPhone)
                return null;

            switch(Resource)
            {
                case Resource.OLX:
                    var collectedPhones = OLXPhoneCollector.Instanse.CollectPhone(this.url);

                    if(collectedPhones == null || collectedPhones.Count() == 0)
                    {
                        DataCollectors.PhoneCollectors.WebBrowserController.Instance.Dispose();
                        collectedPhones = OLXPhoneCollector.Instanse.CollectPhone(this.url);
                    }

                    this.Phones = collectedPhones;

                    return this.Phones;
                default:
                    break;
            }

            throw new InvalidOperationException("Invalid resource");
        }

    }
}
