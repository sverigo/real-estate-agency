﻿using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using real_estate_agency.Infrastructure;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;

namespace real_estate_agency.Models
{
    public class AppIdentityDBContext: IdentityDbContext<AppUser>
    {
        public virtual DbSet<Ad> Ads { get; set; }
        public virtual DbSet<MarkedAd> MarkedAds { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Price> Prices { get; set; }
        public virtual DbSet<Types> Types { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CurrencyRate> CurrencyRates { get; set; }

        public AppIdentityDBContext():base("RealEstateAgencyDB") { }

        static AppIdentityDBContext()
        {
            Database.SetInitializer(new IdentityDBInit());
        }

        public static AppIdentityDBContext Create()
        {
            return new AppIdentityDBContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<IdentityUserLogin>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.Entity<IdentityRole>().HasKey(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
        }
    }

    public class IdentityDBInit: DropCreateDatabaseIfModelChanges<AppIdentityDBContext>
    {
        protected override void Seed(AppIdentityDBContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        public void PerformInitialSetup(AppIdentityDBContext context)
        {
            AppUserManager userMng = new AppUserManager(new UserStore<AppUser>(context));
            AppRoleManager roleMng = new AppRoleManager(new RoleStore<AppRole>(context));

            string[] roleNames = new string[] 
            {
                UserStatusDirectory.Roles.ADMINS,
                UserStatusDirectory.Roles.MODERATORS,
                UserStatusDirectory.Roles.PREMIUM_USER,
                UserStatusDirectory.Roles.USERS
            };
            
            string login = WebConfigurationManager.AppSettings["AdminLogin"];
            string name = WebConfigurationManager.AppSettings["AdminName"];
            string pass = WebConfigurationManager.AppSettings["AdminPass"];
            string email = WebConfigurationManager.AppSettings["AdminMail"];
            
            foreach(string role in roleNames)
                if (!roleMng.RoleExists(role))
                    roleMng.Create(new AppRole(role));

            AppUser user = userMng.FindByName(login);
            if (user == null)
            {
                userMng.Create(new AppUser()
                {
                    UserName = login,
                    Name = name,
                    Email = email,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                },
                pass);

                user = userMng.FindByName(login);
            }

            if (!userMng.IsInRole(user.Id, UserStatusDirectory.Roles.ADMINS))
                userMng.AddToRole(user.Id, UserStatusDirectory.Roles.ADMINS);

            Types[] types =
            {
                new Types {Name = "Квартира", IsOwn = true },
                new Types {Name = "Комната", IsOwn = true },
                new Types {Name = "Дом", IsOwn = true }
            };
            Category[] categories =
            {
                new Category {Name = "Почасово", IsOwn = true },
                new Category {Name = "Посуточно", IsOwn = true },
                new Category {Name = "На долгий срок", IsOwn = true }
            };
            foreach(Types t in types)
            {
                context.Types.Add(t);
            }
            foreach (Category c in categories)
            {
                context.Categories.Add(c);
            }

            context.CurrencyRates.Add(new CurrencyRate { NameCode = "UAH", Rate = 1.0, LastUpdate = DateTime.Now });
            CurrencyOperations.UpdateCurrentExchangeRate();
            
            context.SaveChanges();
        }
    }
}