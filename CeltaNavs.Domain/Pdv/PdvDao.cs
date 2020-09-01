using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain
{
    public class PdvDao : Persistent
    {
        public List<ModelPdv> GetAll(string _enterpriseId)
        {
            int id = Convert.ToInt32(_enterpriseId);
            return context.Pdvs.Where(p => p.IsUsing == false && p.EnterpriseId == id).ToList();
        }

        public void MarkInUse(string pdvId, string enterpriseId, string serialPos)
        {
            int id = Convert.ToInt32(pdvId);
            int idEnt = Convert.ToInt32(enterpriseId);
            ModelPdv pdvValue = context.Pdvs.Find(id);
            pdvValue.EnterpriseId = idEnt;
            pdvValue.IsUsing = true;
            pdvValue.MacAddress = serialPos;
            pdvValue.VersionNumber = "NAVS";
            context.SaveChanges();

        }

        public int ReturnId(int pdvNumber, int enterpriseId)
        {            
            int idPdv = 0;
            var pdvs = context.Pdvs.Where(p => p.PdvNumber == pdvNumber && p.EnterpriseId == enterpriseId);
            foreach (var pdv in pdvs)
            {
                idPdv = pdv.PdvId;
            }
            return idPdv;
        }
    }
}
