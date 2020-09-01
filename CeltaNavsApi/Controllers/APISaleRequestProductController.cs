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

        [HttpGet]
        public List<ModelSaleRequestProduct> GetAll(string _enterpriseId, bool _isConsiderDelivery)
        {
            return saleRequestProdDao.NewGetAll(_enterpriseId, _isConsiderDelivery);
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

        [HttpPut]
        public HttpResponseMessage Update(string _saleRequestProductId)
        {
            try
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

                saleRequestProdDao.MarkToDelivery(_saleRequestProductId);

                return response;

            }
            catch(Exception err)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return response;
            }
        }
    }
}
