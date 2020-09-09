using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain
{
    public class SaleRequestDao : Persistent
    {

        public ModelSaleRequest Get(string _enterpriseId, string _personalizedCode, bool _considerUsing)
        {
            try
            {               
                int enterpriseId = Convert.ToInt32(_enterpriseId);
                List<ModelSaleRequestProduct> newlistSaleRequestProducts = new List<ModelSaleRequestProduct>();
                
                var saleRequest = context.SaleRequests
                   .Where(s => s.PersonalizedCode == _personalizedCode && s.EnterpriseId == enterpriseId && (!_considerUsing || !s.IsUsing))                   
                   .FirstOrDefault();
                
                if(saleRequest != null)
                newlistSaleRequestProducts = Helper.SaleRequestHelpers.FixProductId(saleRequest.SaleRequestId, enterpriseId);                

                return saleRequest;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public List<ModelSaleRequest> GetAll(string _enterpriseId)
        {
            try
            {
                int enterpriseId = Convert.ToInt32(_enterpriseId);

                var saleRequest = context.SaleRequests
                   .Where(s => s.EnterpriseId == enterpriseId && s.IsUsing == false)
                   .Include(prod => prod.Products.Select(s => s.Product))
                   .OrderBy(s => s.PersonalizedCode)
                   .ToList();


                return saleRequest;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public List<ModelSaleRequest> GetProduction(int _enterpriseId, ProductionStatus productioStatusCode)
        {
            try
            {                

                List<ModelSaleRequest> listOfSaleRequest = new List<ModelSaleRequest>();
                using(var context = new NavsContext())
                {
                    var result = (from saleRequest in context.SaleRequests
                                  join saleRequestProduct in context.SaleRequestProducts
                                  on saleRequest.SaleRequestId equals saleRequestProduct.SaleRequestId
                                  where saleRequestProduct.ProductionStatus == ProductionStatus.New && saleRequest.EnterpriseId == _enterpriseId
                                  group saleRequest by saleRequest.PersonalizedCode into newGroup
                                  select new
                                  {
                                      newGroup

                                  }).ToList();

                    //listOfSaleRequest.Add(saleRequestProduction.newGroup.ToList<ModelSaleRequest>());

                    foreach (var saleRequestProduction in result)
                    {
                        //ModelSaleRequest sale = new ModelSaleRequest();
                        listOfSaleRequest = saleRequestProduction.newGroup.ToList<ModelSaleRequest>();

                        

                        //foreach (var item in saleRequestProduction.newGroup)
                        //{
                        //    ModelSaleRequest sale = new ModelSaleRequest();
                        //    sale = item;                            
                        //    listOfSaleRequest.Add(sale);
                        //}
                    }
                }

               
                

                return listOfSaleRequest;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public void Add(ModelSaleRequest saleReq)
        {
            context.SaleRequests.Add(saleReq);
            if (saleReq.Products != null)
            {
                foreach (var item in saleReq.Products)
                {
                    context.SaleRequestProducts.Add(item);
                }
            }
            context.SaveChanges();
        }

        public void AddNew(ModelSaleRequest saleReq)
        {
            context.SaleRequests.Add(saleReq);           
            context.SaveChanges();
        }

        //Criei este método para corrigir o atributo (List<Products> do  tipo ModelSaleRequestProduct) de SaleRequest
        //pois o ModelSaleRequestProduct não tem uma FK com Products utilizo InternalCodeOnErp que podem existir mesmo codigo
        //para ambas empresas
        //private List<ModelSaleRequestProduct> FixProductId(int saleRequestId, int enterpriseId)
        //{
        //    try
        //    {
        //        List<ModelSaleRequestProduct> listSaleRequestProducts = new List<ModelSaleRequestProduct>();

        //        var itens = (from saleRequestProd in context.SaleRequestProducts
        //                     join prods in context.Products
        //                     on saleRequestProd.ProductInternalCodeOnErp equals prods.InternalCodeOnERP
        //                     where saleRequestProd.SaleRequestId == saleRequestId && prods.EnterpriseId == enterpriseId && saleRequestProd.IsCancelled == false
        //                     select new
        //                     {
        //                         saleRequestProd,
        //                         prods

        //                     }).ToList();

        //        foreach (var saleReqprod in itens)
        //        {
        //            ModelSaleRequestProduct srp = new ModelSaleRequestProduct();
        //            srp.SaleRequestProductId = saleReqprod.saleRequestProd.SaleRequestProductId;
        //            srp.SaleRequestId = saleReqprod.saleRequestProd.SaleRequestId;
        //            srp.ProductInternalCodeOnErp = saleReqprod.saleRequestProd.ProductInternalCodeOnErp;
        //            srp.Value = saleReqprod.saleRequestProd.Value;
        //            srp.Quantity = saleReqprod.saleRequestProd.Quantity;
        //            srp.Comments = saleReqprod.saleRequestProd.Comments;
        //            srp.UserId = saleReqprod.saleRequestProd.UserId;
        //            srp.IsCancelled = saleReqprod.saleRequestProd.IsCancelled;
        //            srp.Product = saleReqprod.prods;
        //            srp.TotalLiquid = saleReqprod.saleRequestProd.TotalLiquid;
        //            listSaleRequestProducts.Add(srp);
        //        }

        //        return listSaleRequestProducts;
        //    }
        //    catch (Exception err)
        //    {
        //        throw err;
        //    }
        //}        

        //public void UpdateTeste(int saleRequestId, decimal valueTotal)
        //{
        //    try
        //    {
        //        ModelSaleRequest saleReq = context.SaleRequests.Find(saleRequestId);
        //        saleReq.TotalLiquid = valueTotal;
        //        context.SaveChanges();
        //    }
        //    catch(Exception err)
        //    {
        //        throw err;
        //    }
        //}

        public void Update(ModelSaleRequest _saleReq)
        {
            try
            {
                ModelSaleRequest saleRequest = new ModelSaleRequest();
                saleRequest = context.SaleRequests.Find(_saleReq.SaleRequestId);

                saleRequest.TotalLiquid = _saleReq.TotalLiquid;
                saleRequest.IsUsing = _saleReq.IsUsing;
                saleRequest.Peoples = _saleReq.Peoples;
                
                context.SaveChanges();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public void Delete(int _saleRequestId)
        {
            try
            {
                var saleRequest = context.SaleRequests.Find(_saleRequestId);
                context.SaleRequests.Remove(saleRequest);
                context.SaveChanges();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public void RemoveSaleRequest(string personalizedCode)
        {
            try
            {
                var saleRequest = context.SaleRequests.Where(srP => srP.PersonalizedCode == personalizedCode);
                context.SaleRequests.RemoveRange(saleRequest);
                context.SaveChanges();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public bool MarkInUse(int saleRequestId, bool valueMark)
        {
            try
            {
                var saleRequest = context.SaleRequests.Find(saleRequestId);
                saleRequest.IsUsing = valueMark;
                context.SaveChanges();
                return true;
            }
            catch(Exception err)
            {
                return false;
                throw err;
            }
        }

        public List<ModelSaleRequestProduct> GetSaleRequestProducts(string _enterpriseId, string _saleRequestId, bool isConsider)
        {
            try
            {
                List<ModelSaleRequestProduct> listSaleRequestProducts = new List<ModelSaleRequestProduct>();
                int ent = Convert.ToInt32(_enterpriseId);
                int sId = Convert.ToInt32(_saleRequestId);


                var itens = (from saleRequestProd in context.SaleRequestProducts
                             join prods in context.Products
                             on saleRequestProd.ProductInternalCodeOnErp equals prods.InternalCodeOnERP                             
                             where saleRequestProd.SaleRequestId == sId && prods.EnterpriseId == ent && saleRequestProd.IsCancelled == false
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

        public ModelSaleRequest GetId(string _enterpriseId, string _personalizedCode)
        {
            int ent = Convert.ToInt32(_enterpriseId);
            return context.SaleRequests
                  .Where(s => s.PersonalizedCode == _personalizedCode && s.EnterpriseId == ent)                  
                  .FirstOrDefault();
        }

        public int GetById(string _enterpriseId, string _personalizedCode, bool isConsider)
        {
            int ent = Convert.ToInt32(_enterpriseId);
            var saleReq= context.SaleRequests
                        .Where(s => s.PersonalizedCode == _personalizedCode && s.EnterpriseId == ent && (!isConsider || !s.IsUsing))                        
                         .FirstOrDefault();

            int idSaleRequest = saleReq.SaleRequestId;

            return idSaleRequest;
        }

    }
}
