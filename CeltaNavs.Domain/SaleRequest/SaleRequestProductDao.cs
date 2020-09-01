using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain
{
    public class SaleRequestProductDao : Persistent
    {

        public List<ModelSaleRequestProduct> NewGetAll(string _enterpriseId, bool _isConsiderDelivery)
        {
            int id = Convert.ToInt32(_enterpriseId);
            try
            {
                List<ModelSaleRequestProduct> listSaleRequestProducts = new List<ModelSaleRequestProduct>();

                var itens = (from saleRequestProd in context.SaleRequestProducts
                             join prods in context.Products
                             on saleRequestProd.ProductInternalCodeOnErp equals prods.InternalCodeOnERP
                             join saleReq in context.SaleRequests
                             on saleRequestProd.SaleRequestId equals saleReq.SaleRequestId
                             where prods.EnterpriseId == id && saleRequestProd.IsDelivered == _isConsiderDelivery && saleRequestProd.IsCancelled == false
                             select new
                             {
                                 saleRequestProd,
                                 prods,
                                 saleReq

                             }).ToList();

                foreach (var saleReqprod in itens)
                {
                    ModelSaleRequestProduct srp = new ModelSaleRequestProduct();
                    ModelSaleRequest sale = new ModelSaleRequest();
                    sale.PersonalizedCode = saleReqprod.saleReq.PersonalizedCode;

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
                    srp.SaleRequest = sale;
                    listSaleRequestProducts.Add(srp);
                }

                return listSaleRequestProducts;
            }
            catch (Exception err)
            {
                throw err;
            }
        }


        public List<ModelSaleRequestProduct> GetForDelivery(string _enterpriseId)
        {
            int id = Convert.ToInt32(_enterpriseId);
            try
            {
                List<ModelSaleRequestProduct> listSaleRequestProducts = new List<ModelSaleRequestProduct>();

                var itens = (from saleRequestProd in context.SaleRequestProducts
                             join prods in context.Products
                             on saleRequestProd.ProductInternalCodeOnErp equals prods.InternalCodeOnERP
                             join saleReq in context.SaleRequests
                             on saleRequestProd.SaleRequestId equals saleReq.SaleRequestId
                             where prods.EnterpriseId == id && saleRequestProd.IsDelivered == false && saleRequestProd.IsCancelled == false
                             select new
                             {
                                 saleRequestProd,
                                 prods,
                                 saleReq

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
                    srp.SaleRequest.PersonalizedCode = saleReqprod.saleReq.PersonalizedCode;
                    listSaleRequestProducts.Add(srp);
                }

                return listSaleRequestProducts;
            }
            catch (Exception err)
            {
                throw err;
            }
        }


        public List<ModelSaleRequestProduct> GetDelivered(string _enterpriseId)
        {
            int id = Convert.ToInt32(_enterpriseId);
            try
            {
                List<ModelSaleRequestProduct> listSaleRequestProducts = new List<ModelSaleRequestProduct>();

                var itens = (from saleRequestProd in context.SaleRequestProducts
                             join prods in context.Products
                             on saleRequestProd.ProductInternalCodeOnErp equals prods.InternalCodeOnERP
                             join saleReq in context.SaleRequests
                             on saleRequestProd.SaleRequestId equals saleReq.SaleRequestId
                             where prods.EnterpriseId == id && saleRequestProd.IsDelivered == true && saleRequestProd.IsCancelled == false
                             select new
                             {
                                 saleRequestProd,
                                 prods,
                                 saleReq

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
                    srp.SaleRequest.PersonalizedCode = saleReqprod.saleReq.PersonalizedCode;
                    listSaleRequestProducts.Add(srp);
                }

                return listSaleRequestProducts;
            }
            catch (Exception err)
            {
                throw err;
            }
        }


        public List<ModelSaleRequestProduct> GetAll(string _enterpriseId)
        {
            try
            {
                int id = Convert.ToInt32(_enterpriseId);

                var result = context.SaleRequestProducts
                            .Where(s => s.IsDelivered == false && s.IsCancelled == false && s.SaleRequest.EnterpriseId == id)
                            .Include(sale => sale.SaleRequest)
                            .Include(p => p.Product)
                            .ToList();

                return result;
                            
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        public List<ModelSaleRequestProduct> GetSaleRequestProductsAll(string _enterpriseId)
        {
            List<ModelSaleRequestProduct> listSaleRequestProducts = new List<ModelSaleRequestProduct>();
            try
            {
                int _idEnterprise = Convert.ToInt32(_enterpriseId);
                var itens = from sReqProd in context.SaleRequestProducts
                            join product in context.Products
                            on sReqProd.ProductInternalCodeOnErp equals product.InternalCodeOnERP
                            where sReqProd.IsCancelled == false && product.EnterpriseId == _idEnterprise && sReqProd.IsCancelled == false
                            select new
                            {
                                sReqProd.SaleRequestProductId,
                                sReqProd.SaleRequestId,
                                sReqProd.ProductInternalCodeOnErp,                                
                                sReqProd.Value,
                                sReqProd.Quantity,
                                sReqProd.Comments,
                                sReqProd.UserId,                                
                                sReqProd.IsCancelled,
                                sReqProd.IsDelivered,
                                sReqProd.Product,
                                sReqProd.TotalLiquid
                            };

                foreach (var saleReqprod in itens)
                {
                    ModelSaleRequestProduct srp = new ModelSaleRequestProduct();
                    srp.SaleRequestProductId = saleReqprod.SaleRequestProductId;
                    srp.SaleRequestId = saleReqprod.SaleRequestId;
                    srp.ProductInternalCodeOnErp = saleReqprod.ProductInternalCodeOnErp;                    
                    srp.Value = saleReqprod.Value;
                    srp.Quantity = saleReqprod.Quantity;
                    srp.Comments = saleReqprod.Comments;
                    srp.UserId = saleReqprod.UserId;                    
                    srp.IsCancelled = saleReqprod.IsCancelled;
                    srp.Product = saleReqprod.Product;
                    srp.TotalLiquid = saleReqprod.TotalLiquid;
                    srp.IsDelivered = saleReqprod.IsDelivered;
                    listSaleRequestProducts.Add(srp);

                }

                return listSaleRequestProducts;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public List<ModelSaleRequestProduct> FixProductId(int saleRequestId, int enterpriseId)
        {
            try
            {
                List<ModelSaleRequestProduct> listSaleRequestProducts = new List<ModelSaleRequestProduct>();

                var itens = (from saleRequestProd in context.SaleRequestProducts
                             join prods in context.Products
                             on saleRequestProd.ProductInternalCodeOnErp equals prods.InternalCodeOnERP
                             where saleRequestProd.SaleRequestId == saleRequestId && prods.EnterpriseId == enterpriseId
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

        public void Add(ModelSaleRequestProduct saleReqProd)
        {
            context.SaleRequestProducts.Add(saleReqProd);
            context.SaveChanges();
        }

        public void CancelIten(string movementId)
        {
            int id = Convert.ToInt32(movementId);
            ModelSaleRequestProduct saleReqProd = context.SaleRequestProducts.Find(id);
            saleReqProd.IsCancelled = true;
            context.SaveChanges();
        }

        public void Update(ModelSaleRequestProduct saleRequestProductId)
        {            
            ModelSaleRequestProduct saleReqProd = context.SaleRequestProducts.Find(saleRequestProductId.SaleRequestProductId);
            //saleReqProd.IsDelivered = true;
            context.SaveChanges();
        }

        public void MarkToDelivery(string _saleRequestProductId)
        {
            int _id = Convert.ToInt32(_saleRequestProductId);
            try
            {
                ModelSaleRequestProduct saleReqProd = context.SaleRequestProducts.Find(_id);

                saleReqProd.IsDelivered = true;

                //ModelSaleRequests saleReq = context.SaleRequests.Find(138);
                //saleReqProd.SaleRequest = saleReq;
                ////response.saleRequestProd.IsDelivered = true;      
                //saleReq.Peoples = 2;
                //saleReqProd.IsDelivered = true;
                context.SaveChanges();
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        public void RemoveSaleRequestProducts(string personalizedCode)
        {
            try
            {
                ModelSaleRequest saleReq = new ModelSaleRequest();
                var saleRequest = context.SaleRequests.Where(s => s.PersonalizedCode == personalizedCode);
                foreach (var item in saleRequest)
                {
                    saleReq = item;
                }
                var saleReqProducts = context.SaleRequestProducts.Where(srP => srP.SaleRequestId == saleReq.SaleRequestId);
                context.SaleRequestProducts.RemoveRange(saleReqProducts);
                context.SaveChanges();
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
