using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CeltaNavsApi.Controllers
{
    public class APIPrintServicesController : ApiController
    {
        [HttpGet]
        public List<ModelSaleRequestProduct> GetForPrint(string _enterpriseId)
        {
            return GetForPrint(_enterpriseId);
        }

        [HttpGet]
        public HttpStatusCode MarkToPrinted(int _saleRequestProductId)
        {
            return HttpStatusCode.OK;
        }
    }
}
