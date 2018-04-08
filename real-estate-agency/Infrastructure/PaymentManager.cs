using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using real_estate_agency.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace real_estate_agency.Infrastructure
{
    public struct PaymentData
    {
        public string Data;

        public string Signature;

        public Payment Payment;
    }

    public class PaymentManager
    {
        string privateKey;
        string publicKey;
        AppIdentityDBContext dataBase;

        private AppUserManager UserManager
        {
            get { return HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        public List<Price> Prices
        {
            get { return dataBase.Prices.ToList(); }
        }

        public PaymentManager()
        {
            dataBase = new AppIdentityDBContext();
            privateKey = WebConfigurationManager.AppSettings["PrivateKey"];
            publicKey = WebConfigurationManager.AppSettings["PublicKey"];
        }

        public PaymentData CreatePayment(AppUser user, int days, decimal amount,
            string callBackUrl, string resultUrl)
        {
            Payment payment = new Payment
            {
                Days = days,
                Date = DateTime.UtcNow,
                Amount = amount,
                User = user
            };

            user.Payments.Add(payment);
            IdentityResult result = UserManager.Update(user);
            if (!result.Succeeded)
                throw new Exception(result.Errors.Aggregate((e1, e2) => e1 + "|" + e2));

            JObject json = new JObject();
            json["verison"] = 3;
            json["public_key"] = publicKey;
            json["action"] = "pay";
            json["amount"] = amount;
            json["currency"] = "UAH";
            json["description"] = "Покупка премиум подписки.";
            json["order_id"] = payment.Id;
            json["sandbox"] = 1;
            json["server_url"] = callBackUrl;
            json["result_url"] = resultUrl;
            
            PaymentData paymentData = new PaymentData();
            paymentData.Payment = payment;
            paymentData.Data = Base64Encode(json.ToString());
            paymentData.Signature = GetBase64EncodedSHA1Hash(privateKey + paymentData.Data + privateKey);

            return paymentData;
        }

        public void ConfirmPayment(string data, string signature)
        {
            if (signature != GetBase64EncodedSHA1Hash(privateKey + data + privateKey))
            {

            }
            else
            {
                JObject json = JObject.Parse(data);
                int orderId = (int)json["order_id"];
                Payment payment = dataBase.Payments.Where(p => p.Id == orderId).FirstOrDefault();
                payment.ConfirmedDate = DateTime.UtcNow;
                payment.Status = (string)json["status"];
                dataBase.SaveChanges();
            }
        }

        string GetBase64EncodedSHA1Hash(string str)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream stream = new MemoryStream(byteArray);
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                return Convert.ToBase64String(sha1.ComputeHash(stream));
            }
        }

        string Base64Encode(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        string Base64Decode(string str)
        {
            byte[] bytes = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}