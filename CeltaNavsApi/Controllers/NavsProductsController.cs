using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using CeltaNavs.Domain;
using CeltaNavs.Repository;
using CeltaNavsApi.Helpers;

namespace CeltaNavsApi.Controllers
{
    public class NavsProductsController : BaseController
    {
        
        private ModelProduct product = new ModelProduct();      
        private ModelExpansibleGroup groups = new ModelExpansibleGroup();        
        private ExpansibleGroupDao groupDao = new ExpansibleGroupDao();
        private ProductDao productsDao = new ProductDao();
        private SaleRequestDao saleRequestsDao = new SaleRequestDao();
        private SaleRequestTempDao saleRequestTempDao = new SaleRequestTempDao();
        private SaleRequestProductTempDao saleRequestProductTempDao = new SaleRequestProductTempDao();

        public NavsProductsController()
        {
            
        }

        public HttpResponseMessage Get(string _CODPROD, string _PRODUCTTABLE, string _TERMINALSERIAL)
        {
            string XML = "";
            try
            {
                modelSetting = settingsdao.GetById(_TERMINALSERIAL);
                
                if (_CODPROD == "" || String.IsNullOrEmpty(_CODPROD))
                {

                    var lisOfGroups = groupDao.Get(modelSetting);
                    if (lisOfGroups.Count < 1)
                    {

                        // AQUI LISTAR PRODUTOS SEM GRUPOS mandar para products/getall
                        //string _CODPROD, string _PRODUCTCARD, string _PRODUCTTABLE, string _TERMINALSERIAL
                        //XML += $"<GET TYPE=HIDDEN NAME=_PRODUCTCARD VALUE={_PRODUCTCARD}>";
                        XML += $"<GET TYPE=HIDDEN NAME=_PRODUCTTABLE VALUE={_PRODUCTTABLE}>";
                        XML += $"<GET TYPE=HIDDEN NAME=_PRODUCTTERMINALSERIAL VALUE={_TERMINALSERIAL}>";

                        //GetAll não existe mais.. verificar para onde jogar!!!
                        XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsproducts/getall HOST=h TIMEOUT=5>";

                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                        };
                    }
                    
                    XML += $"<GET TYPE=HIDDEN NAME=_GROUPSTABLE VALUE={_PRODUCTTABLE}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_GROUPSTERMINALSERIAL VALUE={_TERMINALSERIAL}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsgroups/getall HOST=h TIMEOUT=5>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }            

                if (_CODPROD == "0")
                {
                    XML += "<CANCEL_KEY TYPE=DISABLE>";
                    XML += "<console>Confirmar o pedido<BR>";
                    XML += $"----------------------------------------<BR>";
                    XML += "</console>";
                    XML += $"<WRITE_AT LINE=12 COLUMN=4>Confirma o fechamento do pedido?</WRITE_AT>";
                    XML += $"<WRITE_AT LINE=26 COLUMN=2>ENTER: Confirmar.</WRITE_AT>";
                    XML += $"<WRITE_AT LINE=27 COLUMN=2>0: Solicitar Pagamentos.</WRITE_AT>";
                    XML += $"<WRITE_AT LINE=28 COLUMN=2>1: Cancelamentos.</WRITE_AT>";
                    XML += $"<WRITE_AT LINE=29 COLUMN=1>________________________________>_____</WRITE_AT>";
                    XML += "<GET TYPE=FIELD NAME=_OPCAO LIN=29 COL=36 SIZE=3>";                    
                    XML += $"<GET TYPE=HIDDEN NAME=_TOTALCARD VALUE={_PRODUCTTABLE}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_TOTALTERMINALSERIAL VALUE={_TERMINALSERIAL}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navstotalize/get HOST=h>";

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }


                string productCodeWithDigit = _CODPROD;                
                productCodeWithDigit = productCodeWithDigit.PadLeft(Convert.ToInt32(modelSetting.NumberOfCharacteresPLU), '0');
                productCodeWithDigit += "-" + productsDao.CheckDigit(_CODPROD);

                XML += $"<GET TYPE=HIDDEN NAME=_SELPROD VALUE={productCodeWithDigit}>";                
                XML += $"<GET TYPE=HIDDEN NAME=_PERSONALIZEDCODE VALUE={_PRODUCTTABLE}>";
                XML += $"<GET TYPE=SERIALNO NAME=_TERMINALSERIAL>";                
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsQuantity/get HOST=h TIMEOUT=5>";                

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
        public HttpResponseMessage GetWithGroup(string _SELGROUP, string _PRODUCTTABLE, string _PRODUCTTERMINALSERIAL)
        {
            try
            {
                string XML = "";

                modelSetting = settingsdao.GetById(_PRODUCTTERMINALSERIAL);
                XML += "<CONSOLE>Lista de produtos<BR>";
                XML += "----------------------------------------<BR>";

                XML += string.Format("{0,-3} | {1,-2}", "Codigo", "Descricao" + "<BR>");
                XML += "----------------------------------------<BR></CONSOLE>";
                var listOfProducts = productsDao.GetByGroup(modelSetting, _SELGROUP);
                XML += Menu.MenuListProducts(listOfProducts);

                XML += $"<GET TYPE=HIDDEN NAME=_PERSONALIZEDCODE VALUE={_PRODUCTTABLE}>";                
                XML += $"<GET TYPE=HIDDEN NAME=_TERMINALSERIAL VALUE={_PRODUCTTERMINALSERIAL}>";                
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsQuantity/get HOST=h TIMEOUT=5>";

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
        public HttpResponseMessage AddProduct(string _QUANT, string _PRODUCT, string _PERSONALIZEDSALECODE, string _POSSERIAL)
        {
            try
            {
                string XML = "";
                modelSetting = settingsdao.GetById(_POSSERIAL);

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

                ModelSaleRequestTemp _saleRequestTemp = saleRequestTempDao.Get(modelSetting.EnterpriseId.ToString(), _PERSONALIZEDSALECODE, true);
                var resultSaleRequest = saleRequestsDao.Get(modelSetting.EnterpriseId.ToString(), _PERSONALIZEDSALECODE, false);

                if (_saleRequestTemp == null && resultSaleRequest == null)
                {
                    XML += $"<CONSOLE>Erro ao carregar pedido temporario<BR>";                    
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
                else if (resultSaleRequest != null && _saleRequestTemp == null)
                {
                    _saleRequestTemp = new ModelSaleRequestTemp();
                    //preciso carrega-lo na tabela temp!!!!!
                    _saleRequestTemp.PersonalizedCode = resultSaleRequest.PersonalizedCode;
                    _saleRequestTemp.EnterpriseId = resultSaleRequest.EnterpriseId;

                    saleRequestTempDao.Add(_saleRequestTemp);
                }

                ModelSaleRequestProductTemp _saleRequestProductTemp = new ModelSaleRequestProductTemp();
                _saleRequestProductTemp.SaleRequestTempId = _saleRequestTemp.SaleRequestTempId;
                _saleRequestProductTemp.ProductPriceLookUpCode = product.PriceLookupCode;
                _saleRequestProductTemp.ProductInternalCodeOnErp = product.InternalCodeOnERP;
                _saleRequestProductTemp.Value = Convert.ToDecimal(product.SaleRetailPraticedString);
                int quantidade = Convert.ToInt32(_QUANT);
                if (quantidade < 1)
                {
                    quantidade = 1;
                }
                _saleRequestProductTemp.Quantity = quantidade;
                _saleRequestProductTemp.TotalLiquid = (_saleRequestProductTemp.Value * _saleRequestProductTemp.Quantity);
                //_saleRequestTemp.Products.Add(_saleRequestProductTemp);
                _saleRequestTemp.TotalLiquid += _saleRequestProductTemp.TotalLiquid;

                saleRequestProductTempDao.Add(_saleRequestProductTemp);
                saleRequestTempDao.Update(_saleRequestTemp);

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
