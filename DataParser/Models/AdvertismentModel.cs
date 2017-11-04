using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser.Models
{
    public class AdvertismentModel
    {
        public string Url { get; set; }
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
    }
}
