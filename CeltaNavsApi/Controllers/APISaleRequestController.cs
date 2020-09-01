using CeltaNavs.Domain.SaleRequest;
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
        SaleRequestDao saleRequestDaoNew = new SaleRequestDao();

        [HttpGet]
        public List<ModelSaleRequest> GetAll(string _enterpriseId)
        {
            return saleRequestDaoNew.GetAll(_enterpriseId);
        }

        [HttpGet]
        public ModelSaleRequest Get(string _enterpriseId, string _personalizedCode, bool _considerUsing)
        {
            return saleRequestDaoNew.Get(_enterpriseId, _personalizedCode, _considerUsing);
        }        

        [HttpPost]
        public HttpStatusCode AddSaleRequest(ModelSaleRequest saleRequest)
        {
            try
            {
                saleRequestDaoNew.Add(saleRequest);

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
                saleRequestDaoNew.Update(_saleRequest);
                return HttpStatusCode.OK;
            }
            catch (Exception err)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
