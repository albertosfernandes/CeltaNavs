using CeltaNavs.Domain;
using CeltaNavs.Repository;
using CeltaNavsApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using System.Xml;

namespace CeltaNavsApi.Controllers
{
    public class NavsSaleMovementController : ApiController
    {
        private string navsIp;
        private string navsPort;
        NavsSettingDao navsSettingsDao = new NavsSettingDao();
        SaleRequestDao saleRequestDao = new SaleRequestDao();
        SaleRequestProductDao saleRequestProdDao = new SaleRequestProductDao();
        SaleMovementFinalizationDao saleMovementFinDao = new SaleMovementFinalizationDao();

        ModelNavsSetting modelSetting = new ModelNavsSetting();
        ModelSaleRequest saleRequest = new ModelSaleRequest();

        public NavsSaleMovementController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }

        [HttpGet]
        public HttpResponseMessage AddSaleMovement(string _SMSERIAL, string _xmlSatkey, string _saleReqJson, string _personcode, string _saleMovJson)
        {
            string XML = "";
            try
            {
                modelSetting = navsSettingsDao.Get(_SMSERIAL);

                saleRequest = saleRequestDao.Get(modelSetting.EnterpriseId.ToString(), _personcode, false);
                //saleRequest = JsonConvert.DeserializeObject<ModelSaleRequest>(_saleReqJson);

                //ModelSaleMovement saleMovement = JsonConvert.DeserializeObject<ModelSaleMovement>(_saleMovJson);
                ModelSaleMovement saleMovement = new ModelSaleMovement();
                saleMovement.PersonalizedCode = saleRequest.PersonalizedCode;
                saleMovement.Enterprises = modelSetting.Enterprises;
                saleMovement.Pdvs = modelSetting.Pdvs;
                saleMovement.CPFCNPJ = "31970441852";
                saleMovement.TotalLiquid = saleRequest.TotalLiquid;

                List<ModelSaleMovementFinalization> listOfSaleMovFinalization = saleMovementFinDao.GetAll(_personcode, modelSetting);
                //List<ModelSaleRequestProduct> listOfProducts = saleRequestProdDao.n


                //int result = SaleMovementDao.CreateSaleMovementBS(_xmlResultSat, saleMovement, saleRequest.Products, listOfSaleMovFinalization, modelSetting);
                //if (result == 0)
                //{
                //    XML = $"<console><BR><BR>Informacoes gravadas com sucesso.<BR>";
                //    XML += "----------------------------------------<BR></console>";
                //    string codeBarValue = CeltaNavsApi.Helpers.NavsSaleHelpers.GetFiscalNoteAccessKey(_xmlResultSat);
                //    XmlDocument doc = new XmlDocument();
                //    doc.LoadXml(_xmlResultSat);
                //    XmlNodeList elemList = doc.GetElementsByTagName("QRCode");
                //    string qrCodeValue = "";
                //    for (int i = 0; i < elemList.Count; i++)
                //    {
                //        qrCodeValue = (elemList[i].InnerXml);
                //    }

                //    int CouponNumber = Convert.ToInt32(codeBarValue.Substring(34, 6));
                //    XML += "<console><BR> Imprimindo a comanda ... <br><BR></console>";
                //    XML += "<delay time=1>";
                //    XML += Printer.Header(saleMovement, CouponNumber);
                //    XML += Printer.Print(_personcode, saleRequest.Products, modelSetting, true);
                //    XML += Printer.PrintPayments(listOfSaleMovFinalization);
                //    XML += Printer.PrintCodeBar(codeBarValue);
                //    XML += Printer.PrintQrCode(qrCodeValue);

                //    XML += "<console><BR> Apagando registros temporarios ... <br><BR></console>";
                //    saleMovementFinDao.RemovePayments(saleMovement.PersonalizedCode);
                //    saleRequestDao.Delete(saleRequest.SaleRequestId);

                //    if (!modelSetting.SaveXMLSat == true)
                //    {
                //        //remover arquivo xml SAT
                //    }

                //    XML += "<console>Finlizado com sucesso.</console>";
                //    XML += "<delay time=1>";
                //    XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_SMSERIAL}>";
                //    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/start HOST=h timeout=10>";
                //    return new HttpResponseMessage(HttpStatusCode.OK)
                //    {
                //        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                //    };
                //}
                //else
                //{
                //    saleRequestDao.MarkInUse(saleRequest.SaleRequestId, false);
                //    XML += "<console>Ocorreu erro na gravaçao do movimento.</console>";
                //    XML += "<get type=anykey>";
                //    XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_SMSERIAL}>";
                //    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/start HOST=h timeout=10>";
                //    return new HttpResponseMessage(HttpStatusCode.OK)
                //    {
                //        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                //    };
                //}   

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch (Exception err)
            {
                saleRequestDao.MarkInUse(saleRequest.SaleRequestId, false);
                string message = Formatted.FormatError(err.Message);
                XML += $"<console><BR><BR>Erro na gravacao com banco. <BR>";
                XML += "----------------------------------------<BR>";
                XML += $"Erro: {message}<BR><BR></console>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }
    }
}
