using CeltaNavs.Repository;
using CeltaNavs.Domain;
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
    public class NavsGroupsController : ApiController
    {
        private string navsIp;
        private string navsPort;

        ModelExpansibleGroup expandableGroups = new ModelExpansibleGroup();
        ModelNavsSetting modelSetting = new ModelNavsSetting();

        NavsSettingDao navsSettingsDao = new NavsSettingDao();
        ExpansibleGroupDao groupDao = new ExpansibleGroupDao();


        public NavsGroupsController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }

        [HttpGet]
        public HttpResponseMessage GetAll(string _GROUPSTABLE, string _GROUPSTERMINALSERIAL)
        {
            string XML = $"";
            try
            {
                modelSetting = navsSettingsDao.Get(_GROUPSTERMINALSERIAL);
                List<ModelExpansibleGroup> listOfGroup = groupDao.Get(modelSetting);

                XML += "<CONSOLE>  Lista de Grupos<BR>";
                XML += "----------------------------------------<BR>";               
                XML += string.Format("{0,-3} | {1,-2}", "Codigo", "Descricao" + "<BR>");
                XML += "----------------------------------------<BR></CONSOLE>";
                
                XML += Menu.MenuListGroup(listOfGroup);
                
                XML += $"<GET TYPE=HIDDEN NAME=_PRODUCTTABLE VALUE={_GROUPSTABLE}>";
                XML += $"<GET TYPE=HIDDEN NAME=_PRODUCTTERMINALSERIAL VALUE={_GROUPSTERMINALSERIAL}>";

                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsproducts/getwithgroup HOST=h TIMEOUT=5>";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch (Exception err)
            {
                string message = Formatted.FormatError(err.Message);
                XML = $"<console>{message}</console>";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }
    }
}
