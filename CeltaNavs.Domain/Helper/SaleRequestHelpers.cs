using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain.Helper
{
    public class SaleRequestHelpers
    {
        public SaleRequestHelpers()
        {

        }

        //Criei este método para corrigir o atributo (List<Products> do  tipo ModelSaleRequestProduct) de SaleRequest
        //pois o ModelSaleRequestProduct não tem uma FK com Products utilizo InternalCodeOnErp que podem existir mesmo codigo
        //para ambas empresas
        public static List<ModelSaleRequestProduct> FixProductId(int saleRequestId, int enterpriseId)
        {
            try
            {
                NavsContext context = new NavsContext();
                List<ModelSaleRequestProduct> listSaleRequestProducts = new List<ModelSaleRequestProduct>();

                var itens = (from saleRequestProd in context.SaleRequestProducts
                             join prods in context.Products
                             on saleRequestProd.ProductInternalCodeOnErp equals prods.InternalCodeOnERP
                             where saleRequestProd.SaleRequestId == saleRequestId && prods.EnterpriseId == enterpriseId && saleRequestProd.IsCancelled == false
                             select new
                             {
                                 saleRequestProd,
                                 prods

                             }).ToList();

                foreach (var saleReqprod in itens)
                {
                    ModelSaleRequestProduct srp = new ModelSaleRequestProduct();
                    srp.SaleRequestProductId = saleReqprod.saleRequestProd.SaleRequestProductId;
                    srp.SaleRequestId = saleReqprod.saleRequestProd.SaleRequestId;
                    srp.ProductInternalCodeOnErp = saleReqprod.saleRequestProd.ProductInternalCodeOnErp;
                    srp.Value = saleReqprod.saleRequestProd.Value;
                    srp.Quantity = saleReqprod.saleRequestProd.Quantity;
                    srp.Comments = saleReqprod.saleRequestProd.Comments;
                    srp.UserId = saleReqprod.saleRequestProd.UserId;
                    srp.IsCancelled = saleReqprod.saleRequestProd.IsCancelled;
                    srp.Product = saleReqprod.prods;
                    srp.TotalLiquid = saleReqprod.saleRequestProd.TotalLiquid;
                    listSaleRequestProducts.Add(srp);
                }

                return listSaleRequestProducts;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static List<ModelSaleRequestProductTemp> FixProductIdTemp(int saleRequestTempId, int enterpriseId)
        {
            try
            {
                NavsContext context = new NavsContext();
                List<ModelSaleRequestProductTemp> listSaleRequestProductsTemp = new List<ModelSaleRequestProductTemp>();

                var itens = (from saleRequestProdTemp in context.SaleRequestProductsTemp
                             join prods in context.Products
                             on saleRequestProdTemp.ProductInternalCodeOnErp equals prods.InternalCodeOnERP
                             where saleRequestProdTemp.SaleRequestProductTempId == saleRequestTempId && prods.EnterpriseId == enterpriseId
                             select new
                             {
                                 saleRequestProdTemp,
                                 prods

                             }).ToList();

                foreach (var saleReqprodTemp in itens)
                {
                    ModelSaleRequestProductTemp srpt = new ModelSaleRequestProductTemp();
                    srpt.SaleRequestProductTempId = saleReqprodTemp.saleRequestProdTemp.SaleRequestProductTempId;
                    srpt.SaleRequestTempId = saleReqprodTemp.saleRequestProdTemp.SaleRequestTempId;
                    srpt.ProductInternalCodeOnErp = saleReqprodTemp.saleRequestProdTemp.ProductInternalCodeOnErp;
                    srpt.Value = saleReqprodTemp.saleRequestProdTemp.Value;
                    srpt.Quantity = saleReqprodTemp.saleRequestProdTemp.Quantity;                    
                    srpt.Product = saleReqprodTemp.prods;
                    srpt.TotalLiquid = saleReqprodTemp.saleRequestProdTemp.TotalLiquid;
                    listSaleRequestProductsTemp.Add(srpt);
                }

                return listSaleRequestProductsTemp;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
