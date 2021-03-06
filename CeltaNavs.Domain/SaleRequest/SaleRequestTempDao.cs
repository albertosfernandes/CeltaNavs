﻿using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain
{
    public class SaleRequestTempDao : Persistent
    {
        private SaleRequestDao saleRequestsDao = new SaleRequestDao();

        private ModelSaleRequestTemp saleRequestTemp = new ModelSaleRequestTemp();

        public ModelSaleRequestTemp Get(string _enterpriseId, string _personalizedCode, bool _considerUsing)
        {
            try
            {
                int enterpriseId = Convert.ToInt32(_enterpriseId);
                List<ModelSaleRequestProductTemp> newlistSaleRequestProducts = new List<ModelSaleRequestProductTemp>();

                var saleRequestemp = context.SaleRequestsTemp
                   .Where(s => s.PersonalizedCode == _personalizedCode && s.EnterpriseId == enterpriseId)
                   .Include(srp => srp.Products.Select(p => p.Product))
                   .FirstOrDefault();

                //if (saleRequestemp != null)
                //    newlistSaleRequestProducts = Helper.SaleRequestHelpers.FixProductIdTemp(saleRequestemp.SaleRequestTempId, enterpriseId);

                return saleRequestemp;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public List<ModelSaleRequestTemp> GetAll(string _enterpriseId)
        {
            try
            {
                int enterpriseId = Convert.ToInt32(_enterpriseId);

                var saleRequest = context.SaleRequestsTemp
                   .Where(s => s.EnterpriseId == enterpriseId)
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

        public void Add(ModelSaleRequestTemp saleRequestTemp)
        {
            //ModelSaleRequestTemp mysaleRequestTemp = new ModelSaleRequestTemp();
            //mysaleRequestTemp.EnterpriseId = _enterpriseId;
            //mysaleRequestTemp.PersonalizedCode = _personalizedsalecode;
            try
            {
                context.SaleRequestsTemp.Add(saleRequestTemp);
                
                context.SaveChanges();
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        public void Update(ModelSaleRequestTemp _saleReqTemp)
        {
            try
            {
                saleRequestTemp = context.SaleRequestsTemp.Find(_saleReqTemp.SaleRequestTempId);
                saleRequestTemp.TotalLiquid = _saleReqTemp.TotalLiquid;              
                saleRequestTemp.Products = _saleReqTemp.Products;
                context.SaveChanges();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public bool Finish(ModelSaleRequestTemp _saleRequestTemp)
        {
            try
            {
                
                ModelSaleRequest saleRequest = saleRequestsDao.Get(_saleRequestTemp.EnterpriseId.ToString(), _saleRequestTemp.PersonalizedCode, true);                

                if (saleRequest == null)
                {

                    //Pedido novo     
                    ModelSaleRequest saleRequestNew = new ModelSaleRequest();
                    saleRequestNew.PersonalizedCode = _saleRequestTemp.PersonalizedCode;
                    saleRequestNew.DateOfCreation = DateTime.Now;
                    saleRequestNew.DateHourOfCreation = DateTime.Now;
                    saleRequestNew.EnterpriseId = _saleRequestTemp.EnterpriseId;
                    saleRequestNew.IsUsing = false;
                    saleRequestNew.Peoples = 1;
                    saleRequestNew.FlagStatus = "ABERTO";
                    saleRequestNew.FlagOrigin = SaleRequestOrigin.Concentrator;                    
                    //saleRequestsDao.Add(saleRequestNew);

                    foreach (var _saleReq in _saleRequestTemp.Products)
                    {
                        ModelSaleRequestProduct modeSaleReqProd = new ModelSaleRequestProduct();
                        modeSaleReqProd.Comments = null;
                        modeSaleReqProd.IsCancelled = false;
                        modeSaleReqProd.IsDelivered = false;
                        modeSaleReqProd.DateHourOfCreation = DateTime.Now;
                        modeSaleReqProd.ProductionStatus = ProductionStatus.New;
                        modeSaleReqProd.Quantity = _saleReq.Quantity;
                        modeSaleReqProd.Value = Convert.ToDecimal(_saleReq.Product.SaleRetailPraticedString);
                        modeSaleReqProd.TotalLiquid = (_saleReq.Value * _saleReq.Quantity);
                        //modeSaleReqProd.SaleRequestId = saleRequestNew.SaleRequestId;
                        modeSaleReqProd.ProductPriceLookUpCode = _saleReq.ProductPriceLookUpCode;
                        modeSaleReqProd.ProductInternalCodeOnErp = _saleReq.ProductInternalCodeOnErp;
                        //modeSaleReqProd.SaleRequest = saleRequest;
                        //modeSaleReqProd.Product = _saleReq.Product;
                        saleRequestNew.TotalLiquid += _saleReq.TotalLiquid;
                        saleRequestNew.Products.Add(modeSaleReqProd);
                    }

                    //saleRequestsDao.Update(saleRequestNew);
                    saleRequestsDao.AddNew(saleRequestNew);
                    //Delete(_saleRequestTemp);
                    return true;
                }
                
                foreach (var _saleReq in _saleRequestTemp.Products)
                {
                    ModelSaleRequestProduct modeSaleReqProd = new ModelSaleRequestProduct();
                    modeSaleReqProd.Comments = null;
                    modeSaleReqProd.IsCancelled = false;
                    modeSaleReqProd.IsDelivered = false;
                    modeSaleReqProd.DateHourOfCreation = DateTime.Now;
                    modeSaleReqProd.ProductionStatus = ProductionStatus.New;
                    modeSaleReqProd.Quantity = _saleReq.Quantity;
                    modeSaleReqProd.Value = Convert.ToDecimal(_saleReq.Product.SaleRetailPraticedString);
                    modeSaleReqProd.TotalLiquid = (_saleReq.Value * _saleReq.Quantity);
                   // modeSaleReqProd.SaleRequestId = _saleReq.SaleRequestTemp.SaleRequestTempId;
                    modeSaleReqProd.ProductPriceLookUpCode = _saleReq.ProductPriceLookUpCode;
                    modeSaleReqProd.ProductInternalCodeOnErp = _saleReq.ProductInternalCodeOnErp;
                    modeSaleReqProd.SaleRequest = saleRequest;                    
                    saleRequest.TotalLiquid += _saleReq.TotalLiquid;
                    saleRequest.Products.Add(modeSaleReqProd);
                }
                
                saleRequestsDao.Update(saleRequest);
                //Delete(_saleRequestTemp);
                return true;
            }
            catch(Exception err)
            {
                return false;
                throw err;
            }
        }

        public bool FixAndFinish(ModelSaleRequestTemp _saleRequestTemp)
        {
            try
            {

                ModelSaleRequest saleRequest = saleRequestsDao.Get(_saleRequestTemp.EnterpriseId.ToString(), _saleRequestTemp.PersonalizedCode, true);

                if (saleRequest == null)
                {

                    //Pedido novo     
                    ModelSaleRequest saleRequestNew = new ModelSaleRequest();
                    saleRequestNew.PersonalizedCode = _saleRequestTemp.PersonalizedCode;
                    saleRequestNew.DateOfCreation = DateTime.Now;
                    saleRequestNew.DateHourOfCreation = DateTime.Now;
                    saleRequestNew.EnterpriseId = _saleRequestTemp.EnterpriseId;
                    saleRequestNew.IsUsing = false;
                    saleRequestNew.Peoples = 1;
                    saleRequestNew.FlagStatus = "ABERTO";
                    saleRequestNew.FlagOrigin = SaleRequestOrigin.Concentrator;
                    //saleRequestsDao.Add(saleRequestNew);

                    foreach (var _saleReq in _saleRequestTemp.Products)
                    {
                        ModelSaleRequestProduct modeSaleReqProd = new ModelSaleRequestProduct();
                        modeSaleReqProd.Comments = null;
                        modeSaleReqProd.IsCancelled = false;
                        modeSaleReqProd.IsDelivered = false;
                        modeSaleReqProd.ProductionStatus = ProductionStatus.New;
                        modeSaleReqProd.Quantity = _saleReq.Quantity;
                        modeSaleReqProd.Value = Convert.ToDecimal(_saleReq.Product.SaleRetailPraticedString);
                        modeSaleReqProd.TotalLiquid = (_saleReq.Value * _saleReq.Quantity);
                        //modeSaleReqProd.SaleRequestId = saleRequestNew.SaleRequestId;
                        modeSaleReqProd.ProductPriceLookUpCode = _saleReq.ProductPriceLookUpCode;
                        modeSaleReqProd.ProductInternalCodeOnErp = _saleReq.ProductInternalCodeOnErp;
                        //modeSaleReqProd.SaleRequest = saleRequest;
                        //modeSaleReqProd.Product = _saleReq.Product;
                        saleRequestNew.TotalLiquid += _saleReq.TotalLiquid;
                        saleRequestNew.Products.Add(modeSaleReqProd);
                    }

                    //saleRequestsDao.Update(saleRequestNew);
                    saleRequestsDao.AddNew(saleRequestNew);
                    Delete(_saleRequestTemp);
                    return true;
                }

                
                foreach (var _saleReq in _saleRequestTemp.Products)
                {
                    


                    ModelSaleRequestProduct modeSaleReqProd = new ModelSaleRequestProduct();
                    modeSaleReqProd.Comments = null;
                    modeSaleReqProd.IsCancelled = false;
                    modeSaleReqProd.IsDelivered = false;
                    modeSaleReqProd.ProductionStatus = ProductionStatus.New;
                    modeSaleReqProd.Quantity = _saleReq.Quantity;
                    modeSaleReqProd.Value = Convert.ToDecimal(_saleReq.Product.SaleRetailPraticedString);
                    modeSaleReqProd.TotalLiquid = (_saleReq.Value * _saleReq.Quantity);
                    modeSaleReqProd.SaleRequestId = _saleReq.SaleRequestTemp.SaleRequestTempId;
                    modeSaleReqProd.ProductPriceLookUpCode = _saleReq.ProductPriceLookUpCode;
                    modeSaleReqProd.ProductInternalCodeOnErp = _saleReq.ProductInternalCodeOnErp;
                    modeSaleReqProd.SaleRequest = saleRequest;
                    saleRequest.TotalLiquid += _saleReq.TotalLiquid;
                    saleRequest.Products.Add(modeSaleReqProd);
                }

                saleRequestsDao.Update(saleRequest);
                Delete(_saleRequestTemp);
                return true;
            }
            catch (Exception err)
            {
                return false;
                throw err;
            }
        }

        public void Delete(ModelSaleRequestTemp saleRequestTemp)
        {
            context.SaleRequestsTemp.Remove(saleRequestTemp);
            context.SaveChanges();
        }

        public void DeleteNew(int id)
        {
            var d = context.SaleRequestsTemp.Find(id);
            context.SaleRequestsTemp.Remove(d);
            context.SaveChanges();
        }
    }
}
