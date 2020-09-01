using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CeltaNavs.Repository;

namespace CeltaNavs.Domain
{
    public class BkpSaleRequestDao : Persistent
    {
        ModelSaleRequest saleReq = new ModelSaleRequest();

        //Não retorna a lista Products de SaleRequestProduct
        //public ModelSaleRequest Get(ModelNavsSetting settings, string table)
        //{
        //    try
        //    {
        //        var query = from _saleReq in context.SaleRequests
        //                    join _ent in context.Enterprises
        //                    on _saleReq.EnterpriseId equals _ent.EnterpriseId
        //                    where _saleReq.EnterpriseId == settings.EnterpriseId 
        //                    && _saleReq.PersonalizedCode == table
        //                    && _saleReq.IsUsing == false && _saleReq.FlagStatus == "ABERTO"
        //                    select new
        //                    {
        //                        _ent,
        //                        _saleReq,                                                               
        //                    };
        //        //var query = context.SaleRequests.Where(sr => sr.PersonalizedCode == table
        //        //&& sr.EnterpriseId == settings.EnterpriseId && sr.IsUsing == false && sr.Status == "ABERTO");        

        //        foreach (var saleRequest in query)
        //        {
        //            saleReq.SaleRequestId = saleRequest._saleReq.SaleRequestId;
        //            saleReq.PersonalizedCode = saleRequest._saleReq.PersonalizedCode;
        //            saleReq.DateOfCreation = saleRequest._saleReq.DateOfCreation;
        //            saleReq.EnterpriseId = saleRequest._ent.EnterpriseId;
        //            saleReq.IsCancelled = saleRequest._saleReq.IsCancelled;
        //            saleReq.Peoples = saleRequest._saleReq.Peoples;
        //            saleReq.FlagStatus = saleRequest._saleReq.FlagStatus;
        //            saleReq.TotalLiquid = saleRequest._saleReq.TotalLiquid;
        //           // saleReq.EnterpriseCode = saleRequest._ent.EnterpriseCode;
        //           // saleReq.EnterprisePersonalizedCode = saleRequest._ent.PersonalizedCode;
        //            saleReq.FlagOrigin = saleRequest._saleReq.FlagOrigin;
        //        }
        //        return saleReq;
        //    }
        //    catch (Exception err)
        //    {
        //        throw err;
        //    }
        //}


        //
        //public ModelSaleRequest GetStatus(ModelNavsSetting settings, string table)
        //{
        //    try
        //    {
        //        var query = from _saleReq in context.SaleRequests
        //                    join _ent in context.Enterprises
        //                    on _saleReq.EnterpriseId equals _ent.EnterpriseId
        //                    where _saleReq.EnterpriseId == settings.EnterpriseId
        //                    && _saleReq.PersonalizedCode == table                            
        //                    select new
        //                    {
        //                        _ent,
        //                        _saleReq,
        //                    };
        //        //var query = context.SaleRequests.Where(sr => sr.PersonalizedCode == table
        //        //&& sr.EnterpriseId == settings.EnterpriseId && sr.IsUsing == false && sr.Status == "ABERTO");        

        //        foreach (var saleRequest in query)
        //        {
        //            saleReq.SaleRequestId = saleRequest._saleReq.SaleRequestId;
        //            saleReq.PersonalizedCode = saleRequest._saleReq.PersonalizedCode;
        //            saleReq.DateOfCreation = saleRequest._saleReq.DateOfCreation;
        //            saleReq.EnterpriseId = saleRequest._ent.EnterpriseId;
        //            saleReq.IsCancelled = saleRequest._saleReq.IsCancelled;
        //            saleReq.Peoples = saleRequest._saleReq.Peoples;
        //            saleReq.FlagStatus = saleRequest._saleReq.FlagStatus;
        //            saleReq.TotalLiquid = saleRequest._saleReq.TotalLiquid;
        //            //IsUsing !!!!!!!!!!!!!!!
        //            //!!!!!
        //            // saleReq.EnterpriseCode = saleRequest._ent.EnterpriseCode;
        //            // saleReq.EnterprisePersonalizedCode = saleRequest._ent.PersonalizedCode;
        //            saleReq.FlagOrigin = saleRequest._saleReq.FlagOrigin;
        //        }
        //        return saleReq;
        //    }
        //    catch (Exception err)
        //    {
        //        throw err;
        //    }
        //}

        //Utilizado na controller de Tables para ver as mesas/Pedidos que estão em aberto
        //public List<ModelSaleRequest> GetTablesOpen(ModelNavsSetting settings)
        //{
        //    List<ModelSaleRequest> listSaleRequest = new List<ModelSaleRequest>();
        //    try
        //    {
        //        var tables = (from tablesSaleRequests in context.SaleRequests
        //                      where tablesSaleRequests.EnterpriseId == settings.EnterpriseId
        //                      && tablesSaleRequests.IsUsing == false
        //                      && tablesSaleRequests.FlagStatus == "ABERTO"
        //                      group tablesSaleRequests by new
        //                      {
        //                          table = tablesSaleRequests.PersonalizedCode
        //                      } into sr
        //                      select new
        //                      {
        //                          sr.Key.table
        //                      }).OrderBy(s => s.table).ToList();

