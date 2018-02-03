using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using real_estate_agency.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using real_estate_agency.Infrastructure;
using Microsoft.AspNet.Identity;

namespace real_estate_agency.Models
{
    public class AppIdentityDBContext: IdentityDbContext<AppUser>
    {
        public virtual DbSet<Ad> Ads { get; set; }

        public AppIdentityDBContext(): base("RealEstateAgencyDB") { }

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

            string roleName = "Admins";
            string login = "Admin";
            string name = "Administrator";
            string pass = "adminpass1";
            string email = "admin@gmail.com";

            if (!roleMng.RoleExists(roleName))
                roleMng.Create(new AppRole(roleName));

            AppUser user = userMng.FindByName(login);
            if (user == null)
            {
                userMng.Create(new AppUser()
                {
                    UserName = login,
                    Name = name,
                    Email = email
                },
                pass);

                user = userMng.FindByName(login);
            }

            if (!userMng.IsInRole(user.Id, roleName))
                userMng.AddToRole(user.Id, roleName);
        }
    }
}