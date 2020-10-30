using CeltaNavs.Domain;
using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CeltaNavsApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class APISaleRequestProductController : ApiController
    {
        SaleRequestProductDao saleRequestProdDao = new SaleRequestProductDao();
        SaleRequestProductTempDao saleRequestProductTempDao = new SaleRequestProductTempDao();

        [HttpGet]
        public List<ModelSaleRequestProduct> GetAll(string _enterpriseId, bool _isConsiderDelivery)
        {
            return saleRequestProdDao.NewGetAll(_enterpriseId, _isConsiderDelivery);
        }

        [HttpGet]
        public List<ModelSaleRequestProduct> GetForPrint(string _enterpriseId)
        {
            int id = Convert.ToInt32(_enterpriseId);
            return saleRequestProdDao.GetForPrint(id);
        }

        [HttpGet]
        public List<ModelSaleRequestProduct> Get(string _enterpriseId, string saleRequesId)
        {
            return saleRequestProdDao.GetAll(_enterpriseId, saleRequesId);
        }

        [HttpGet]
        public List<ModelSaleRequestProduct> GetForDelivery(string _enterpriseId)
        {
            return saleRequestProdDao.GetForDelivery(_enterpriseId);
        }

        [HttpGet]
        public List<ModelSaleRequestProduct> GetDelivered(string _enterpriseId)
        {
            return saleRequestProdDao.GetDelivered(_enterpriseId);
        }

        [HttpGet]
        public List<ModelSaleRequestProduct> GetIsDelivered(string _enterpriseId, string _saleRequestId)
        {
            return saleRequestProdDao.GetProducts(_enterpriseId, _saleRequestId, true);
        }

        [HttpGet]
        public HttpResponseMessage Update(string _saleRequestProductId, int isMArk)
        {
            try
            {
                bool mark = Convert.ToBoolean(isMArk);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                if (mark)
                {
                    saleRequestProdDao.MarkToDelivery(_saleRequestProductId);
                }
                else
                {
                    saleRequestProdDao.UnMarkDelivery(_saleRequestProductId);
                }
                
                return response;

            }
            catch(Exception err)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return response;
            }
        }
        
        [HttpGet]
        public HttpResponseMessage UpdateStatus(string _saleRequestProductId, string statusproductioncocde)
        {
            try
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

                if (statusproductioncocde == "1")
                    saleRequestProdDao.MarkToProduction(_saleRequestProductId);
                else if (statusproductioncocde == "2")
                    saleRequestProdDao.MarkToReleased(_saleRequestProductId);

                return response;

            }
            catch (Exception err)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return response;
            }
        }        

        [HttpGet]
        public HttpResponseMessage ChangeProductionStatus(string _saleRequestProductId, int productionstatus)
        {
            try
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

                saleRequestProdDao.ChangeStatusProduction(_saleRequestProductId, productionstatus);

                return response;

            }
            catch (Exception err)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return response;
            }
        }

        [HttpGet]
        public IHttpActionResult MarkToPrinted(int _saleRequestProductId)
        {
            saleRequestProdDao.MarkToPrinted(_saleRequestProductId);
            return Ok();
        }

        [HttpPost]
        public HttpResponseMessage Add(ModelSaleRequestProductTemp _saleRequestProductTemp)
        {
            try
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                saleRequestProductTempDao.Add(_saleRequestProductTemp);
                return response;
            }
            catch(Exception err)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return response;
            }
        }

        [HttpGet]
        public HttpStatusCode DeleteSaleRequestProductTemp(int id)
        {
            try
            {
                saleRequestProductTempDao.Delete(id);
                return HttpStatusCode.OK;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
