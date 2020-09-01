using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using CeltaNavsApi.Helpers;

using CeltaNavs.Repository;
using CeltaNavs.Domain;

namespace CeltaNavsApi.Controllers
{
    public class NavsSitefController : ApiController
    {
        private string navsIp;
        private string navsPort;        

        ModelNavsSetting modelSetting = new ModelNavsSetting();
        ModelSaleMovementFinalization saleRequestFin = new ModelSaleMovementFinalization();

        NavsSettingDao navsSettingsDao = new NavsSettingDao();
        SaleMovementFinalizationDao saleRequestFinDao = new SaleMovementFinalizationDao();

        public NavsSitefController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }

        [HttpGet]
        public HttpResponseMessage PrintSitef(string RESPAG, string BINCART, string NAMEINST, string NSUAUT, string NSUSIT, string NSUCAN, string CODAUT, string NPARCEL, string TIPOTRANS, string TIPOCART, string CODADQ, string _PAYSITEFPRINTCARD, string _PAYSITEFPRINTTERMINALSERIAL, string _PAYSITEFPRINTPAYVALUE, string _SITEFMODALITY)
        {
            modelSetting = navsSettingsDao.GetById(_PAYSITEFPRINTTERMINALSERIAL);

            string XML = $"<BR><BR><CONSOLE>Gravando informacoes<BR>";
            XML += $"Resposta: {RESPAG} <BR>";
            XML += $"Cartão: {BINCART}<BR> Nome:  {NAMEINST}<BR> NSU: {NSUAUT}, <BR>TIPO TRAnsacao: {TIPOTRANS}, <BR> tipo cartao: {TIPOCART}, <BR>adq: {CODADQ} <BR> autorizador: {CODAUT}";            
            XML += "</console>";            
            if (RESPAG == "APROVADO")
            {
                //validar o tipo de finalizadora!     
                if (_SITEFMODALITY == "DEBITO")
                    saleRequestFin.FinalizationId = 5;
                if (_SITEFMODALITY == "CREDITO")
                    saleRequestFin.FinalizationId = 6;
                saleRequestFin.EnterpriseId = modelSetting.EnterpriseId;
                saleRequestFin.PdvId = modelSetting.PdvId;
                saleRequestFin.PersonalizedCode = _PAYSITEFPRINTCARD;
                saleRequestFin.Value = Convert.ToDecimal(_PAYSITEFPRINTPAYVALUE);
                saleRequestFin.PaymentResponse = RESPAG;
                saleRequestFin.BinCard = BINCART;
                saleRequestFin.InstitutionalName = NAMEINST;
                saleRequestFin.NSUAUT = NSUAUT;
                saleRequestFin.NSUSIT = NSUSIT;
                saleRequestFin.CODAUT = CODAUT;
                saleRequestFin.NPARCEL = NPARCEL;
                saleRequestFin.TIPOTRANS = TIPOTRANS;
                saleRequestFin.TIPOCART = TIPOCART;
                saleRequestFin.CODADQ = CODADQ;
                saleRequestFin.NSUCAN = NSUCAN;

                saleRequestFinDao.Add(saleRequestFin);
            }

            XML += $"<GET TYPE=HIDDEN NAME=_TOTALCARD VALUE={_PAYSITEFPRINTCARD}>";
            XML += $"<GET TYPE=HIDDEN NAME=_TOTALTERMINALSERIAL VALUE={_PAYSITEFPRINTTERMINALSERIAL}>";
            XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navstotalize/GetTotal HOST=h timeout=10>";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(XML, Encoding.UTF8, "application/xml")
            };
        }

        [HttpGet]
        public HttpResponseMessage Cancel(string _SITEFCANCELCARD, string _SITEFCANCELTERMINALSERIAL)
        {
            string XML = "";
            try
            {
                modelSetting = navsSettingsDao.Get(_SITEFCANCELTERMINALSERIAL);

                List<ModelSaleMovementFinalization> listOfSaleMovementFinalizations = saleRequestFinDao.GetAll(_SITEFCANCELCARD, modelSetting);

                XML +=  $"<console>Cancelando transacoes tef...</console>";
                
                if(listOfSaleMovementFinalizations.Count > 0)
                {
                    XML = Menu.MenuSalesSitef(listOfSaleMovementFinalizations);
                    XML += $"<console>Quantidade de vendas tef: {listOfSaleMovementFinalizations.Count.ToString()}</console>";
                    XML += "<delay time=1>";
                    XML += $"<GET TYPE=HIDDEN NAME=_CANCELTEFCARD VALUE={_SITEFCANCELCARD}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_SERIALCANCEL VALUE={_SITEFCANCELTERMINALSERIAL}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssitef/Canceling HOST=h timeout=10>";                    

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }
                
                XML += $"<GET TYPE=HIDDEN NAME=_OPTPAY VALUE={"5"}>";
                XML += $"<GET TYPE=HIDDEN NAME=_CARDPAY VALUE={_SITEFCANCELCARD}>";
                XML += $"<GET TYPE=HIDDEN NAME=_TOTALPAY VALUE={0}>";
                XML += $"<GET TYPE=HIDDEN NAME=_SERIALPAY VALUE={_SITEFCANCELTERMINALSERIAL}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsPay/GetOptionPay HOST=h timeout=10>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };



            }
            catch (Exception err)
            {
                string message = Formatted.FormatError(err.Message);
                XML += $"<console>{message}</console>";
            }
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(XML, Encoding.UTF8, "application/xml")
            };
        }

        [HttpGet]
        public HttpResponseMessage Canceling(string _STEF, string _CANCELTEFCARD, string _SERIALCANCEL)
        {            
            string XML = "";
            XML += $"<console>Cancelando transacao id: {_STEF}</console>";
            XML += "<get type=anykey>";
            //int idSaleOrderFin = Convert.ToInt32(_STEF);
            try
            {
                modelSetting = navsSettingsDao.Get(_SERIALCANCEL);
                //ModelSaleMovementFinalization salesOrderFinalizations = saleRequestFinDao.GetSaleOrderFinalization(idSaleOrderFin);
                ModelSaleMovementFinalization salesOrderFinalizations = saleRequestFinDao.Get(_CANCELTEFCARD, modelSetting);
                if (salesOrderFinalizations.FinalizationId == 1)
                {
                    XML += $"<GET TYPE=HIDDEN NAME=_OPTPAY VALUE={5}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_CARDPAY VALUE={_CANCELTEFCARD}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_TOTALPAY VALUE={0}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_SERIALCANCEL VALUE={_SERIALCANCEL}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsPay/GetOptionPay HOST=h timeout=10>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }
                
                XML += $"<PAY> PAGRET=RESPAG; IPTEF={modelSetting.SitefAddressIp}; PORTATEF={modelSetting.SitefPort}; CODLOJA={modelSetting.SitefStoreCode}; TIPO=ESTORNO;";
                XML += $" BIN=BINCART; NINST=NAMEINST; NSU=NSUAUT; NSUSITEF=NSUSIT; NSUCANC=NSUCAN; AUT=CODAUT; NPAR=NPARCEL;";
                XML += $" MODPAG=TIPOTRANS; TIPOCAR=TIPOCART; REDEADQ=CODADQ; NSUHOST={salesOrderFinalizations.NSUSIT}; VALOR={salesOrderFinalizations.Value}; </PAY>";
                //string _RESPAG, string _CHECKSALEORDERID, string _CHECKTEFCARD
                XML += "<console>enviando para checar informacoes</console>";
                XML += "<get type=anykey>";
                XML += $"<GET TYPE=HIDDEN NAME=_CHECKSALEORDERID VALUE={_STEF}>";
                XML += $"<GET TYPE=HIDDEN NAME=_CHECKTEFCARD VALUE={_CANCELTEFCARD}>";

                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssitef/checkcanceltef HOST=h timeout=10>";
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
        public HttpResponseMessage CheckCancelTef(string RESPAG, string BINCART, string NAMEINST, string NSUAUT, string NSUSIT, string NSUCAN, string CODAUT, string NPARCEL, string TIPOTRANS, string TIPOCART, string CODADQ, string _CHECKSALEORDERID, string _CHECKTEFCARD)
        {
            string XML = "";
            try
            {
                XML += $"<console>Resposta sitef {RESPAG}</console>";
                XML += "<delay time=1>";
                if (RESPAG == "APROVADO")
                {
                    XML += $"<console>Cancelando transacao id: {_CHECKSALEORDERID}</console>";
                    int idSalesOrderFin = Convert.ToInt32(_CHECKSALEORDERID);
                    saleRequestFinDao.RemovePaymentsTef(_CHECKTEFCARD, idSalesOrderFin);
                    
                    XML += $"<GET TYPE=HIDDEN NAME=_SITEFCANCELCARD VALUE={_CHECKTEFCARD}>";
                    XML += $"<GET TYPE=SERIALNO NAME=_SITEFCANCELTERMINALSERIAL>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssitef/cancel HOST=h timeout=10>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }
                XML += $"<GET TYPE=HIDDEN NAME=_SITEFCANCELCARD VALUE={_CHECKTEFCARD}>";
                XML += $"<GET TYPE=SERIALNO NAME=_SITEFCANCELTERMINALSERIAL>";
                //Cancel(string _SITEFCANCELCARD, string _SITEFCANCELTERMINALSERIAL)
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssitef/cancel HOST=h timeout=10>";
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
