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
        
        [HttpGet]
        public List<ModelSaleRequest> GetProduction(string _enterpriseId)
        {
            try
            {
                return saleRequestDao.GetAll(_enterpriseId);
                //List<ModelSaleRequest> saleRequestProductionList = new List<ModelSaleRequest>();
                //switch (productionStatusCode)
                //{
                //    case 0: { return saleRequestDao.GetProduction(Convert.ToInt32(_enterpriseId), ProductionStatus.New).ToList(); }
                //    case 1: { return saleRequestDao.GetProduction(Convert.ToInt32(_enterpriseId), ProductionStatus.InProduction); }
                //    case 2: { return saleRequestDao.GetProduction(Convert.ToInt32(_enterpriseId), ProductionStatus.Delivered); }
                //    default: { return saleRequestProductionList; }
                //}
            }
            catch(Exception err)
            {
                throw err;
            }
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
