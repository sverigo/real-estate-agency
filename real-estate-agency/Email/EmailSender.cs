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

namespace real_estate_agency.Email
{
    public class EmailSender
    {
        static string seviceEmail;
        static string password;

        static EmailSender()
        {
            seviceEmail = "filenkoandrij@gmail.com";
            password = "aa12345aa";
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
                $"<a href=\"http://localhost:53687{link}\">Активировать!</a>";

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
                $"<a href=\"http://localhost:53687{link}\">ссылке</a>";

            SendMail(user, message);
        }
    }
}