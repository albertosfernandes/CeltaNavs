using CeltaNavs.Domain;
using CeltaNavs.Domain.SaleRequest;
using CeltaNavs.Repository;
using CeltaNavsApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;

namespace CeltaNavsApi.Controllers
{
    public class NavsSaleRequestController : ApiController
    {
        private string navsIp;
        private string navsPort;
        private string characters;
        private HttpClient _httpClient = null;

        ModelSaleRequest saleRequest = new ModelSaleRequest();
        ModelNavsSetting modelSetting = new ModelNavsSetting();
        
        SaleRequestDao saleRequestDaoNew = new SaleRequestDao();
        private SaleRequestDao saleRequestsDao = new SaleRequestDao();
        SaleRequestProductDao saleRequestProductsDao = new SaleRequestProductDao();
        NavsSettingDao settingsDao = new NavsSettingDao();
        ProductDao productsDao = new ProductDao();

        public NavsSaleRequestController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
            characters = WebConfigurationManager.AppSettings.Get("");

            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.BaseAddress = new Uri($"http://{navsIp}:{navsPort}");
        }

        /*Chamo a partir do NavsCommands caso o cliente digite o numero do pedido direto verifico se existe*/
        [HttpGet]
        public HttpResponseMessage Get(string _TABLE, string _TSERIAL)
        {
            string XML = "";
            try
            {
                modelSetting = settingsDao.GetById(_TSERIAL);

                var mySaleRequest = saleRequestsDao.GetId(modelSetting.EnterpriseId.ToString(), _TABLE);
                

                //novo pedido
                if (mySaleRequest == null)
                {
                    ModelSaleRequest saleRequest = new ModelSaleRequest();
                    saleRequest.PersonalizedCode = _TABLE;
                    saleRequest.DateOfCreation = DateTime.Now;
                    saleRequest.DateHourOfCreation = DateTime.Now;
                    saleRequest.EnterpriseId = modelSetting.EnterpriseId;
                    saleRequest.IsUsing = false;
                    saleRequest.Peoples = 1;
                    saleRequest.FlagStatus = "ABERTO";
                    saleRequest.FlagOrigin = SaleRequestOrigin.Concentrator;                    

                    HttpResponseMessage response = null;
                    var content = new ObjectContent<ModelSaleRequest>(saleRequest, new JsonMediaTypeFormatter());

                    response = _httpClient.PostAsync("api/APISaleRequest/AddSaleRequest", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        XML += $"<CONSOLE>Novo pedido adicionado com sucesso<BR>";
                        XML += $"----------------------------------------<BR></CONSOLE>";

                        XML += $"<GET TYPE=HIDDEN NAME=_TABLE VALUE={saleRequest.PersonalizedCode}>";
                        XML += $"<GET TYPE=HIDDEN NAME=_TSERIAL VALUE={modelSetting.PosSerial}>";
                        XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssalerequest/get HOST=h TIMEOUT=5>";
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                        };

                    }
                    else
                    {
                        XML += $"<CONSOLE>Falha ao adicionar novo pedido<BR>";
                        XML += $"{response.Content.ToString()}";
                        XML += " Pressione X para tentar novamente.";
                        XML += $"----------------------------------------<BR></CONSOLE>";
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                        };
                    }


                }
                else
                {
                    if (mySaleRequest.IsUsing)
                    {
                        XML += $"<CONSOLE>Mesa/Comanda bloqueada ou uso.<BR>";
                        XML += $"----------------------------------------<BR></CONSOLE>";
                        XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={modelSetting.PosSerial}>";
                        XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsCommands/Start HOST=h TIMEOUT=5>";
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                        };
                    }

                    var saleRequesProducts = saleRequestsDao.GetSaleRequestProducts(modelSetting.EnterpriseId.ToString(), mySaleRequest.SaleRequestId.ToString(), true);

                    XML += $"<CONSOLE>Comanda/Mesa:  {_TABLE}<BR>";
                    XML += "----------------------------------------<BR>";
                    
                    XML += Menu.SaleRequestItens(saleRequesProducts);
                    XML += "----------------------------------------<BR>";

                    string totalItens = Menu.SaleOrderTotalQuantity(saleRequesProducts);

                    XML += $"Total itens: {totalItens}<BR>";
                    //decimal value = SaleRequestDao.TotalLiquid(listItensSaleReq);
                    decimal value = mySaleRequest.TotalLiquid;
                    XML += $"Total da comanda: {value.ToString("C")}";
                    XML += "</CONSOLE>";
                    //Math.Round(2.123455909, 2);

