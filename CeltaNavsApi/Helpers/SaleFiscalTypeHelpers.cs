using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace CeltaNavsApi.Helpers
{
    public class SaleFiscalTypeHelpers
    {
        public static string GetSaleFiscalType(string xmlCancelSaleMovement)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlCancelSaleMovement);

            XmlNodeList xmlNodes = document.GetElementsByTagName("CancelamentoCupom");

            //if (xmlNodes.Count > 0)
            //    return Convert.ToString(GetNodeElementText(xmlNodes[0]["TipoCupomFiscal"]));

            //Ok.. não é cancelamento de venda! é venda NFCe?
            xmlNodes = document.GetElementsByTagName("SaleFiscalType");

            if (xmlNodes.Count > 0)
            {
                if (xmlNodes[0].InnerText.ToUpperInvariant() == "SAT")
                {
                    var xmlManufacturerType = document.GetElementsByTagName("SATManufacturerType");

                    if (xmlManufacturerType != null &&
                        xmlManufacturerType.Count > 0 &&
                        xmlManufacturerType[0].InnerText.ToUpperInvariant() == "EMULADOR")
                        return "SATEMULADOR";
                }

                return xmlNodes[0].InnerText;
            }

            return string.Empty;
        }
    }
}