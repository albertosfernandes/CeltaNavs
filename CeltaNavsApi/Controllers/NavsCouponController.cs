using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;

using CeltaNavsApi.Helpers;
using System.Xml;
using CeltaNavs.Domain;
using CeltaNavs.Repository;
using Newtonsoft.Json;

namespace CeltaNavsApi.Controllers
{
    public class NavsCouponController : ApiController
    {
        private string navsIp;
        private string navsPort;

        NavsSettingDao navsSettingsDao = new NavsSettingDao();
        SaleMovementFinalizationDao saleMovementFinalizations = new SaleMovementFinalizationDao();
        SaleRequestDao saleRequestDaoNew = new SaleRequestDao();
        SaleMovementDao saleMovementDao = new SaleMovementDao();

        ModelNavsSetting modelSetting = new ModelNavsSetting();
        ModelSaleMovement saleMovement = new ModelSaleMovement();
        ModelSaleRequest saleRequest = new ModelSaleRequest();

        public NavsCouponController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }

        [HttpGet]
        public HttpResponseMessage Teste()
        {
            string XML = "";
            try
            {
                saleMovement.PersonalizedCode = "6";
                saleRequestDaoNew.RemoveSaleRequest(saleMovement.PersonalizedCode);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch (Exception err)
            {
                XML += $"Erro: {err.Message}<BR><BR></console>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }

        [HttpGet]
        public HttpResponseMessage isNotaPaulista(string _COUPONCARD, string _COUPONTERMINALSERIAL)
        {
            string XML = "";
            try
            {
                XML = $"<console> Nota Paulista: <BR>";
                XML += "----------------------------------------<BR></console>";
                XML += "<CANCEL_KEY TYPE=DISABLE>";

                XML += "<RECTANGLE NAME=RETCARD X=50 Y=150 WIDTH=200 HEIGHT=100 VISIBLE=1 COLOR=ccc> ";
                XML += $"<WRITE_AT LINE=13 COLUMN=13>CPF / CNPJ</WRITE_AT>";

                XML += $"<WRITE_AT LINE=29 COLUMN=1>__________________________________>_____</WRITE_AT>";

                XML += "<GET TYPE=CPF NAME=_statusCPF COL=8 LIN=15>";
                XML += $"<GET TYPE=HIDDEN NAME=_statusFinCard VALUE={_COUPONCARD}>";
                XML += $"<GET TYPE=HIDDEN NAME=_statusFinSerial VALUE={_COUPONTERMINALSERIAL}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscoupon/SatStatus HOST=h>";

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
        public HttpResponseMessage Finish(string _CPF, string _FINISHCARD, string _FINISHTERMINALSERIAL)
        {
            string XML = "";

            try
            {
                modelSetting = navsSettingsDao.Get(_FINISHTERMINALSERIAL);

                saleRequest = saleRequestDaoNew.Get(modelSetting.EnterpriseId.ToString(), _FINISHCARD, false);

                ModelSaleMovement saleMovement = new ModelSaleMovement();
                saleMovement.PersonalizedCode = saleRequest.PersonalizedCode;
                saleMovement.Enterprises = modelSetting.Enterprises;
                saleMovement.Pdvs = modelSetting.Pdvs;
                saleMovement.CPFCNPJ = _CPF;
                saleMovement.TotalLiquid = saleRequest.TotalLiquid;

                List<ModelSaleMovementFinalization> listOfFinalizationsSaleMov = saleMovementFinalizations.GetAll(saleMovement.PersonalizedCode, modelSetting);

                //Validar se é nova venda (novo cupom) ou atualizar
                string xmlToSat = SaleMovementXML.GetXmlToSATNFCEFromSaleMovement(saleMovement, saleRequest.Products, listOfFinalizationsSaleMov, true, modelSetting);
                if (modelSetting.SaveXMLSat)
                    SaveFiles.SaveXmlSat(xmlToSat);


                string xmlFromSat = NavsSatHelpers.SendSaleMovement(xmlToSat, "OK", modelSetting);
                if (modelSetting.SaveXMLSat)
                    SaveFiles.SaveXmlResponseSat(xmlFromSat);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlFromSat);

                XmlNodeList elementResponse = doc.GetElementsByTagName("returnCode");
                string resp = "";
                for (int i = 0; i < elementResponse.Count; i++)
                {
                    resp = (elementResponse[i].InnerXml);
                }
                if (resp == "06000")
                {
                    XmlNodeList elemList = doc.GetElementsByTagName("QRCode");
                    string qrCode = "";
                    for (int i = 0; i < elemList.Count; i++)
                    {
                        qrCode = (elemList[i].InnerXml);
                    }

                    #region Gravar sale movement
                    //iniciar gravação sat na tabela SaleMovement

                    int result = SaleMovementDao.CreateSaleMovementBS(xmlFromSat, saleMovement, saleRequest.Products, listOfFinalizationsSaleMov, modelSetting);                  


                    if (result == 0)
                    {
                        string codeBarValue = CeltaNavsApi.Helpers.NavsSaleHelpers.GetFiscalNoteAccessKey(xmlFromSat);
                        int CouponNumber = Convert.ToInt32(codeBarValue.Substring(34, 6));
                        XML += "<console><BR> Imprimindo a comanda ... <br><BR></console>";
                        XML += "<delay time=1>";
                        XML += Printer.Header(saleMovement, CouponNumber);
                        XML += Printer.Print(_FINISHCARD, saleRequest.Products, saleRequest, modelSetting, true);
                        XML += Printer.PrintPayments(listOfFinalizationsSaleMov);
                        XML += Printer.PrintCodeBar(codeBarValue);
                        XML += Printer.PrintQrCode(qrCode);

                        XML += "<console><BR> Apagando registros temporarios ... <br><BR></console>";
                        saleMovementFinalizations.RemovePayments(saleMovement.PersonalizedCode);
                        saleRequestDaoNew.Delete(saleRequest.SaleRequestId);

                        XML += "<console>Finlizado com sucesso.</console>";
                        XML += "<delay time=1>";
                        XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_FINISHTERMINALSERIAL}>";
                        XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/start HOST=h timeout=10>";
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                        };
                    }
                    else
                    {
                        saleRequestDaoNew.MarkInUse(saleRequest.SaleRequestId, false);
                        XML += "<console>Ocorreu erro na gravaçao do movimento.</console>";
                        XML += "<get type=anykey>";
                        XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_FINISHTERMINALSERIAL}>";
                        XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/start HOST=h timeout=10>";
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                        };
                    }
                    #endregion                 
                }
                else
                {

                    saleRequestDaoNew.MarkInUse(saleRequest.SaleRequestId, false);
                    XML += "<console>Ocorreu erro na geracao fiscal.</console>";
                    XML += "<get type=anykey>";
                    XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_FINISHTERMINALSERIAL}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/start HOST=h timeout=10>";
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
                XML += $"<console><BR><BR>Erro na Geracao fiscal. <BR>";
                XML += "----------------------------------------<BR>";
                XML += $"Erro: {message}<BR><BR></console>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }

        [HttpGet]
        public HttpResponseMessage SatStatus(string _statusCPF, string _statusFinCard, string _statusFinSerial)
        {
            string XML = "";
            try
            {
                modelSetting = navsSettingsDao.Get(_statusFinSerial);
                saleRequest = saleRequestDaoNew.Get(modelSetting.EnterpriseId.ToString(), _statusFinCard, false);
                XML += $"<console>Consultado status do SAT . . . <BR>";
                XML += "----------------------------------------<BR>";
                XML += $"Cpf: {_statusCPF}<BR>";
                XML += $"Pedido: {_statusFinCard}<BR>";
                XML += $"Terminal: {_statusFinSerial}<BR></console>";
                if (!NavsSatHelpers.ConsultSATOperacionalStatus(modelSetting.SatAddressSharePdv, modelSetting.SatPortSharePdv))
                {
                    saleRequestDaoNew.MarkInUse(saleRequest.SaleRequestId, false);
                    XML = $"<console><BR><BR>SAT OFF-LINE <BR>";
                    XML += "----------------------------------------<BR></console>";
                    XML += "<delay time=1>";
                    XML += $"<GET TYPE=HIDDEN NAME=_SERIALPOS VALUE={modelSetting.PosSerial}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsCoupon/errorSat HOST=h timeout=10>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }
                else
                {
                    XML = $"<console>Resposta de SAT: Sucesso<BR>";
                    XML += "----------------------------------------<BR>";
                    XML += "Gerando cupom fiscal . . . <BR>";
                    XML += $"CPF: {_statusCPF}<BR>";
                    XML += $"PEDIDO: {_statusFinCard}<BR></console>";

                    XML += $"<GET TYPE=HIDDEN NAME=_CPF VALUE={_statusCPF}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_FINISHCARD VALUE={_statusFinCard}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_FINISHTERMINALSERIAL VALUE={_statusFinSerial}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsCoupon/Finish HOST=h timeout=10>";
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
                XML += $"<console><BR><BR>SAT OFF-LINE <BR>";
                XML += "----------------------------------------<BR>";
                XML += $"Erro: {message}<BR><BR></console>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }

        [HttpGet]
        public HttpResponseMessage ErrorSat(string _SERIALPOS)
        {
            string XML = "";
            try
            {
                modelSetting = navsSettingsDao.Get(_SERIALPOS);
                XML = $"<console> SAT OFF-LINE: <BR>";
                XML += "----------------------------------------<BR>";
                XML += $"Revise a configuracao na empresa: {modelSetting.Enterprises.EnterpriseCode}<BR>";
                XML += $"IP SAT: {modelSetting.SatAddressSharePdv}<BR>";
                XML += $"Porta SAT: {modelSetting.SatPortSharePdv}<BR>";
                XML += $"Pressione uma tecla para continuar.";
                XML += "</console>";
                XML += "<get type=anykey>";
                XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_SERIALPOS}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/start HOST=h timeout=10>";
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