                    //XML += $"<WRITE_AT LINE=26 COLUMN=1>1: Consultar produtos.</WRITE_AT>";
                    //XML += $"<WRITE_AT LINE=27 COLUMN=1>2: Cancelar item.</WRITE_AT>";
                    XML += $"<WRITE_AT LINE=28 COLUMN=1>0: Solicitar conta.</WRITE_AT>";
                    XML += $"<WRITE_AT LINE=29 COLUMN=1>_________________Codigo_produto_>_____</WRITE_AT>";

                    XML += "<GET TYPE=FIELD NAME=_CODPROD LIN=29 COL=36 SIZE=3>";

                    XML += $"<GET TYPE=HIDDEN NAME=_PRODUCTTABLE VALUE={_TABLE}>";
                    XML += $"<GET TYPE=SERIALNO NAME=_TERMINALSERIAL>";

                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsproducts/get HOST=h TIMEOUT=5>";


                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }


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
        public HttpResponseMessage AddProduct(string _QUANT, string _PRODUCT, string _PERSONALIZEDSALECODE, string _POSSERIAL)
        {
            try
            {
                string XML = "";                
                modelSetting = settingsDao.GetById(_POSSERIAL);
                ModelProduct myproduct = new ModelProduct();
                if (_PRODUCT.Contains("-"))
                {
                    myproduct = productsDao.FindByPlu(_PRODUCT, modelSetting);
                }
                else
                {
                    myproduct = productsDao.FindByInternalCode(_PRODUCT, modelSetting);
                }

                ModelSaleRequest saleRequest = saleRequestDaoNew.Get(modelSetting.EnterpriseId.ToString(), _PERSONALIZEDSALECODE, true);

                ModelSaleRequestProduct saleRequestProducts = new ModelSaleRequestProduct();
                saleRequestProducts.SaleRequestId = saleRequest.SaleRequestId;
                saleRequestProducts.ProductPriceLookUpCode = myproduct.PriceLookupCode;
                saleRequestProducts.ProductInternalCodeOnErp = myproduct.InternalCodeOnERP;
                saleRequestProducts.Value = Convert.ToDecimal(myproduct.SaleRetailPraticedString);
                int quant = Convert.ToInt32(_QUANT);
                if (quant < 1)
                {
                    quant = 1;
                }
                saleRequestProducts.Quantity = quant;
                saleRequestProducts.TotalLiquid = (saleRequestProducts.Value * saleRequestProducts.Quantity);
                saleRequestProducts.IsCancelled = false;
                saleRequestProducts.IsDelivered = false;
                
                saleRequest.Products.Add(saleRequestProducts);                
                saleRequest.TotalLiquid += saleRequestProducts.TotalLiquid;

                // Teste OK somente SaleReque sem prods saleRequestDaoNew.UpdateTeste(saleRequest.SaleRequestId, saleRequest.TotalLiquid);
                saleRequestDaoNew.Update(saleRequest);

                XML += $"<CONSOLE>Produto adicionado com sucesso<BR>";
                XML += $"----------------------------------------<BR></CONSOLE>";
                XML += $"<GET TYPE=HIDDEN NAME=_TABLE VALUE={_PERSONALIZEDSALECODE}>";
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

        [HttpGet]
        public HttpResponseMessage CancelProduct(string _TABLE, string _POSSERIAL)
        {
            try
            {
                string XML = "";

                modelSetting = settingsDao.Get(_POSSERIAL);
                var mySaleRequestId = saleRequestsDao.GetById(modelSetting.EnterpriseId.ToString(), _TABLE, false);
                //var listItensSaleReq = saleRequestDao.GetItensSaleRequest(modelSettings, _TABLE);
                var listOfItensSaleReq = saleRequestDaoNew.GetSaleRequestProducts(modelSetting.EnterpriseId.ToString(), mySaleRequestId.ToString(), false);

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

                saleRequest = saleRequestDaoNew.Get(modelSetting.EnterpriseId.ToString(), _CARDREMOVE, false);
                var listSaleRequestProducts = saleRequestDaoNew.GetSaleRequestProducts(modelSetting.EnterpriseId.ToString(), saleRequest.SaleRequestId.ToString(), false);                
                saleRequestDaoNew.Update(saleRequest);

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
