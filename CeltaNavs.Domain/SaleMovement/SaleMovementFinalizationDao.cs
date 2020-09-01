using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain
{
    public class SaleMovementFinalizationDao : Persistent
    {

        public ModelSaleMovementFinalization Get(string personalizedCode, ModelNavsSetting settings)
        {
            try
            {
                return context.NavsFinalizations.First(sr => sr.PersonalizedCode == personalizedCode && sr.EnterpriseId == settings.EnterpriseId);
            }
            catch(Exception err)
            {
                throw err;
            }
        }       

        public List<ModelSaleMovementFinalization> GetAll(string personalizedCode, ModelNavsSetting settings)
        {
            try
            {
                return context.NavsFinalizations.Where(sr => sr.PersonalizedCode == personalizedCode && sr.EnterpriseId == settings.EnterpriseId).ToList();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public decimal PaidAmountValue(string card, ModelNavsSetting settings)
        {            
            try
            {
                decimal value = 0;

                var response = context.NavsFinalizations.Where(sf => sf.PersonalizedCode == card && sf.EnterpriseId == settings.EnterpriseId && sf.PdvId == settings.PdvId).ToList();
                foreach (var item in response)
                {
                    value += item.Value;
                }
                return value;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

     
        public void Add(ModelSaleMovementFinalization saleRequestFinalizations)
        {
            context.NavsFinalizations.Add(saleRequestFinalizations);
            context.SaveChanges();
        }

        public void Update(ModelSaleMovementFinalization navsFinalization)
        {
            ModelSaleMovementFinalization _navsFinalization = context.NavsFinalizations.Find(navsFinalization.NavsFinalizationId);
            _navsFinalization = navsFinalization;
            context.SaveChanges();
        }

        public void RemovePayments(string card)
        {
            try
            {
                var salesorderFinalizations = context.NavsFinalizations.Where(sf => sf.PersonalizedCode == card);
                context.NavsFinalizations.RemoveRange(salesorderFinalizations);
                context.SaveChanges();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public void RemovePaymentsMoney(ModelSaleMovementFinalization salesRequestFin)
       {
           try
           {                
               var salesorderFinalizations = context.NavsFinalizations
                   .Where(sf => sf.PersonalizedCode == salesRequestFin.PersonalizedCode && sf.EnterpriseId == salesRequestFin.EnterpriseId &&
                                sf.PdvId == salesRequestFin.PdvId && sf.FinalizationId == 1);
               
               context.NavsFinalizations.RemoveRange(salesorderFinalizations);
               context.SaveChanges();
           }
           catch (Exception err)
           {
               throw err;
           }

       }

        public void RemovePaymentsTef(string card, int saleOrderFinId)
        {
            try
            {
                int cardId = Convert.ToInt32(card);
                var salesorderFinalizations = context.NavsFinalizations.Where(sf => sf.PersonalizedCode == card && sf.NavsFinalizationId == saleOrderFinId);
                context.NavsFinalizations.RemoveRange(salesorderFinalizations);
                context.SaveChanges();
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
