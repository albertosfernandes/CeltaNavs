using CeltaNavs.Domain;
using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CeltaNavsApi.Helpers
{
    public class ValuesHelpers
    {
        public static ModelProduct GetPriceOrOffer(ModelProduct p, ModelNavsSetting navsSettings)
        {
            ProductDao pDao = new ProductDao();
            ModelProduct productTest = pDao.FindByInternalCode(p.InternalCodeOnERP.ToString(), navsSettings);
            if (productTest.OfferRetailPrice == 0)
            {
                return productTest;
            }
            else
            {
                productTest.SaleRetailPrice = productTest.OfferRetailPrice;
                return productTest;
            }
        }
    }
}