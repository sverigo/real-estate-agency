using System.Collections.Generic;
using System.Linq;
using real_estate_agency.Models;

namespace real_estate_agency.Infrastructure
{
    public class TypeManager
    {
        AppIdentityDBContext dataBase;

        public TypeManager()
        {
            dataBase = new AppIdentityDBContext();
        }

        public void AddType(Types type)
        {
            dataBase.Types.Add(type);
            dataBase.SaveChanges();
        }

        public Types EditType(Types type)
        {
            dataBase.Entry(type).State = System.Data.Entity.EntityState.Modified;
            dataBase.SaveChanges();
            return null;
        }

        public void RemoveType(Types type)
        {
            dataBase.Types.Remove(type);
            dataBase.SaveChanges();
        }

        public Types FindType()
        {
            return null;
        }

        public static List<Types> GetTypeList()
        {
            List<Types> result = new List<Types>();
            AppIdentityDBContext db = new AppIdentityDBContext();

            result = db.Types.ToList();
            
            return result;
        }
    }
}