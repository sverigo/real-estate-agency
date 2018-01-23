using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using real_estate_agency.Models;
using DataParser;
using System.Data.Entity;

namespace real_estate_agency.Infrastructure
{
    public class AdsManager
    {
        RealEstateDBContext dataBase;

        public IEnumerable<Ad> AllAds
        {
            get { return dataBase.Ads.AsEnumerable(); }
        }

        public AdsManager()
        {
            dataBase = new RealEstateDBContext();
        }

        public void AddNewAd(Ad ad)
        {
            dataBase.Ads.Add(ad);
            dataBase.SaveChanges();
        }

        public void Edit(Ad ad)
        {
            dataBase.Entry(ad).State = EntityState.Modified;
            dataBase.SaveChanges();
        }

        public void RemoveById(int id)
        {
            if (id <= 0)
                throw new Exception("Invalid ID");

            var adToDelete = dataBase.Ads.Where(ad => ad.Id == id).FirstOrDefault();
            dataBase.Ads.Remove(adToDelete);
            dataBase.SaveChanges();
        }

        public void LoadAds()
        {
            var result = DataCollector.CollectFromOLX(1);

            var preparedEntities = result.Select(adModel =>
            {
                Ad newAd = new Ad()
                {
                    Title = adModel.Title,
                    Type = adModel.AdType,
                    Address = adModel.Address,
                    Area = adModel.Area,
                    Author = adModel.AuthorName,
                    Floors = adModel.Floor,
                    FloorsCount = adModel.FloorCount,
                    RoomsCount = adModel.RoomCount,
                    Images = StringImgPhoneConverter.ListToString(adModel.Images?.ToList()),
                    Phone = StringImgPhoneConverter.ListToString(adModel.Phones?.ToList()),
                    Value = adModel.Price,
                    Details = adModel.Details,
                    PrevImage = adModel.PreviewImg,
                    AdUrl = adModel.Url
                };

                return newAd;
            });

            try
            {
                dataBase.Ads.AddRange(preparedEntities);
                dataBase.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                throw ex;
            }
        }

        public Ad FindById(int? id)
        {
            if (id == null || id <= 0)
                return null;
            else
            {
                Ad ad = dataBase.Ads.Where(a => a.Id == id).FirstOrDefault();
                return ad;
            }
        }

        public List<string> GetPhonesForAd(int id)
        {
            List<string> phoneList;
            Ad currentAd = dataBase.Ads.Where(ad => ad.Id == id).FirstOrDefault();

            phoneList = StringImgPhoneConverter.StringToList(currentAd.Phone);

            if(phoneList.Count == 0)
            {
                var parserModel = new DataParser.Models.AdvertismentModel(currentAd.AdUrl);
                phoneList = parserModel.CollectPhones()?.ToList() ?? new List<string>();

                if (phoneList.Count == 0)
                    return phoneList;
                else
                {
                    currentAd.Phone = StringImgPhoneConverter.ListToString(phoneList);
                    dataBase.Entry(currentAd).State = EntityState.Modified;
                    dataBase.SaveChanges();
                }
            }

            return phoneList;
        }
    }
}