using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace CeltaNavsApi.Helpers
{
    public class NavsSaleHelpers
    {
        #region Private Methods
        private static string GetNodeElementText(XmlElement element)
        {
            if (element != null)
                return element.InnerText;

            return String.Empty;
        }
        #endregion


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
    }
}