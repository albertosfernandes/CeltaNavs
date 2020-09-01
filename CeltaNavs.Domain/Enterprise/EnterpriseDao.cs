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

        public int ReturnId(string personalizedCode)
        {
            try
            {
                int idEnterprise = 0;
                var enterprises = context.Enterprises.Where(e => e.PersonalizedCode == personalizedCode);
                foreach (var enterprise in enterprises)
                {
                    idEnterprise = enterprise.EnterpriseId;
                }
                return idEnterprise;

            }catch(Exception err)
            {
                throw err;
            }
        }
    }
}
