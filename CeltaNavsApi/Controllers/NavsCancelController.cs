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
    public class NavsCancelController : BaseController
    {        
        private ModelNavsSetting modelSetting = new ModelNavsSetting();
        private ModelSaleRequest saleRequest = new ModelSaleRequest();

        private NavsSettingDao settingsDao = new NavsSettingDao();
        private SaleRequestDao saleRequestsDao = new SaleRequestDao();
        private SaleRequestProductDao saleRequestProductsDao = new SaleRequestProductDao();
        private SaleRequestTempDao saleRequestTempDao = new SaleRequestTempDao();

        public NavsCancelController()
        {
           
        }
        [HttpGet]
        public HttpResponseMessage Get(string _CANCELTABLE, string _CANCELSERIAL)
        {
            string XML = "";
            modelSetting = settingsDao.GetById(_CANCELSERIAL);
            try
            {
                XML += $"<CONSOLE>Cancelamento.<BR>";
                XML += $"----------------------------------------<BR><BR>";
                XML += "</CONSOLE>";
                XML += $"<WRITE_AT LINE=27 COLUMN=2>0s: Cancelar item do pedido atual.</WRITE_AT>";
                XML += $"<WRITE_AT LINE=28 COLUMN=2>ENTER: Cancelar pedido atual.</WRITE_AT>";
                XML += $"<WRITE_AT LINE=29 COLUMN=1>_________________Opcao Cancelamento_>____</WRITE_AT>";
                XML += "<GET TYPE=FIELD NAME=_OPCAO LIN=29 COL=36 SIZE=3>";
                XML += $"<GET TYPE=HIDDEN NAME=_CANCELTABLE VALUE={_CANCELTABLE}>";
                XML += $"<GET TYPE=HIDDEN NAME=_CANCELSERIAL VALUE={_CANCELSERIAL}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscancel/option HOST=h>";
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

        [HttpGet]
        public HttpResponseMessage Option(string _OPCAO, string _CANCELTABLE, string _CANCELSERIAL)
        {
            string XML = "";
            modelSetting = settingsDao.GetById(_CANCELSERIAL);
            var resultSaleRequest = saleRequestsDao.Get(modelSetting.EnterpriseId.ToString(), _CANCELTABLE, true);
            var resultsaleRequestTemp = saleRequestTempDao.Get(modelSetting.EnterpriseId.ToString(), _CANCELTABLE, true);
            try
            {
                XML += $"<CONSOLE>Cancelamento.<BR>";
                XML += $"----------------------------------------<BR><BR>";
                XML += "</CONSOLE>";

                //Apaga pedido atual
                if (String.IsNullOrEmpty(_OPCAO))
                {
                    if(resultsaleRequestTemp != null)
                    {
                        saleRequestTempDao.Delete(resultsaleRequestTemp);
                    }
                    else
                    {
                        saleRequestsDao.Delete(resultSaleRequest.SaleRequestId);
                    }
                    XML += $"<CONSOLE>Pedido cancelado.<BR>";
                    XML += $"----------------------------------------<BR><BR>";
                    XML += "</CONSOLE>";
                }

                XML += $"<GET TYPE=SERIALNO NAME=_SERIALNUMBER>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/start HOST=h TIMEOUT=5>";
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

        [HttpGet]
        public HttpResponseMessage CancelProduct(string _TABLE, string _POSSERIAL)
        {
            try
            {
                string XML = "";

                modelSetting = settingsDao.Get(_POSSERIAL);
                var mySaleRequestId = saleRequestsDao.GetById(modelSetting.EnterpriseId.ToString(), _TABLE, false);
                //var listItensSaleReq = saleRequestDao.GetItensSaleRequest(modelSettings, _TABLE);
                var listOfItensSaleReq = saleRequestsDao.GetSaleRequestProducts(modelSetting.EnterpriseId.ToString(), mySaleRequestId.ToString(), false);

                XML += $"<CONSOLE>Pedido: {_TABLE}  <BR>";
                XML += "----------------------------------------<BR>";
                XML += string.Format("{0,-7}| {1,-21}| {2,3}", "Codigo", "Descricao", "Quant." + "<BR>");

                XML += "----------------------------------------<BR></CONSOLE>";

                XML += Menu.MenuListSaleRequestProducts(listOfItensSaleReq);

                XML += $"<GET TYPE=HIDDEN NAME=_CARDREMOVE VALUE={_TABLE}>";
                XML += $"<GET TYPE=HIDDEN NAME=_REMOVETERMINALSERIAL VALUE={_POSSERIAL}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsSaleRequest/cancelProductId HOST=h TIMEOUT=5>";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch (Exception err)
            {
                string message = Formatted.FormatError(err.Message);
                string XML = $"<console>{message}</console>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }

        [HttpGet]
        public HttpResponseMessage CancelProductId(string _SELMOVID, string _CARDREMOVE, string _REMOVETERMINALSERIAL)
        {
            try
            {
                string XML = "";
                XML += $"<console>MovId: {_SELMOVID}<BR>";
                XML += $"<console>Pedido: {_CARDREMOVE}<BR>";
                XML += $"<console>Serial: {_REMOVETERMINALSERIAL}<BR></console>";
                XML += "<get type=anykey>";
                modelSetting = settingsDao.Get(_REMOVETERMINALSERIAL);
                saleRequestProductsDao.CancelIten(_SELMOVID);

                saleRequest = saleRequestsDao.Get(modelSetting.EnterpriseId.ToString(), _CARDREMOVE, false);
                var listSaleRequestProducts = saleRequestsDao.GetSaleRequestProducts(modelSetting.EnterpriseId.ToString(), saleRequest.SaleRequestId.ToString(), false);
                saleRequestsDao.Update(saleRequest);

                XML += $"<CONSOLE><BR><BR>Cancelado com sucesso. <BR></CONSOLE>";
                XML += $"<DELAY TIME=01>";

                XML += $"<GET TYPE=HIDDEN NAME=_TABLE VALUE={_CARDREMOVE}>";
                XML += $"<GET TYPE=SERIALNO NAME=_TSERIAL>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssaleRequest/get HOST=h TIMEOUT=5>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch (Exception err)
            {
                string message = Formatted.FormatError(err.Message);
                string XML = $"<console>{message}</console>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }
    }
}
