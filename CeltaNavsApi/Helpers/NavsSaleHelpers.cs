using CeltaNavs.Domain;
using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Linq;

namespace CeltaNavsApi.Helpers
{
    public class NavsSaleHelpers
    {
        #region Private properties
        private ModelSaleRequestTemp saleRequestTemp = new ModelSaleRequestTemp();
        private HttpClient _httpClient = null;
        private string navsIp;
        private string navsPort;
        private SaleRequestProductTempDao saleRequestProductTempDao = new SaleRequestProductTempDao();
        private SaleRequestTempDao saleRequestTempDao = new SaleRequestTempDao();
        #endregion

        #region Private Methods
        private static string GetNodeElementText(XmlElement element)
        {
            if (element != null)
                return element.InnerText;

            return String.Empty;
        }
        #endregion

        public NavsSaleHelpers()
        {
            this.navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            this.navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }
      


        public static string GetFiscalNoteAccessKey(string xmlSale)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlSale);

            //Procuro no xml de cancelamento de venda?
            XmlNodeList xmlNode = document.GetElementsByTagName("CancelamentoCupom");

            if (xmlNode.Count > 0 && xmlNode[0]["Chave"] != null)
                return Convert.ToString(GetNodeElementText(xmlNode[0]["Chave"]));

            //Ok.. não é cancelamento de venda! é venda NFCe?
            xmlNode = document.GetElementsByTagName("fiscalNoteConsumerEletronicKeyAccess");

            if (xmlNode.Count > 0)
                return xmlNode[0].InnerText;


            //Ok..ok.. não é NFCE? é retorno do SAT então?
            var _readXml = XElement.Parse(xmlSale);
            var elem = _readXml.Descendants("XmlSale").FirstOrDefault();
            var resultKey = (XCData)elem.FirstNode;

            if (resultKey.Value != null &&
                resultKey.Value != String.Empty)
            {
                document.LoadXml(resultKey.Value);

                var infCFeNode = document.DocumentElement.SelectSingleNode("infCFe");

                if (infCFeNode != null)
                    return infCFeNode.Attributes["Id"].Value;
            }
            else
            {
                document.LoadXml(xmlSale);
                var _readxml = document.GetElementsByTagName("ConsultKey");
                return _readxml[0].InnerText;
            }

            return String.Empty;
        }

        public ModelSaleRequestTemp TransferSaleRequestToSaleRequestTemp(ModelSaleRequest saleRequest)
        {
            //preciso carrega-lo na tabela temp!!!!!
            saleRequestTemp.PersonalizedCode = saleRequest.PersonalizedCode;
            saleRequestTemp.EnterpriseId = saleRequest.EnterpriseId;

            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.BaseAddress = new Uri($"http://{navsIp}:{navsPort}");
            HttpResponseMessage response = null;
            var content = new ObjectContent<ModelSaleRequestTemp>(saleRequestTemp, new JsonMediaTypeFormatter());

            response = _httpClient.PostAsync("api/APISaleRequest/AddSaleRequestTemp", content).Result;

            if (!response.IsSuccessStatusCode)
            {
                //O`pa deu erro ao criar o pedido na Temp!!

            }

            var resultSaleRequestTemp = saleRequestTempDao.Get(saleRequest.EnterpriseId.ToString(), saleRequest.PersonalizedCode, false);

            foreach (var p in saleRequest.Products)
            {
                ModelSaleRequestProductTemp pTemp = new ModelSaleRequestProductTemp();
                //pTemp.Product = p.Product;
                pTemp.ProductInternalCodeOnErp = p.ProductInternalCodeOnErp;
                pTemp.ProductPriceLookUpCode = p.ProductPriceLookUpCode;
                pTemp.Quantity = p.Quantity;
                //pTemp.SaleRequestProductTempId = p.sa
                //pTemp.SaleRequestTemp = saleRequestTemp;
                pTemp.SaleRequestTempId = resultSaleRequestTemp.SaleRequestTempId;
                pTemp.TotalLiquid = p.TotalLiquid;
                pTemp.Value = p.Value;
                //saleRequestTemp.Products.Add(pTemp);
                resultSaleRequestTemp.TotalLiquid += p.TotalLiquid;
                saleRequestProductTempDao.Add(pTemp);
            }
                saleRequestTempDao.Update(resultSaleRequestTemp);

            return resultSaleRequestTemp;
        }
    }
}