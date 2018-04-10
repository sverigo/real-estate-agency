using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Security.Policy;
using System.Web.Mvc;
using real_estate_agency.Models;
using real_estate_agency.Models.ViewModels;
using System.Web.Configuration;

namespace real_estate_agency.Email
{
    public class EmailSender
    {
        static string seviceEmail;
        static string password;

        static EmailSender()
        {
            seviceEmail = WebConfigurationManager.AppSettings["Mail"];
            password = WebConfigurationManager.AppSettings["Pass"];
        }

        private static void SendMail(AppUser user, MailMessage message)
        {
            using (SmtpClient cilent = new SmtpClient("smtp.gmail.com", 587))
            {
                cilent.Credentials = new NetworkCredential()
                {
                    UserName = seviceEmail,
                    Password = password
                };
                cilent.EnableSsl = true;
                
                cilent.Send(message);
            }
        }

        public static void SendConfirmEmail(AppUser user, string link)
        {
            MailMessage message = new MailMessage(
                    new MailAddress(seviceEmail, "Real Estate Agency"), new MailAddress(user.Email))
            {
                IsBodyHtml = true,
                Subject = "Подтверждение регистрации"
            };
            message.Body = $"Здравствуйте, {user.Name}<br>" +
                $"Для завершения регистрации активируйте Вашу учетную запись.<br>" +
                $"<a href=\"{link}\">Активировать!</a>";

            SendMail(user, message);
        }

        public static void SendPasswordResetMail(AppUser user, string link)
        {
            MailMessage message = new MailMessage(
                    new MailAddress(seviceEmail, "Real Estate Agency"), new MailAddress(user.Email))
            {
                IsBodyHtml = true,
                Subject = "Смена пароля"
            };
            message.Body = $"Здравствуйте, {user.Name}.<br>" +
                $"Для смены пароля перейдите по " +
                $"<a href=\"{link}\">ссылке</a>";

            SendMail(user, message);
        }

        public static void SendUserBlockedMail(AppUser user, BlockDuration duration, string cause, AppUser sender = null)
        {
            MailMessage message = new MailMessage(
                    new MailAddress(seviceEmail, "Real Estate Agency"), new MailAddress(user.Email))
            {
                IsBodyHtml = true,
                Subject = "Блокировка учетной записи"
            };
            string durationText = duration == BlockDuration.Forever ? "навсегда" : "до " + user.LockoutEndDateUtc;
            message.Body = $"Здравствуйте, {user.Name}.<br>" +
                $"Ваша учетная запись была заблокирована {durationText}.<br>" +
                $"Причина: {cause}.<br>";
            message.Body += sender != null ? $"Блокировку произвел: {sender.UserName} | {sender.Name}" : "";

            SendMail(user, message);
        }
    }
}