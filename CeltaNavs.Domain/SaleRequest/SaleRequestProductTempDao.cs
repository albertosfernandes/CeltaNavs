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
    }
}
