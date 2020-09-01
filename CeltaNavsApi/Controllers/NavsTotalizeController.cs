using CeltaNavs.Domain;
using CeltaNavs.Domain.SaleRequest;
using CeltaNavs.Repository;
using CeltaNavsApi.Helpers;
using CeltaNavsApi.Services;
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
    public class NavsTotalizeController : ApiController
    {
        private string navsIp;
        private string navsPort;

        NavsSettingDao navsSettingsDao = new NavsSettingDao();
        ModelNavsSetting modelSetting = new ModelNavsSetting();
        ModelSaleRequest _saleRequest = new ModelSaleRequest();
        ModelSaleMovementFinalization modelNavsFinalization = new ModelSaleMovementFinalization();
        
        SaleRequestDao saleRequestDaoNew = new SaleRequestDao();
        SaleMovementFinalizationDao navsFinalizations = new SaleMovementFinalizationDao();

        public NavsTotalizeController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }

        [HttpGet]
        public HttpResponseMessage GetTotal(string _TOTALCARD, string _TOTALTERMINALSERIAL)
        {
            try
            {
                string XML = "";
                modelSetting = navsSettingsDao.Get(_TOTALTERMINALSERIAL);
                ModelSaleMovementFinalization lastFinalization = null;

                var saleRequest = saleRequestDaoNew.Get(modelSetting.EnterpriseId.ToString(), _TOTALCARD, false);
                saleRequest.IsUsing = false;
                List<ModelSaleMovementFinalization> listOfNavsFinalization = navsFinalizations.GetAll(saleRequest.PersonalizedCode, modelSetting);

                //decimal value = saleRequest.TotalLiquid;
                decimal valueAmountPaid = navsFinalizations.PaidAmountValue(_TOTALCARD, modelSetting);
                decimal faltaPagar = (saleRequest.TotalLiquid - valueAmountPaid);
                
                XML += $"<CONSOLE>----------------------------------------<BR>";
                XML += $"     Formas de Pagamento: <BR>";
                XML += "----------------------------------------<BR><BR>";
                XML += $"<BR> Pedido: {_TOTALCARD}<BR>";
                XML += $" Total do Pedido: {saleRequest.TotalLiquid.ToString("C")}<BR>";
                XML += " Pagamentos:<BR>";

                foreach (var finalization in listOfNavsFinalization)
                {                    
                    XML += $" -{finalization.Value.ToString()}<BR>";                    
                }

                XML += $"<BR> Falta pagar R$: {faltaPagar.ToString("0.00")}</CONSOLE>";

                if (listOfNavsFinalization.Count > 0)
                {
                    lastFinalization = (from r in listOfNavsFinalization
                                        orderby r.NavsFinalizationId descending
                                        select r).First();

                    //if tiver uma forma de pagamento bloqueia caso contrario não!!!
                    XML += "<CANCEL_KEY TYPE=DISABLE>";

                    saleRequest.IsUsing = true;
                }


                if (!saleRequestDaoNew.MarkInUse(saleRequest.SaleRequestId, saleRequest.IsUsing))
                {

                    //XML += $"<CONSOLE><BR><BR>Falha ao marcar o pedido em uso.<BR>Tente novamente.<br>Pressione uma tecla.</console>";
                    //XML += $"<GET TYPE=ANYKEY>";
                    XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_TOTALTERMINALSERIAL}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/START HOST=h  TIMEOUT=6>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

                if (faltaPagar <= 0)
                {
                    XML += $"<CONSOLE> Valor total pago</CONSOLE>";
                    XML += $"<GET TYPE=HIDDEN NAME=_COUPONCARD VALUE={_TOTALCARD}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_COUPONTERMINALSERIAL VALUE={_TOTALTERMINALSERIAL}>";

                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscoupon/isNotaPaulista HOST=h>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };

                    //if (lastFinalization.PayBackValue > 0) //tem troco!!!
                    //{
                    //    //lastFinalization.PayBackValue = (valueAmountPaid - value) ;
                    //    //navsFinalizations.Update(lastFinalization);
                    //    XML += $"<CONSOLE> Valor total pago</CONSOLE>";
                    //    XML += $"<GET TYPE=HIDDEN NAME=_COUPONCARD VALUE={_TOTALCARD}>";
                    //    XML += $"<GET TYPE=HIDDEN NAME=_COUPONTERMINALSERIAL VALUE={_TOTALTERMINALSERIAL}>";

                    //    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscoupon/isNotaPaulista HOST=h>";
                    //    return new HttpResponseMessage(HttpStatusCode.OK)
                    //    {
                    //        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    //    };
                    //}
                    //else
                    //{
                    //    XML += $"<CONSOLE> Valor total pago</CONSOLE>";
                    //    XML += $"<GET TYPE=HIDDEN NAME=_COUPONCARD VALUE={_TOTALCARD}>";
                    //    XML += $"<GET TYPE=HIDDEN NAME=_COUPONTERMINALSERIAL VALUE={_TOTALTERMINALSERIAL}>";

                    //    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscoupon/isNotaPaulista HOST=h>";
                    //    return new HttpResponseMessage(HttpStatusCode.OK)
                    //    {
                    //        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    //    };
                    //}
                }

                XML += $"<WRITE_AT LINE=24 COLUMN=1>1: Imprimir</WRITE_AT>";
                XML += $"<WRITE_AT LINE=25 COLUMN=1>2: Pagamento Dinheiro</WRITE_AT>";
                XML += $"<WRITE_AT LINE=26 COLUMN=1>3: Pagamento Cartao Debito</WRITE_AT>";
                XML += $"<WRITE_AT LINE=27 COLUMN=1>4: Pagamento Cartao Credito</WRITE_AT>";
                //XML += $"<WRITE_AT LINE=28 COLUMN=1>5: Cancelar pagamentos e voltar.</WRITE_AT>";
                XML += $"<WRITE_AT LINE=29 COLUMN=1>__________________________________>_____</WRITE_AT>";
                XML += "<GET TYPE=FIELD NAME=_OPTPAY LIN=29 COL=36 SIZE=1>";

                XML += $"<GET TYPE=HIDDEN NAME=_CARDPAY VALUE={_TOTALCARD}>";
                XML += $"<GET TYPE=HIDDEN NAME=_TOTALPAY VALUE={(saleRequest.TotalLiquid - valueAmountPaid).ToString("0.00")}>";
                XML += $"<GET TYPE=HIDDEN NAME=_SERIALPAY VALUE={_TOTALTERMINALSERIAL}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navspay/GetOptionPay HOST=h>";
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
