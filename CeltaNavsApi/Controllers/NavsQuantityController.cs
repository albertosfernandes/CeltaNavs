using CeltaNavs.Domain;
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
    public class NavsQuantityController : BaseController
    {
        //private string navsIp;
        //private string navsPort;

        public NavsQuantityController()
        {
            //navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            //navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }

        //ModelNavsSetting modelSetting = new ModelNavsSetting();

        //NavsSettingDao settingsDao = new NavsSettingDao();
        ProductDao productsDao = new ProductDao();

        public HttpResponseMessage Get(string _SELPROD, string _PERSONALIZEDCODE, string _TERMINALSERIAL)
        {
            string XML = "";
            try
            {
                modelSetting = settingsdao.GetById(_TERMINALSERIAL);
                ModelProduct myproduct = new ModelProduct();
                if (_SELPROD.Contains("-"))
                {
                    myproduct = productsDao.FindByPlu(_SELPROD, modelSetting);
                }
                else
                {
                    myproduct = productsDao.FindByInternalCode(_SELPROD, modelSetting);
                }

                if(myproduct == null)
                {
                    XML += $"<CONSOLE>Codigo de produto nao encontrado.<BR>";
                    XML += $"----------------------------------------<BR><BR>";
                    XML += $"Codigo: {_SELPROD}<BR></CONSOLE>";
                    XML += " <DELAY TIME = 01> ";
                    XML += $"<GET TYPE=HIDDEN NAME=_TABLE VALUE={_PERSONALIZEDCODE}>";
                    XML += $"<GET TYPE=SERIALNO NAME=_TSERIAL>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssaleRequest/get HOST=h TIMEOUT=5>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

                XML += $"<CONSOLE>Informe a quantidade.<BR>";
                XML += $"----------------------------------------<BR><BR>";
                XML += $"Codigo do Produto: {myproduct.PriceLookupCode}<BR>";
                XML += $"Nome: {myproduct.NameReduced}<BR>";
                XML += $"Valor: {myproduct.SaleRetailPraticedString}<BR></CONSOLE>";

                XML += "<RECTANGLE NAME=RETCARD X=53 Y=198 WIDTH=150 HEIGHT=28 VISIBLE=1 COLOR=ccc> ";
                XML += $"<WRITE_AT LINE=12 COLUMN=7>Informe a quantidade</WRITE_AT>";
                XML += "<GET TYPE=FIELD NAME=_QUANT LIN=14 COL=7 SIZE=2 >";
                XML += $"<WRITE_AT LINE=29 COLUMN=1>________________________________________</WRITE_AT>";

                XML += $"<GET TYPE=HIDDEN NAME=_PRODUCT VALUE={_SELPROD}>";
                XML += $"<GET TYPE=HIDDEN NAME=_PERSONALIZEDSALECODE VALUE={_PERSONALIZEDCODE}>";
                XML += $"<GET TYPE=HIDDEN NAME=_POSSERIAL VALUE={_TERMINALSERIAL}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsproducts/AddProduct HOST=h TIMEOUT=5>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch(Exception err)
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
