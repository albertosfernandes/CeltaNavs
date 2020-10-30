using CeltaNavs.Domain;
using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CeltaNavsApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class APIProductController : ApiController
    {
        private ProductDao productDao = new ProductDao();
        private NavsSettingDao settingsDao = new NavsSettingDao();
        private ModelNavsSetting navsSettings = new ModelNavsSetting();

        [HttpGet]
        public ModelProduct Get(int _enterpriseId, string _productCode)
        {
            try
            {

                navsSettings = settingsDao.GetByEnterprise(_enterpriseId);                
                var result = productDao.FindByPlu(_productCode, navsSettings);
                if (result == null)
                {
                    result = productDao.FindByEan(_productCode, navsSettings);
                }
                else if(result == null)
                {
                   //deve ser descrição entãos
                }
                return  result;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
