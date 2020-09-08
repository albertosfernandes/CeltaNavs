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
    public class NavsPeoplesController : ApiController
    {
        private string navsIp;
        private string navsPort;

        
        ModelNavsSetting modelSetting = new ModelNavsSetting();
        ModelSaleRequest saleRequest = new ModelSaleRequest();

        NavsSettingDao navsSettingsDao = new NavsSettingDao();
        SaleRequestDao saleRequestDao = new SaleRequestDao();



        public NavsPeoplesController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }

        [HttpGet]
        public HttpResponseMessage Get(string _PEOPLETERMINALSERIAL, string _CARDPEOPLE)
        {
            string XML = $"";
            try
            {
                modelSetting = navsSettingsDao.Get(_PEOPLETERMINALSERIAL);
                XML += "<console> <BR> </console>";
                XML += "<RECTANGLE NAME=RETCARD X=53 Y=200 WIDTH=150 HEIGHT=28 VISIBLE=1 COLOR=ccc> ";
                XML += $"<WRITE_AT LINE=12 COLUMN=8>Informe a quantidade de pessoas</WRITE_AT>";
                //XML += $"<WRITE_AT LINE=29 COLUMN=1>__________________________________>_____</WRITE_AT>";
                XML += "<GET TYPE=FIELD NAME=QUANT LIN=14 COL=7 SIZE=2>";                
                XML += $"<GET TYPE=HIDDEN NAME=_SAVETERMINALSERIAL VALUE={_PEOPLETERMINALSERIAL}>";
                XML += $"<GET TYPE=HIDDEN NAME=_SAVECARD VALUE={_CARDPEOPLE}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navspeoples/print HOST=h TIMEOUT=5>";

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

        [HttpGet]
        public HttpResponseMessage Print(string QUANT, string _SAVETERMINALSERIAL, string _SAVECARD)
        {
            string XML = $"";
            try
            {
                modelSetting = navsSettingsDao.Get(_SAVETERMINALSERIAL);
                saleRequest = saleRequestDao.Get(modelSetting.EnterpriseId.ToString(), _SAVECARD, false);                

                if (String.IsNullOrEmpty(QUANT))
                {                    
                    XML += Printer.Print(_SAVECARD, saleRequest.Products, saleRequest, modelSetting, false);
                }
                else
                {
                    saleRequest.Peoples = Convert.ToInt32(QUANT);
                    saleRequestDao.Update(saleRequest);
                    XML += Printer.Print(_SAVECARD, saleRequest.Products, saleRequest, modelSetting, false);                    
                }

                XML += $"<GET TYPE=HIDDEN NAME=_TOTALCARD VALUE={_SAVECARD}>";
                XML += $"<GET TYPE=HIDDEN NAME=_TOTALTERMINALSERIAL VALUE={_SAVETERMINALSERIAL}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navstotalize/gettotal HOST=h timeout=10>";
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

        //[HttpGet]
        //public HttpResponseMessage Save(string QUANT, string _SAVETERMINALSERIAL, string _SAVECARD)
        //{
        //    string XML = $"";
        //    try
        //    {
        //        modelSetting = navsSettingsDao.Get(_SAVETERMINALSERIAL);

        //        if (String.IsNullOrEmpty(QUANT))
        //        {
        //            // string _OPTPAY, string _CARDPAY, string _TOTALPAY, string _SERIALPAY - GetOptionPay - navspay
        //            var saleRequestId = saleRequestDao.GetById(modelSetting.EnterpriseId.ToString(), _SAVECARD, false);
        //            var listItensSaleReq = saleRequestDao.GetSaleRequestProducts(modelSetting.EnterpriseId.ToString(), saleRequestId.ToString(), false);

        //            XML += Printer.Print(_SAVECARD, listItensSaleReq, saleRequest, modelSetting, false);
        //        }
        //        else
        //        {
        //            XML += "<RECTANGLE NAME=RETCARD X=53 Y=200 WIDTH=150 HEIGHT=28 VISIBLE=1 COLOR=ccc> ";
        //            XML += $"<WRITE_AT LINE=12 COLUMN=8>Informe a quantidade de pessoas</WRITE_AT>";
        //            //XML += $"<WRITE_AT LINE=29 COLUMN=1>__________________________________>_____</WRITE_AT>";
        //            XML += "<GET TYPE=FIELD NAME=OPC LIN=14 COL=7 SIZE=2>";

        //            XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsproducts/getwithgroup HOST=h TIMEOUT=5>";
        //        }

        //        XML += $"<GET TYPE=HIDDEN NAME=_TOTALCARD VALUE={_SAVECARD}>";
        //        XML += $"<GET TYPE=HIDDEN NAME=_TOTALTERMINALSERIAL VALUE={_SAVETERMINALSERIAL}>";
        //        XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navstotalize/gettotal HOST=h timeout=10>";
        //        return new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StringContent(XML, Encoding.UTF8, "application/xml")
        //        };


        //    }
        //    catch (Exception err)
        //    {
        //        string message = Formatted.FormatError(err.Message);
        //        XML = $"<console>{message}</console>";

        //        return new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StringContent(XML, Encoding.UTF8, "application/xml")
        //        };
        //    }
        //}
    }
}
