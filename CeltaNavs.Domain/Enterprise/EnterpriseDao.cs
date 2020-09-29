using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain
{
    public class EnterpriseDao : Persistent
    {
        public List<ModelEnterprise> GetAll()
        {
            return context.Enterprises.ToList();
        }

        public ModelEnterprise Get(string _enterpriseId)
        {
            int id = Convert.ToInt32(_enterpriseId);
            return context.Enterprises.Find(id);
        }

        public ModelEnterprise GetByPersonalizedCode(string personalizedCode)
        {
            try
            {                               
                 return context.Enterprises.Where(e => e.PersonalizedCode == personalizedCode).FirstOrDefault();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public int ReturnId(string personalizedCode)
        {
            try
            {
                int id = Convert.ToInt32(personalizedCode);
                //int idEnterprise = 0;
                //var enterprises = context.Enterprises.Where(e => e.PersonalizedCode == personalizedCode);
                //foreach (var enterprise in enterprises)
                //{
                //    idEnterprise = enterprise.EnterpriseId;
                //}
                //return idEnterprise;

                var ent = context.Enterprises.Where(e => e.EnterpriseCode == id).FirstOrDefault();

                return ent.EnterpriseId;

            }catch(Exception err)
            {
                throw err;
            }
        }
    }
}
