using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain
{
    public class SaleRequestProductTempDao : Persistent
    {
        public void Add(ModelSaleRequestProductTemp saleReqProd)
        {
            context.SaleRequestProductsTemp.Add(saleReqProd);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            ModelSaleRequestProductTemp p = new ModelSaleRequestProductTemp();
            var resp = context.SaleRequestProductsTemp.Find(id);
            p = resp;
            context.SaleRequestProductsTemp.Remove(p);
            context.SaveChanges();
        }
    }
}
