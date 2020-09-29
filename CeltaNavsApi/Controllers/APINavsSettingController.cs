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
    public class APINavsSettingController : ApiController
    {
        NavsSettingDao settingsDao = new NavsSettingDao();
        EnterpriseDao enterpriseDao = new EnterpriseDao();

        [HttpGet]
        public ModelNavsSetting Get(string _enterpriseId, string _pdv)
        {
            return settingsDao.GetByEnterpriseAndPdv(_enterpriseId, _pdv);
        }

        [HttpGet]
        public List<ModelEnterprise> GetAllEnterprises()
        {
             return settingsDao.GetAllEnterprises();
        }

        [HttpGet]
        public IHttpActionResult GetEnterpriseByPersonalizedCode(int _personalizedCode)
        {
            var result = enterpriseDao.GetByPersonalizedCode(_personalizedCode.ToString());
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        public List<ModelPdv> GetAllPdvs(string _enterpriseid)
        {
            if (string.IsNullOrEmpty(_enterpriseid))
            {
                return null;
            }
            return settingsDao.GetAllPdvs(_enterpriseid);
        }

        [HttpPut]
        public HttpStatusCode UpdateSetting(ModelNavsSetting modelNavsSettings)
        {
            try
            {
                settingsDao.UpdateNavsSettings(modelNavsSettings);
                return HttpStatusCode.OK;
            }
            catch(Exception err)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
