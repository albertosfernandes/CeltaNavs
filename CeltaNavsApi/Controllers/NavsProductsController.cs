using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using CeltaNavs.Domain;
using CeltaNavs.Repository;
using CeltaNavsApi.Helpers;

namespace CeltaNavsApi.Controllers
{
    public class NavsProductsController : ApiController
    {
        private string navsIp;
        private string navsPort;
        NavsSettingDao navsSettingsDao = new NavsSettingDao();
        ExpansibleGroupDao groupDao = new ExpansibleGroupDao();
        ProductDao productsDao = new ProductDao();

        ModelNavsSetting modelSetting = new ModelNavsSetting();
        ModelExpansibleGroup groups = new ModelExpansibleGroup();
       

        public NavsProductsController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }

        public HttpResponseMessage Get(string _CODPROD, string _PRODUCTTABLE, string _TERMINALSERIAL)
        {
            string XML = "";
            try
            {
                modelSetting = navsSettingsDao.GetById(_TERMINALSERIAL);

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

                //if (_CODPROD == "2")
                //{                   
                //    XML += $"<GET TYPE=HIDDEN NAME=_TABLE VALUE={_PRODUCTTABLE}>";
                //    XML += $"<GET TYPE=HIDDEN NAME=_POSSERIAL VALUE={_TERMINALSERIAL}>";
                //    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssaleRequest/CancelProduct HOST=h TIMEOUT=10>";

                //    return new HttpResponseMessage(HttpStatusCode.OK)
                //    {
                //        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                //    };
                //}

                if (_CODPROD == "0")
                {
                    XML += "<console>Solicitando conta!!!</console>";                    
                    XML += $"<GET TYPE=HIDDEN NAME=_TOTALCARD VALUE={_PRODUCTTABLE}>";
                    XML += $"<GET TYPE=HIDDEN NAME=_TOTALTERMINALSERIAL VALUE={_TERMINALSERIAL}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navstotalize/gettotal HOST=h>";

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

                modelSetting = navsSettingsDao.GetById(_PRODUCTTERMINALSERIAL);
                XML += "<CONSOLE>Lista de produtos<BR>";
                XML += "----------------------------------------<BR>";

                XML += string.Format("{0,-3} | {1,-2}", "Codigo", "Descricao" + "<BR>");
                XML += "----------------------------------------<BR></CONSOLE>";
                var listOfProducts = productsDao.GetByGroup(modelSetting, _SELGROUP);
                XML += Menu.MenuListProducts(listOfProducts);

                //XML += "<CONSOLE><BR><BR>    Quantidade: <BR></CONSOLE>";
                //XML += "<GET TYPE=FIELD NAME=_QUANT LIN=7 COL=4 SIZE=1>";
                XML += $"<GET TYPE=HIDDEN NAME=_PERSONALIZEDCODE VALUE={_PRODUCTTABLE}>";                
                XML += $"<GET TYPE=HIDDEN NAME=_TERMINALSERIAL VALUE={_PRODUCTTERMINALSERIAL}>";                
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsQuantity/get HOST=h TIMEOUT=5>";
                //XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssaleRequest/AddProduct HOST=h TIMEOUT=5>";

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
