using System;
using System.Collections.Generic;
using System.Linq;
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
            var adToDelete = FindById(id);
            dataBase.Ads.Remove(adToDelete);
            dataBase.SaveChanges();
        }

        public void LoadAds()
        {
            var result = DataCollector.CollectFromOLX(10);

            var preparedEntities = result.Select(adModel =>
            {
                Ad newAd = new Ad()
                {
                    Title = adModel.Title,
                    Type = adModel.ObjectType,
                    Category = adModel.Category,
                    Address = adModel.Address,
                    Area = adModel.Area,
                    Author = adModel.AuthorName,
                    Floors = Convert.ToInt32(adModel.Floor),
                    FloorsCount = Convert.ToInt32(adModel.FloorCount),
                    RoomsCount = Convert.ToInt32(adModel.RoomCount),
                    Images = StringImgPhoneConverter.ListToString(adModel.Images?.ToList()),
                    Phone = StringImgPhoneConverter.ListToString(adModel.Phones?.ToList()),
                    Value = CurrencySepatator.SeparateValue(adModel.Price),
                    Currency = CurrencySepatator.SeparateCurrency(adModel.Price),
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
            Ad currentAd = FindById(id);

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

        /// <summary>
        /// Get all ads of specified user.
        /// If nothing is found, returns null.
        /// </summary>
        /// <param name="id">AppUser's id.</param>
        /// <returns></returns>
        public IEnumerable<Ad> GetAdsByUserAthorId(string id)
        {
            IEnumerable<Ad> ads = dataBase.Ads.Where(ad => ad.UserAuthorId == id).ToList();
            return ads;
        }

        /// <summary>
        /// Get all ads marked by specified user.
        /// If nothing is found, returns null.
        /// </summary>
        /// <param name="id">AppUser's id.</param>
        /// <returns></returns>
        public IEnumerable<Ad> GetMarkedAdsByUserAthorId(string id)
        {
            IEnumerable<Ad> ads = dataBase.MarkedAds
                .Where(mk => mk.AppUserId == id)
                .Select(x => x.Ad).ToList();
            
            return ads;
        }

        public void SetMark(int adId, string userId)
        {
            Ad ad = FindById(adId);
            dataBase.MarkedAds.Add(new MarkedAd
            {
                AppUserId = userId,
                Ad = ad
            });
            dataBase.SaveChanges();
        }

        public void RemoveMark(int adId, string userId)
        {
            Ad ad = FindById(adId);
            var marked = ad.UsersMarked.Where(mk => mk.AppUserId == userId).FirstOrDefault();
            dataBase.MarkedAds.Remove(marked);
            dataBase.SaveChanges();
        }
    }
}