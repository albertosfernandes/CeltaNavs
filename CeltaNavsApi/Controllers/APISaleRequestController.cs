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
    public class APISaleRequestController : ApiController
    {        
        private SaleRequestDao saleRequestDao = new SaleRequestDao();
        private SaleRequestTempDao saleRequestTempDao = new SaleRequestTempDao();

        [HttpGet]
        public List<ModelSaleRequest> GetAll(string _enterpriseId)
        {
            return saleRequestDao.GetAll(_enterpriseId);
        }

        [HttpGet]
        public ModelSaleRequest Get(string _enterpriseId, string _personalizedCode, bool _considerUsing)
        {
            return saleRequestDao.Get(_enterpriseId, _personalizedCode, _considerUsing);
        }        

        [HttpPost]
        public HttpStatusCode AddSaleRequest(ModelSaleRequest saleRequest)
        {
            try
            {
                saleRequestDao.Add(saleRequest);

                return HttpStatusCode.OK;
            }
            catch (Exception err)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        [HttpPost]
        public HttpStatusCode AddSaleRequestTemp(ModelSaleRequestTemp saleRequestTemp)
        {
            try
            {
                saleRequestTempDao.Add(saleRequestTemp);

                return HttpStatusCode.OK;
            }
            catch (Exception err)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        [HttpPut]
        public HttpStatusCode UpdateSaleRequest(ModelSaleRequest _saleRequest)
        {
            try
            {
                saleRequestDao.Update(_saleRequest);
                return HttpStatusCode.OK;
            }
            catch (Exception err)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
