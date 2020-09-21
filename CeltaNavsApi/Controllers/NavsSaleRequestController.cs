using CeltaNavs.Domain;
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
    public class NavsSaleRequestController : BaseController
    {
        //private string navsIp;
        //private string navsPort;
        //private string characters;
        //private HttpClient _httpClient = null;

        ModelSaleRequest saleRequest = new ModelSaleRequest();
        ModelSaleRequestTemp saleRequestTemp = new ModelSaleRequestTemp();
        //ModelNavsSetting modelSetting = new ModelNavsSetting();
        ModelSaleRequestProduct saleRequestProducts = new ModelSaleRequestProduct();        
        ModelProduct product = new ModelProduct();
        //SaleRequestDao saleRequestDaoNew = new SaleRequestDao();
        private SaleRequestTempDao saleRequestTempDao = new SaleRequestTempDao();        
        private SaleRequestDao saleRequestsDao = new SaleRequestDao();
        private SaleRequestProductDao saleRequestProductsDao = new SaleRequestProductDao();        
        //private NavsSettingDao settingsDao = new NavsSettingDao();
        private ProductDao productsDao = new ProductDao();

        public NavsSaleRequestController()
        {            
            //navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            //navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
            //characters = WebConfigurationManager.AppSettings.Get("");

            //_httpClient = new HttpClient();
            //_httpClient.Timeout = new TimeSpan(0, 0, 30);
            //_httpClient.BaseAddress = new Uri($"http://{navsIp}:{navsPort}");
        }

        /*Chamo a partir do NavsCommands caso o cliente digite o numero do pedido direto verifico se existe*/
        [HttpGet]
        public HttpResponseMessage Get(string _TABLE, string _TSERIAL)
        {
            string XML = "";
            try
            {
                modelSetting = settingsdao.GetById(_TSERIAL);                
                var resultSaleRequest = saleRequestsDao.Get(modelSetting.EnterpriseId.ToString(), _TABLE, false);
                var resultsaleRequestTemp = saleRequestTempDao.Get(modelSetting.EnterpriseId.ToString(), _TABLE, false);

                //novo pedido
                if (resultSaleRequest == null && resultsaleRequestTemp == null)
                {

                    saleRequestTemp.PersonalizedCode = _TABLE;
                    saleRequestTemp.EnterpriseId = modelSetting.EnterpriseId;

                    HttpResponseMessage response = null;
                    var content = new ObjectContent<ModelSaleRequestTemp>(saleRequestTemp, new JsonMediaTypeFormatter());

                    response = _httpClient.PostAsync("api/APISaleRequest/AddSaleRequestTemp", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        XML += $"<CONSOLE>Novo pedido adicionado com sucesso<BR>";
                        XML += $"----------------------------------------<BR></CONSOLE>";

                        XML += $"<GET TYPE=HIDDEN NAME=_TABLE VALUE={saleRequestTemp.PersonalizedCode}>";
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
                //pedido existe mas vai atualizar!
                else if (resultSaleRequest != null && resultsaleRequestTemp == null)
                {
                    if (resultSaleRequest.IsUsing)
                    {
                        XML += $"<CONSOLE>Mesa/Comanda bloqueada.<BR>";
                        XML += $"Procure o fiscal de caixa.<BR>";
                        XML += $"----------------------------------------<BR></CONSOLE>";
                        XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={modelSetting.PosSerial}>";
                        XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsCommands/Start HOST=h TIMEOUT=5>";
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                        };
                    }

                    //var saleRequesProducts = saleRequestsDao.GetSaleRequestProducts(modelSetting.EnterpriseId.ToString(), mySaleRequest.SaleRequestId.ToString(), true);
                    //XML += "<CANCEL_KEY TYPE=DISABLE>";
                    XML += $"<CONSOLE>Comanda/Mesa:  {_TABLE}<BR>";
                    XML += "----------------------------------------<BR>";

                    XML += Menu.SaleRequestItens(resultSaleRequest.Products.ToList<ModelSaleRequestProduct>());
                    XML += "----------------------------------------<BR>";

                    string totalItens = Menu.SaleOrderTotalQuantity(resultSaleRequest.Products.ToList<ModelSaleRequestProduct>());

                    XML += $"Total itens: {totalItens}<BR>";
                    decimal value = resultSaleRequest.TotalLiquid;
                    XML += $"Total da comanda: {value.ToString("C")}";
                    XML += "</CONSOLE>";

                    if(resultsaleRequestTemp != null)
                    {
                        if (resultsaleRequestTemp.Products?.Any() == true)
                        {
                            XML += $"<CONSOLE><BR>----------------------------------------<BR>";
                            XML += "Novos itens:  {_TABLE}<BR>";
                            XML += "----------------------------------------<BR>";
                            XML += Menu.SaleRequestItensTemp(resultsaleRequestTemp.Products);
                            XML += "----------------------------------------<BR>";
                            XML += "</CONSOLE>";
                        }
                    }
                    

                    XML += $"<WRITE_AT LINE=28 COLUMN=1>0: Fecha pedido.</WRITE_AT>";
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
                //pedido nao salvo em SAleRequest, ainda esta na tabela temp
                else
                {
                    string totalItens = string.Empty;
                    XML += "<CANCEL_KEY TYPE=DISABLE>";

                    XML += $"<CONSOLE>Comanda/Mesa:  {_TABLE}<BR>";
                    XML += "----------------------------------------<BR>";

                    if(resultSaleRequest != null)
                    {
                        if(resultSaleRequest.Products?.Any() == true)
                        {
                            XML += Menu.SaleRequestItens(resultSaleRequest.Products.ToList<ModelSaleRequestProduct>());
                            XML += "----------------------------------------<BR>";

                            totalItens = Menu.SaleOrderTotalQuantity(resultSaleRequest.Products.ToList<ModelSaleRequestProduct>());

                            XML += $"Total itens: {totalItens}<BR>";
                            decimal value = resultSaleRequest.TotalLiquid;
                            XML += $"Total da comanda: {value.ToString("C")}";
                        }                        
                    }
                                     
 
                    if (resultsaleRequestTemp.Products?.Any() == true)
                    {
                        XML += $"<BR>----------------------------------------<BR>";
                        XML += $"Novos itens:  {_TABLE}<BR>";
                        XML += "----------------------------------------<BR>";
                        XML += Menu.SaleRequestItensTemp(resultsaleRequestTemp.Products);
                        XML += "----------------------------------------<BR>";                        
                    }

                    XML += "</CONSOLE>";
                    XML += $"<WRITE_AT LINE=28 COLUMN=1>0: Fecha pedido | Volta.</WRITE_AT>";
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
        public HttpResponseMessage CloseSaleRequestTemp(string _TABLE, string _TSERIAL, string _OPCAO)
        {
            string XML = "";
            try
            {
                modelSetting = settingsdao.GetById(_TSERIAL);
                
                //apenas fecha o pedido da tabela temp e grava na sale request
                if (_OPCAO == "" || String.IsNullOrEmpty(_OPCAO))
                {
                    saleRequestTemp = saleRequestTempDao.Get(modelSetting.EnterpriseId.ToString(), _TABLE, true);
                    if(saleRequestTemp == null)
                    {
                        XML += $"<CONSOLE>Nenhum item adicionado.<BR>";
                        XML += $"----------------------------------------<BR></CONSOLE>";
                        XML += " <DELAY TIME = 01>";
                        XML += $"<GET TYPE=SERIALNO NAME=_SERIALNUMBER>";
                        XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsCommands/start HOST=h TIMEOUT=5>";
                    }
                    else
                    {
                        if (!saleRequestTempDao.Finish(saleRequestTemp))
                        {
                            XML += $"<CONSOLE>Erro ao gravar pedido temporario.<BR>";
                            XML += $"----------------------------------------<BR></CONSOLE>";
                            XML += " <DELAY TIME = 01> ";
                            XML += $"<GET TYPE=HIDDEN NAME=_TABLE VALUE={_TABLE}>";
                            XML += $"<GET TYPE=SERIALNO NAME=_TSERIAL>";
                            XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssaleRequest/get HOST=h TIMEOUT=5>";
                            return new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                            };
                        }

                        XML += $"<CONSOLE>Pedido gravado com sucesso.<BR>";
                        XML += $"----------------------------------------<BR></CONSOLE>";
                        XML += " <DELAY TIME = 01>";
                        XML += $"<GET TYPE=SERIALNO NAME=_SERIALNUMBER>";
                        XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsCommands/start HOST=h TIMEOUT=5>";
                    }
                                  
                }
                //fecha o pedido, grava na SaleReQuest e vai para pagamentos
                else if (Convert.ToInt32(_OPCAO) == 0) 
                {
                    saleRequestTemp = saleRequestTempDao.Get(modelSetting.EnterpriseId.ToString(), _TABLE, true);
                    if(saleRequestTemp != null)
                    {
                        if (!saleRequestTempDao.Finish(saleRequestTemp))
                        {
                            XML += $"<CONSOLE>Erro na gravar pedido.<BR>";
                            XML += $"----------------------------------------<BR></CONSOLE>";
                            XML += " <DELAY TIME = 01> ";
                            XML += $"<GET TYPE=HIDDEN NAME=_TABLE VALUE={_TABLE}>";
                            XML += $"<GET TYPE=SERIALNO NAME=_TSERIAL>";
                            XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssaleRequest/get HOST=h TIMEOUT=5>";
                        }

                        XML += $"<CONSOLE>Pedido gravado com sucesso.<BR>";
                        XML += $"----------------------------------------<BR></CONSOLE>";
                        XML += " <DELAY TIME = 01>";
                    }
                   
                  
                    XML += $"<GET TYPE=HIDDEN NAME=_TOTALCARD VALUE={_TABLE}>";                    
                    XML += $"<GET TYPE=SERIALNO NAME=_TOTALTERMINALSERIAL>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsTotalize/GetTotal HOST=h TIMEOUT=5>";

                }
                //vai para cancelamentos
                else if (Convert.ToInt32(_OPCAO) == 1) 
                {
                    XML += $"<GET TYPE=HIDDEN NAME=_CANCELTABLE VALUE={_TABLE}>";
                    XML += $"<GET TYPE=SERIALNO NAME=_CANCELSERIAL>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscancel/get HOST=h TIMEOUT=5>";
                }
                //Opção invalida
                else
                {                 
                    XML += $"<CONSOLE>Opcao invalida<BR>";
                    XML += $"----------------------------------------<BR></CONSOLE>";
                    XML += " <DELAY TIME = 01> ";
                    XML += $"<GET TYPE=HIDDEN NAME=_TABLE VALUE={_TABLE}>";
                    XML += $"<GET TYPE=SERIALNO NAME=_TSERIAL>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssaleRequest/get HOST=h TIMEOUT=5>";
                }
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

        [HttpGet]
        public HttpResponseMessage Save(string _QUANT, string _PRODUCT, string _PERSONALIZEDSALECODE, string _POSSERIAL)
        {
            try
            {
                string XML = "";                
                modelSetting = settingsdao.GetById(_POSSERIAL);
                ModelSaleRequest saleRequest = saleRequestsDao.Get(modelSetting.EnterpriseId.ToString(), _PERSONALIZEDSALECODE, true);

                if (_PRODUCT.Contains("-"))
                {
                    product = productsDao.FindByPlu(_PRODUCT, modelSetting);
                }
                else
                {
                    product = productsDao.FindByInternalCode(_PRODUCT, modelSetting);
                }

                if (product == null)
                {
                    XML += $"<CONSOLE>Produto nao encontrado<BR>";
                    XML += $"----------------------------------------<BR></CONSOLE>";
                    XML += " <DELAY TIME = 01> ";
                    XML += $"<GET TYPE=HIDDEN NAME=_TABLE VALUE={_PERSONALIZEDSALECODE}>";
                    XML += $"<GET TYPE=SERIALNO NAME=_TSERIAL>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssaleRequest/get HOST=h TIMEOUT=5>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

                saleRequestProducts.SaleRequestId = saleRequest.SaleRequestId;
                saleRequestProducts.ProductPriceLookUpCode = product.PriceLookupCode;
                saleRequestProducts.ProductInternalCodeOnErp = product.InternalCodeOnERP;
                saleRequestProducts.Value = Convert.ToDecimal(product.SaleRetailPraticedString);
                int quant = Convert.ToInt32(_QUANT);
                if (quant < 1)
                {
                    quant = 1;
                }
                saleRequestProducts.Quantity = quant;
                saleRequestProducts.TotalLiquid = (saleRequestProducts.Value * saleRequestProducts.Quantity);
                saleRequestProducts.IsCancelled = false;
                saleRequestProducts.IsDelivered = false;
                saleRequestProducts.ProductionStatus = ProductionStatus.New;

                saleRequest.Products.Add(saleRequestProducts);                
                saleRequest.TotalLiquid += saleRequestProducts.TotalLiquid;

                saleRequestsDao.Update(saleRequest);

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
      
    }
}
