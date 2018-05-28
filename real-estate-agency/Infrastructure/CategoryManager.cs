using System.Collections.Generic;
using System.Linq;
using real_estate_agency.Models;

namespace real_estate_agency.Infrastructure
{
    public class CategoryManager
    {
        AppIdentityDBContext dataBase;

        public CategoryManager()
        {
            dataBase = new AppIdentityDBContext();
        }

        public void AddType(Category category)
        {
            dataBase.Categories.Add(category);
            dataBase.SaveChanges();
        }

        public Types EditType(Category category)
        {
            dataBase.Entry(category).State = System.Data.Entity.EntityState.Modified;
            dataBase.SaveChanges();
            return null;
        }

        public void RemoveType(Category category)
        {
            dataBase.Categories.Remove(category);
            dataBase.SaveChanges();
        }

        public Types FindType()
        {
            return null;
        }

        public static List<Category> GetCategoryList()
        {
            List<Category> result = new List<Category>();
            AppIdentityDBContext db = new AppIdentityDBContext();

            result = db.Categories.ToList();

            return result;
        }
    }
}