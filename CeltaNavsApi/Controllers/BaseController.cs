using CeltaNavs.Domain;
using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace CeltaNavsApi.Controllers
{
    public class BaseController : ApiController
    {
        protected string navsIp;
        protected string navsPort;
        protected string characters;
        protected HttpClient _httpClient = null;
        //protected ModelNavsSetting modelSetting = new ModelNavsSetting();
        //protected NavsSettingDao settingsDao = new NavsSettingDao();


        public BaseController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
            characters = WebConfigurationManager.AppSettings.Get("");

            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.BaseAddress = new Uri($"http://{navsIp}:{navsPort}");

        }
    }
}