        //        foreach (var tableItem in tables)
        //        {
        //            ModelSaleRequest sale = new ModelSaleRequest();
        //            sale.PersonalizedCode = tableItem.table;
        //            listSaleRequest.Add(sale);
        //        }

        //        return listSaleRequest;
        //    }
        //    catch (Exception err)
        //    {
        //        throw err;
        //    }
        //}

        //public List<ModelSaleRequest> GetSaleRequest(ModelNavsSetting settings)
        //{
        //    try
        //    {
        //        List<ModelSaleRequest> listOfSaleRequest = new List<ModelSaleRequest>();
        //        var result = from _saleReq in context.SaleRequests
        //                     join _ent in context.Enterprises
        //                     on _saleReq.EnterpriseId equals _ent.EnterpriseId
        //                     where _saleReq.EnterpriseId == settings.EnterpriseId
        //                     select new
        //                     {
        //                         _ent,
        //                         _saleReq
        //                     };

        //        foreach (var item in result)
        //        {
        //            ModelSaleRequest _saleRequest = new ModelSaleRequest();
        //            //_saleRequest.EnterpriseCode = item._ent.EnterpriseId;
        //            //_saleRequest.EnterprisePersonalizedCode = item._ent.EnterpriseCode.ToString();
        //            _saleRequest.PersonalizedCode = (item._saleReq.PersonalizedCode.ToString()).Insert(0, "00000000");
        //            _saleRequest.DateOfCreation = item._saleReq.DateOfCreation;
        //            _saleRequest.FlagOrigin = CeltaNavs.Repository.SaleRequestOrigin.Concentrator;
        //            listOfSaleRequest.Add(_saleRequest);
        //        }
        //        return listOfSaleRequest;
        //    }
        //    catch (Exception err)
        //    {
        //        throw err;
        //    }
        //}

        //public List<ModelSaleRequest> GetSaleRequestId(ModelNavsSetting settings, string _table)
        //{
        //    try
        //    {
        //        List<ModelSaleRequest> listOfSaleRequest = new List<ModelSaleRequest>();
        //        var result = (from _saleReq in context.SaleRequests
        //                      join _ent in context.Enterprises
        //                      on _saleReq.EnterpriseId equals _ent.EnterpriseId
        //                      join _saleReqproduct in context.SaleRequestProducts
        //                      on _saleReq.SaleRequestId equals _saleReqproduct.SaleRequestId
        //                      where _saleReq.EnterpriseId == settings.EnterpriseId && _saleReq.PersonalizedCode == _table
        //                      select new
        //                      {
        //                          _ent,
        //                          _saleReq,
        //                          _saleReqproduct,
        //                          _saleReqproduct.Product
        //                      }).FirstOrDefault();

        //        List<ModelSaleRequestProduct> listProd = new List<ModelSaleRequestProduct>();
        //        ModelSaleRequest _saleRequest = new ModelSaleRequest();

        //        //_saleRequest.EnterpriseCode = result._ent.EnterpriseId;
        //        //_saleRequest.EnterprisePersonalizedCode = result._ent.EnterpriseCode.ToString();
        //        _saleRequest.DateOfCreation = result._saleReq.DateOfCreation;
        //        _saleRequest.FlagOrigin = CeltaNavs.Repository.SaleRequestOrigin.Concentrator;

        //        var resultProducts = (from _saleReq in context.SaleRequests
        //                              join _ent in context.Enterprises
        //                              on _saleReq.EnterpriseId equals _ent.EnterpriseId
        //                              join _saleReqproduct in context.SaleRequestProducts
        //                              on _saleReq.SaleRequestId equals _saleReqproduct.SaleRequestId
        //                              where _saleReq.EnterpriseId == settings.EnterpriseId && _saleReq.PersonalizedCode == _table
        //                              select new
        //                              {
        //                                  _ent,
        //                                  _saleReq,
        //                                  _saleReqproduct,
        //                                  _saleReqproduct.Product
        //                              });

        //        foreach (var item in resultProducts)
        //        {
        //            // SaleRequestProducts _saleRequestProd = new SaleRequestProducts();

        //            ////_saleRequest.EnterpriseCode = item._ent.EnterpriseId;
        //            ////_saleRequest.EnterprisePersonalizedCode = item._ent.EnterpriseCode.ToString();                                       
        //            ////_saleRequest.PersonalizedCode = (item._saleReq.PersonalizedCode.ToString()).Insert(0, "00000000");
        //            ////_saleRequest.DateOfCreation = item._saleReq.Date;
        //            ////_saleRequest.Origin = SaleRequestOrigin.Concentrator;

