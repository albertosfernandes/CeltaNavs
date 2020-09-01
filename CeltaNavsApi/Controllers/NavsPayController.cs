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
    public class NavsPayController : ApiController
    {
        private string navsIp;
        private string navsPort;

        ModelNavsSetting modelSetting = new ModelNavsSetting();
        ModelSaleMovementFinalization navsFinalization = new ModelSaleMovementFinalization();
        ModelSaleRequest saleRequest = new ModelSaleRequest();


        NavsSettingDao settingsDao = new NavsSettingDao();        
        SaleRequestDao saleRequestDaoNew = new SaleRequestDao();
        SaleMovementFinalizationDao saleRequestFinalizationsDao = new SaleMovementFinalizationDao();
        SaleMovementFinalizationDao navsFinalizationsDao = new SaleMovementFinalizationDao();

        public NavsPayController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }

        [HttpGet]
        public HttpResponseMessage GetOptionPay(string _OPTPAY, string _CARDPAY, string _TOTALPAY, string _SERIALPAY)
        {
            try
            {
                string XML = "";
                modelSetting = settingsDao.Get(_SERIALPAY);

                if (_OPTPAY == "1")
                {                    
                    var saleRequestId = saleRequestDaoNew.GetById(modelSetting.EnterpriseId.ToString(), _CARDPAY, false);
                    var listItensSaleReq = saleRequestDaoNew.GetSaleRequestProducts(modelSetting.EnterpriseId.ToString(), saleRequestId.ToString(), false);

                    //XML += Printer.Print(_CARDPAY, listItensSaleReq, modelSetting, false);  
                    XML += $"<GET TYPE=HIDDEN NAME=_PEOPLETERMINALSERIAL VALUE={_SERIALPAY}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_CARDPEOPLE VALUE={_CARDPAY}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navspeoples/get HOST=h timeout=10>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

                if (_OPTPAY == "2")
                {
                    XML += "<CANCEL_KEY TYPE=DISABLE>";
                    XML += $"<CONSOLE>    DINHEIRO     <BR>";
                    XML += "----------------------------------------<BR><BR></CONSOLE>";
                    XML += "<RECTANGLE NAME=RETCARD X=50 Y=150 WIDTH=200 HEIGHT=100 VISIBLE=1 COLOR=ccc> ";
                    XML += $"<WRITE_AT LINE=13 COLUMN=11>Valor em dinheiro</WRITE_AT>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYTOTALMONEY VALUE={_TOTALPAY}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYMONEYCARD VALUE={_CARDPAY}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYMONEYSERIAL VALUE={_SERIALPAY}>";
                    XML += "<GET TYPE=VALUE NAME=_PAYVALUEMONEY LIN=29 COL=31 SIZE=5 DECIMALS=2 ZL>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navspay/AddMoney HOST=h timeout=10>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };

                }

                if (_OPTPAY == "3")
                {
                    XML += "<CANCEL_KEY TYPE=DISABLE>";
                    XML += $"<CONSOLE>    CARTAO  DEBITO   <BR>";
                    XML += "----------------------------------------<BR><BR></CONSOLE>";
                    XML += "<RECTANGLE NAME=RETCARD X=50 Y=150 WIDTH=200 HEIGHT=100 VISIBLE=1 COLOR=ccc> ";
                    XML += $"<WRITE_AT LINE=13 COLUMN=11>Valor em cartao</WRITE_AT>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYTOTALTEF VALUE={_TOTALPAY}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYMONEYCARD VALUE={_CARDPAY}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYMONEYSERIAL VALUE={_SERIALPAY}>";
                    XML += "<GET NAME=_PAYVALUETEF TYPE=VALUE LIN=29 COL=31 SIZE=5 DECIMALS=2 ZL>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYMODALITY VALUE={"DEBITO"}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navspay/AddTef HOST=h timeout=10>";

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

                if (_OPTPAY == "4")
                {
                    XML += "<CANCEL_KEY TYPE=DISABLE>";
                    XML += $"<CONSOLE>    CARTAO  CREDITO   <BR>";
                    XML += "----------------------------------------<BR><BR></CONSOLE>";
                    XML += "<RECTANGLE NAME=RETCARD X=50 Y=150 WIDTH=200 HEIGHT=100 VISIBLE=1 COLOR=ccc> ";
                    XML += $"<WRITE_AT LINE=13 COLUMN=11>Valor em cartao</WRITE_AT>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYTOTALTEF VALUE={_TOTALPAY}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYMONEYCARD VALUE={_CARDPAY}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYMONEYSERIAL VALUE={_SERIALPAY}>";
                    XML += "<GET NAME=_PAYVALUETEF TYPE=VALUE LIN=29 COL=31 SIZE=5 DECIMALS=2 ZL>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYMODALITY VALUE={"CREDITO"}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navspay/AddTef HOST=h timeout=10>";                    
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

                if (_OPTPAY == "5")
                {
                    XML += $"<CONSOLE>Consultando valores para cancelamento.<BR><BR></CONSOLE>";
                    XML += "<get type=anykey>";
                    List<ModelSaleMovementFinalization> listOfSaleMovementFinalizations = saleRequestFinalizationsDao.GetAll(_CARDPAY, modelSetting);

                    if(listOfSaleMovementFinalizations.Count < 1)
                    {
                        XML += $"<CONSOLE>Todas vendas canceladas.<BR><BR></CONSOLE>";
                        XML += "<delay time=1>";
                        XML += "<get type=anykey>";
                        saleRequest = saleRequestDaoNew.Get(modelSetting.EnterpriseId.ToString(), _CARDPAY, false);
                        saleRequestDaoNew.MarkInUse(saleRequest.SaleRequestId, false);

                        XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_SERIALPAY}>";
                        XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsCommands/Start HOST=h>";
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                        };
                    }

                    foreach (var item in listOfSaleMovementFinalizations)
                    {
                        if(item.FinalizationId == 1)
                            saleRequestFinalizationsDao.RemovePaymentsMoney(item);
                        else
                        {
                            XML += $"<GET TYPE=HIDDEN NAME=_SITEFCANCELCARD VALUE={_CARDPAY}>";
                            XML += $"<GET TYPE=HIDDEN NAME=_SITEFCANCELTERMINALSERIAL VALUE={_SERIALPAY}>";
                            XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssitef/cancel HOST=h>";
                            return new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                            };
                        }
                    }

                    XML += $"<CONSOLE>Todas vendas canceladas.<BR><BR></CONSOLE>";
                    XML += "<delay time=1>";
                    XML += "<get type=anykey>";
                    saleRequest = saleRequestDaoNew.Get(modelSetting.EnterpriseId.ToString(), _CARDPAY, false);
                    saleRequestDaoNew.MarkInUse(saleRequest.SaleRequestId, false);

                    XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_SERIALPAY}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsCommands/Start HOST=h>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

                XML += $"<CONSOLE><BR><BR>Opcao invalida.</CONSOLE>";
                XML += "<delay time=1>";
                XML += $"<GET TYPE=HIDDEN NAME=_TOTALCARD VALUE={_CARDPAY}>";
                XML += $"<GET TYPE=HIDDEN NAME=_TOTALTERMINALSERIAL VALUE={_SERIALPAY}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navstotalize/gettotal HOST=h timeout=10>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch(Exception err)
            {
                saleRequestDaoNew.MarkInUse(saleRequest.SaleRequestId, false);
                string message = Formatted.FormatError(err.Message);
                string XML = $"<console>{message}</console>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }


        [HttpGet]
        public HttpResponseMessage AddMoney(string _PAYTOTALMONEY, string _PAYMONEYCARD, string _PAYMONEYSERIAL, string _PAYVALUEMONEY)
        {
            string XML = $"<CONSOLE><BR><BR> Registrando o valor em dinheiro";           

            try
            {
                modelSetting = settingsDao.Get(_PAYMONEYSERIAL);
                saleRequest = saleRequestDaoNew.Get(modelSetting.EnterpriseId.ToString(), _PAYMONEYCARD, false);
                if (string.IsNullOrEmpty(_PAYVALUEMONEY))
                {
                    XML += $"<BR><BR><BR> Registrando valor Total: {_PAYTOTALMONEY}";
                    navsFinalization.FinalizationId = 1;
                    navsFinalization.PersonalizedCode = _PAYMONEYCARD;
                    navsFinalization.Value = Convert.ToDecimal(_PAYTOTALMONEY);
                    navsFinalization.EnterpriseId = modelSetting.EnterpriseId;
                    navsFinalization.PdvId = modelSetting.PdvId;
                    navsFinalization.SaleRequestId = saleRequest.SaleRequestId;
                    saleRequestFinalizationsDao.Add(navsFinalization);
                                        
                    XML += $"</CONSOLE>";                    
                    XML += $"<GET TYPE=HIDDEN NAME=_TOTALCARD VALUE={_PAYMONEYCARD}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_TOTALTERMINALSERIAL VALUE={_PAYMONEYSERIAL}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsTotalize/GetTotal HOST=h timeout=10>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }
                else
                {
                    decimal payValueMoney = Convert.ToDecimal(_PAYVALUEMONEY);
                    payValueMoney = payValueMoney / 100;                    
                    XML += $"<BR><BR><BR> Registrando valor Parcial: {payValueMoney.ToString("0.00")}";
                    navsFinalization.FinalizationId = 1;
                    navsFinalization.PersonalizedCode = _PAYMONEYCARD;
                    navsFinalization.Value = payValueMoney;
                    decimal valueAmountPaid = navsFinalizationsDao.PaidAmountValue(_PAYMONEYCARD, modelSetting);
                    navsFinalization.PayBackValue = ((payValueMoney+ valueAmountPaid) - saleRequest.TotalLiquid);
                    navsFinalization.EnterpriseId = modelSetting.EnterpriseId;
                    navsFinalization.PdvId = modelSetting.PdvId;
                    navsFinalization.SaleRequestId = saleRequest.SaleRequestId;
                    saleRequestFinalizationsDao.Add(navsFinalization);                    
                    XML += $"</CONSOLE>";
                    XML += "<DELAY TIME=1> ";                    
                    XML += $"<GET TYPE=HIDDEN NAME=_TOTALCARD VALUE={_PAYMONEYCARD}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_TOTALTERMINALSERIAL VALUE={_PAYMONEYSERIAL}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsTotalize/GetTotal HOST=h timeout=10>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

            }
            catch (Exception err)
            {
                saleRequestDaoNew.MarkInUse(saleRequest.SaleRequestId, false);
                string message = Formatted.FormatError(err.Message);
                XML += $"<console>{message}</console>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }


        [HttpGet]
        public HttpResponseMessage AddTef(string _PAYTOTALTEF, string _PAYMONEYCARD, string _PAYMONEYSERIAL, string _PAYVALUETEF, string _PAYMODALITY)
        {
            try
            {
                string XML = "";
                modelSetting = settingsDao.Get(_PAYMONEYSERIAL);
                saleRequest = saleRequestDaoNew.Get(modelSetting.EnterpriseId.ToString(), _PAYMONEYCARD, false);

                if (string.IsNullOrEmpty(_PAYVALUETEF))
                {                                       
                    XML += $"<CONSOLE><BR><BR><BR> Registrando valor Total: {_PAYTOTALTEF}</CONSOLE>";                    
                    XML += $"<PAY> PAGRET=RESPAG; IPTEF={modelSetting.SitefAddressIp}; PORTATEF={modelSetting.SitefPort}; CODLOJA={modelSetting.SitefStoreCode}; TIPO={_PAYMODALITY}; VALOR={_PAYTOTALTEF}";
                    XML += $" BIN=BINCART; NINST=NAMEINST; NSU=NSUAUT; NSUSITEF=NSUSIT; NSUCANC=NSUCAN; AUT=CODAUT; NPAR=NPARCEL;";                    
                    XML += $" MODPAG=TIPOTRANS; TIPOCAR=TIPOCART; REDEADQ=CODADQ; </PAY>";

                    XML += $"<GET TYPE=HIDDEN NAME=_PAYSITEFPRINTCARD VALUE={_PAYMONEYCARD}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYSITEFPRINTTERMINALSERIAL VALUE={_PAYMONEYSERIAL}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYSITEFPRINTPAYVALUE VALUE={_PAYTOTALTEF}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_SITEFMODALITY VALUE={_PAYMODALITY}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssitef/PrintSitef HOST=h timeout=10>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }
                else
                {                    
                    var totalPay = saleRequestFinalizationsDao.PaidAmountValue(_PAYMONEYCARD, modelSetting);                    
                    decimal payValueTef = Convert.ToDecimal(_PAYVALUETEF);
                    payValueTef = payValueTef / 100;
                    if(payValueTef > (saleRequest.TotalLiquid- totalPay))
                    {
                        XML += $"<console><BR><BR><BR> Troco impossivel para esta<BR> forma de pagamento.</CONSOLE>";
                        XML += $"<DELAY TIME=2>";                        
                        XML += $"<GET TYPE=HIDDEN NAME=_TOTALCARD VALUE={_PAYMONEYCARD}>";
                        XML += $"<GET TYPE=HIDDEN NAME=_TOTALTERMINALSERIAL VALUE={_PAYMONEYSERIAL}>";
                        XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsTotalize/GetTotal HOST=h timeout=10>";
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                        };
                    }
                    
                    XML += $"<console><BR><BR><BR> Registrando valor Parcial: {_PAYVALUETEF}</CONSOLE>";
                    
                    XML += $"<PAY> PAGRET=RESPAG; IPTEF={modelSetting.SitefAddressIp}; PORTATEF={modelSetting.SitefPort}; CODLOJA={modelSetting.SitefStoreCode}; TIPO={_PAYMODALITY}; VALOR={payValueTef.ToString("0.00")}";
                    XML += $" BIN=BINCART; NINST=NAMEINST; NSU=NSUAUT; NSUSITEF=NSUSIT; NSUCANC=NSUCAN; AUT=CODAUT; NPAR=NPARCEL;";                    
                    XML += $" MODPAG=TIPOTRANS; TIPOCAR=TIPOCART; REDEADQ=CODADQ; </PAY>";

                    XML += $"<GET TYPE=HIDDEN NAME=_PAYSITEFPRINTCARD VALUE={_PAYMONEYCARD}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYSITEFPRINTTERMINALSERIAL VALUE={_PAYMONEYSERIAL}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_PAYSITEFPRINTPAYVALUE VALUE={payValueTef.ToString("0.00")}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_SITEFMODALITY VALUE={_PAYMODALITY}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssitef/PrintSitef HOST=h timeout=10>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }
            }
            catch(Exception err)
            {
                saleRequestDaoNew.MarkInUse(saleRequest.SaleRequestId, false);
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
