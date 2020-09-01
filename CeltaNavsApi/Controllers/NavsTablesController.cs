using CeltaNavs.Domain;
using CeltaNavs.Domain.SaleRequest;
using CeltaNavs.Repository;
using CeltaNavsApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;

namespace CeltaNavsApi.Controllers
{
    public class NavsTablesController : ApiController
    {
        private string navsIp;
        private string navsPort;
        private HttpClient _httpClient = null;

        private BkpSaleRequestDao saleRequestDao = new BkpSaleRequestDao();
        private SaleRequestDao saleRequestsDao = new SaleRequestDao();
        ModelNavsSetting modelSetting = new ModelNavsSetting();
        NavsSettingDao settings = new NavsSettingDao();

        public NavsTablesController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.BaseAddress = new Uri($"http://{navsIp}:{navsPort}");
        }

        [HttpGet]
        public HttpResponseMessage GetOpenTables(string _TABLESERIALNUMBER)
        {
            string XML = "";
            try
            {                
                modelSetting = settings.Get(_TABLESERIALNUMBER);

                //var listTables = saleRequestDao.GetTablesOpen(modelSettings);

                var listOfTables = saleRequestsDao.GetAll(modelSetting.EnterpriseId.ToString());
                if (listOfTables.Count < 1)
                {
                    XML += $"<CONSOLE><BR><BR>Nao existem comandas em aberto <BR>";
                    XML += "----------------------------------------<BR><BR></CONSOLE>";
                    XML += "<delay time=1>";
                    XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_TABLESERIALNUMBER}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/start HOST=h TIMEOUT=5>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

                XML += $"<CONSOLE> Lista de comandas/mesas em aberto<BR>";
                XML += "----------------------------------------<BR><BR></CONSOLE>";

                XML += Menu.MenuListTables(listOfTables);

                XML += $"<WRITE_AT LINE=29 COLUMN=1>________________________________________</WRITE_AT>";
                XML += $"<GET TYPE=HIDDEN NAME=_TSERIAL VALUE={_TABLESERIALNUMBER}>";                
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssalerequest/Get HOST=h TIMEOUT=5>";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch (Exception err)
            {
                string message = Formatted.FormatError(err.Message);
                XML += $"<console>{message}</console>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }
        
    }
}
