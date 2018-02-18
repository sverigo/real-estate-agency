﻿using real_estate_agency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace real_estate_agency.Infrastructure
{
    public class FilterOptions
    {
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public string Currency { get; set; }
        public int MinRoomsCount { get; set; }
        public int MaxRoomsCount { get; set; }
        public int MinArea { get; set; }
        public int MaxArea { get; set; }
        public int MinFloor { get; set; }
        public int MaxFloor { get; set; }

        public IEnumerable<Ad> getRoomsFilter(IEnumerable<Ad> ads)
        {
            if (MinRoomsCount == 0 && MaxRoomsCount == 0)
                return ads;
            else if (MaxRoomsCount == 0)
                return (from i in ads where i.RoomsCount >= MinRoomsCount select i);
            else if (MinRoomsCount == 0)
                return (from i in ads where i.RoomsCount <= MaxRoomsCount select i);
            else
                return (from i in ads where i.RoomsCount >= MinRoomsCount && i.RoomsCount <= MaxRoomsCount select i);
        }

        public IEnumerable<Ad> getQuantityFilter(IEnumerable<Ad> ads)
        {
            ads = from i in ads /*where i.Currency == Currency*/ select i;

            if (MinQuantity == 0 && MaxQuantity == 0)
                return ads;
            else if (MaxQuantity == 0)
                return (from i in ads where i.Value >= MinQuantity select i);
            else if (MinQuantity == 0)
                return (from i in ads where i.Value <= MaxQuantity select i);
            else
                return (from i in ads where i.Value >= MinQuantity && i.Value <= MaxQuantity select i);
        }

        public IEnumerable<Ad> getAreaFilter(IEnumerable<Ad> ads)
        {
            // TBA
            return null;
        }

        public IEnumerable<Ad> getFloorFilter(IEnumerable<Ad> ads)
        {
            if (MinFloor == 0 && MaxFloor == 0)
                return ads;
            else if (MaxFloor == 0)
                return (from i in ads where i.Floors >= MinFloor select i);
            else if (MinFloor == 0)
                return (from i in ads where i.Floors <= MaxFloor select i);
            else
                return (from i in ads where i.Floors >= MinFloor && i.Floors <= MaxFloor select i);
        }
    }
}