        //            //_saleRequestProd.AveragedDeliveryTax = 0;
        //            //_saleRequestProd.AveragedDiscount = 0;
        //            //_saleRequestProd.AveragedIncrement = 0;
        //            ////_saleRequestProd.BlockadeDiscountType
        //            //_saleRequestProd.SaleRetailOffer = item._saleReqproduct.Products.SaleRetailPrice;
        //            //_saleRequestProd.WholeSaleOffer = 0;
        //            //_saleRequestProd.WholeSalePrice = 0;
        //            ////_saleRequestProd._id = "0";
        //            //_saleRequestProd.InternalCodeOnERP = item._saleReqproduct.Products.InternalCodeOnERP;
        //            //_saleRequestProd.ProductName = item._saleReqproduct.Products.Name;
        //            //_saleRequestProd.ProductPlu = item._saleReqproduct.Products.PriceLookupCode;
        //            //_saleRequestProd.Quantity = item._saleReqproduct.Quantity;
        //            //_saleRequestProd.SaleRequestProductInternalCodeOnERP = item._saleReqproduct.ProductInternalCodeOnErp;
        //            //_saleRequestProd.SaleRetailPrice = item._saleReqproduct.Products.SaleRetailPrice;
        //            //_saleRequestProd.Value = (item._saleReqproduct.Products.SaleRetailPrice * item._saleReqproduct.Quantity);
        //            //listProd.Add(_saleRequestProd);

        //            //listOfSaleRequest.Add(_saleRequest);
        //        }
        //        //_saleRequest.Products = listProd;
        //        //listOfSaleRequest.Add(_saleRequest);
        //        return listOfSaleRequest;
        //    }
        //    catch (Exception err)
        //    {
        //        throw err;
        //    }
        //}

        //public List<ModelSaleRequestProduct> GetItensSaleRequest(ModelNavsSetting settings, string table)
        //{
        //    List<ModelSaleRequestProduct> listSaleRequestProducts = new List<ModelSaleRequestProduct>();
        //    try
        //    {

        //        var itens = from sReqProd in context.SaleRequestProducts
        //                    join product in context.Products
        //                    on sReqProd.ProductInternalCodeOnErp equals product.InternalCodeOnERP
        //                    join saleRequest in context.SaleRequests
        //                    on sReqProd.SaleRequestId equals saleRequest.SaleRequestId
        //                    where saleRequest.PersonalizedCode ==  table && sReqProd.IsCancelled == false && product.EnterpriseId == settings.EnterpriseId
        //                    select new
        //                    {
        //                        sReqProd.SaleRequestProductId,
        //                        sReqProd.SaleRequestId,
        //                        sReqProd.ProductInternalCodeOnErp,                                
        //                        sReqProd.Value,
        //                        sReqProd.Quantity,
        //                        sReqProd.Comments,
        //                        sReqProd.UserId,                                
        //                        sReqProd.IsCancelled,
        //                        sReqProd.Product,
        //                        sReqProd.TotalLiquid
        //                    };

        //        foreach (var saleReqprod in itens)
        //        {
        //            ModelSaleRequestProduct srp = new ModelSaleRequestProduct();
        //            srp.SaleRequestProductId = saleReqprod.SaleRequestProductId;
        //            srp.SaleRequestId = saleReqprod.SaleRequestId;
        //            srp.ProductInternalCodeOnErp = saleReqprod.ProductInternalCodeOnErp;                    
        //            srp.Value = saleReqprod.Value;
        //            srp.Quantity = saleReqprod.Quantity;
        //            srp.Comments = saleReqprod.Comments;
        //            srp.UserId = saleReqprod.UserId;                    
        //            srp.IsCancelled = saleReqprod.IsCancelled;
        //            srp.Product = saleReqprod.Product;
        //            srp.TotalLiquid = saleReqprod.TotalLiquid;
        //            listSaleRequestProducts.Add(srp);

        //        }

        //        return listSaleRequestProducts;
        //    }
        //    catch (Exception err)
        //    {
        //        throw err;
        //    }
        //}

        //public static decimal TotalLiquid(List<ModelSaleRequestProduct> saleRequestProducts)
        //{
        //    decimal value = 0;

        //    foreach (var item in saleRequestProducts)
        //    {
        //        value += item.TotalLiquid;
        //    }

        //    return value;
        //}

        //public void Add(ModelSaleRequest saleReq)
        //{
        //    context.SaleRequests.Add(saleReq);
        //    context.SaveChanges();
        //}

        //public void Update(ModelSaleRequest _saleReq)
        //{
        //    var saleRequest = context.SaleRequests.Find(saleReq.SaleRequestId);
        //    saleRequest.TotalLiquid = _saleReq.TotalLiquid;
        //    saleRequest.IsUsing = _saleReq.IsUsing;
        //    context.SaveChanges();
        //}

        //public bool MarkSaleRequest(CeltaNavs.Repository.ModelNavsSettings _navsSettings, ModelSaleRequests saleRequest)
        //{
        //    try
        //    {
        //        saleRequest.IsUsing = saleRequest.IsUsing;
        //        Update(saleRequest);
        //        return true;
        //    }
        //    catch (Exception err)
        //    {
        //        return false;
        //        throw err;
        //    }
        //}

    }
}
