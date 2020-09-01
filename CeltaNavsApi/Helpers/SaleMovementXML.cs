using CeltaNavs.Repository;
//using CeltaNavsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace CeltaNavsApi.Helpers
{
    public class SaleMovementXML
    {

        public static string GetXmlToSATNFCEFromSaleMovement(ModelSaleMovement nSaleMovement, ICollection<ModelSaleRequestProduct> saleProducts, IList<ModelSaleMovementFinalization> salePayments, bool isForSAT, ModelNavsSetting navsSettings)
        {            

            XmlDocument xmlSAT = new XmlDocument();

            xmlSAT.AppendChild(xmlSAT.CreateXmlDeclaration("1.0", "ISO-8859-1", ""));

            XmlElement elementCupom = xmlSAT.CreateElement("Cupom");
            xmlSAT.AppendChild(elementCupom);

            elementCupom.AppendChild(xmlSAT.CreateElement("CodigoEmpresa")).InnerText = nSaleMovement.Enterprises.PersonalizedCode;
            elementCupom.AppendChild(xmlSAT.CreateElement("FlagSimplesFederalEmpresa")).InnerText = nSaleMovement.Enterprises.FederalSimpleType.ToString();            
            
            elementCupom.AppendChild(xmlSAT.CreateElement("CNPJEmpresa")).InnerText = nSaleMovement.Enterprises.CNPJ;
            elementCupom.AppendChild(xmlSAT.CreateElement("INSCESTEmpresa")).InnerText = nSaleMovement.Enterprises.InscriptionState;            

            elementCupom.AppendChild(xmlSAT.CreateElement("NumeroCaixa")).InnerText = nSaleMovement.Pdvs.PdvNumber.ToString();
            elementCupom.AppendChild(xmlSAT.CreateElement("NumeroCupom")).InnerText = nSaleMovement.PersonalizedCode;
            //elementCupom.AppendChild(xmlSAT.CreateElement("NumeroOrdemCupom")).InnerText = sale.PersonalizedCodeReduced;

            ////Pra efeitos fiscais.. a venda é gerada agora, mano!!
            elementCupom.AppendChild(xmlSAT.CreateElement("DataHoraVenda")).InnerText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            elementCupom.AppendChild(xmlSAT.CreateElement("PDVTYPE")).InnerText = "1"; //CeltaBSPDV

            var elementCustomer = elementCupom.AppendChild(xmlSAT.CreateElement("Cliente"));

            if (!String.IsNullOrEmpty(nSaleMovement.CPFCNPJ))
            {
                // !!!!! 
                // !!!!! Validar CPF e CPNJ
                //if (ValidationHelpers.IsEnterpriseIdentityOk(nSaleMovement.CPFCNPJ))
                //    elementCustomer.AppendChild(xmlSAT.CreateElement("CNPJ")).InnerText = nSaleMovement.CPFCNPJ;
                //else
                elementCustomer.AppendChild(xmlSAT.CreateElement("CPF")).InnerText = nSaleMovement.CPFCNPJ;
            }

            int countOfProducts = 0;


            foreach (var saleProd in saleProducts)
            {
                AddSaleProduct(ref elementCupom, ref xmlSAT, countOfProducts, saleProd.Product, saleProd.Quantity, isForSAT, navsSettings);
                countOfProducts++;
            }


            var finalization = elementCupom.AppendChild(xmlSAT.CreateElement("Finalizacao"));
            //finalization.AppendChild(xmlSAT.CreateElement("Desconto")).InnerText = calculator.GetTotalDiscountOnGeneral().ToString();
            //finalization.AppendChild(xmlSAT.CreateElement("Acrescimo")).InnerText = calculator.GetTotalIncrementOnGeneral().ToString();

            foreach (var salePayment in salePayments)
            {
                var elementPayment = finalization.AppendChild(xmlSAT.CreateElement("Pagamento"));
                elementPayment.AppendChild(xmlSAT.CreateElement("Finalizadora")).InnerText = ((int)salePayment.FinalizationId).ToString();
                elementPayment.AppendChild(xmlSAT.CreateElement("Valor")).InnerText = salePayment.Value.ToString();
            }            

            #region InfAdic

            var elementInfAdic = finalization.AppendChild(xmlSAT.CreateElement("InfAdic"));

            decimal totalTaxFromPercImpNBPTNational = (from p in saleProducts
                                                      where p.Product.PercImpNBPTNational > 0
                                                      select p.Product.SaleRetailPrice * (p.Product.PercImpNBPTNational / 100)).Sum();

            decimal totalTaxFromPercImpNBPTState = (from p in saleProducts
                                                   where p.Product.PercImpNBPTState > 0
                                                   select p.Product.SaleRetailPrice * (p.Product.PercImpNBPTState / 100)).Sum();

            decimal totalTaxFromPercImpNBPTMunicipal = (from p in saleProducts
                                                       where p.Product.PercImpNBPTMunicipal > 0
                                                       select p.Product.SaleRetailPrice * (p.Product.PercImpNBPTMunicipal / 100)).Sum();

            decimal totalTaxFromPercImpNBPT = totalTaxFromPercImpNBPTNational + totalTaxFromPercImpNBPTState + totalTaxFromPercImpNBPTMunicipal;

            elementInfAdic.AppendChild(xmlSAT.CreateElement("Observacoes")).InnerText =
                String.Format("Valor aproximado dos tributos deste cupom (R$) {0}. Federal {1} / Estadual {2} / Municipal {3}. Fonte IBPT(Conforme Lei Federal 12.741/2012)", totalTaxFromPercImpNBPT.ToString("0.00"), totalTaxFromPercImpNBPTNational.ToString("0.00"), totalTaxFromPercImpNBPTState.ToString("0.00"), totalTaxFromPercImpNBPTMunicipal.ToString("0.00"));

            #endregion

            return xmlSAT.InnerXml.ToString();
        }


        private static void AddSaleProduct(ref XmlElement elementCupom, ref XmlDocument xmlSAT, int indexOfProduct, ModelProduct saleProduct, decimal quantity, bool isForSAT, ModelNavsSetting _navsSettings)
        {            

            var elementProduct = elementCupom.AppendChild(xmlSAT.CreateElement("Produto"));

            elementProduct.AppendChild(xmlSAT.CreateElement("Item")).InnerText = indexOfProduct.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("PLU")).InnerText = saleProduct.PriceLookupCode;

            if (!String.IsNullOrEmpty(saleProduct.EanCode))
                elementProduct.AppendChild(xmlSAT.CreateElement("EAN")).InnerText = saleProduct.EanCode;

            //Retirar as acento do produto
            elementProduct.AppendChild(xmlSAT.CreateElement("NomeProduto")).InnerText = saleProduct.NameReduced.Trim();
            elementProduct.AppendChild(xmlSAT.CreateElement("AbreviacaoEmbalagem")).InnerText = saleProduct.PackingAbbreviation.Trim();

            //elementProduct.AppendChild(xmlSAT.CreateElement("Desconto")).InnerText = saleProduct.TotalDiscountAll.ToString();
            //elementProduct.AppendChild(xmlSAT.CreateElement("Acrescimo")).InnerText = saleProduct.TotalIncrement.ToString();

            elementProduct.AppendChild(xmlSAT.CreateElement("Quantidade")).InnerText = quantity.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("ValorUnitario")).InnerText = ValuesHelpers.GetPriceOrOffer(saleProduct, _navsSettings).SaleRetailPrice.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("ValorTotal")).InnerText = (ValuesHelpers.GetPriceOrOffer(saleProduct, _navsSettings).SaleRetailPrice * quantity).ToString();

            elementProduct.AppendChild(xmlSAT.CreateElement("CustoReposicao")).InnerText = saleProduct.RepositionCost.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("CustoReposicaoMedio")).InnerText = saleProduct.RepositionCostMiddle.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("CustoReposicaoReal")).InnerText = saleProduct.RepositionCostReal.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("CustoLiquido")).InnerText = saleProduct.LiquidCost.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("CustoLiquidoMedio")).InnerText = saleProduct.LiquidCostMiddle.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("CustoLiquidoReal")).InnerText = saleProduct.LiquidCostReal.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("VendaVarejo")).InnerText = saleProduct.SaleRetailPrice.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("VendaAtacado")).InnerText = saleProduct.WholeSalePrice.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("OfertaAtacado")).InnerText = saleProduct.WholeOfferPrice.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("OfertaVarejo")).InnerText = saleProduct.OfferRetailPrice.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("CFOP")).InnerText = saleProduct.CFOP.ToString();
            elementProduct.AppendChild(xmlSAT.CreateElement("NCM")).InnerText = saleProduct.FiscalClassificationNCM;

            var icms = elementProduct.AppendChild(xmlSAT.CreateElement("ICMS"));

            icms.AppendChild(xmlSAT.CreateElement("CST")).InnerText = saleProduct.TributarySituationCode;
            icms.AppendChild(xmlSAT.CreateElement("ALIQUOTA")).InnerText = saleProduct.IcmsAliquot.ToString();
            icms.AppendChild(xmlSAT.CreateElement("REDUC")).InnerText = saleProduct.IcmsReductionMargin.ToString();

            if (isForSAT &&
                saleProduct.TributarySituationCode.EndsWith("20") &&
                saleProduct.IcmsReductionMargin > 0)
                icms.AppendChild(xmlSAT.CreateElement("REDUZIRALIQUOTA")).InnerText = "1";

            icms.AppendChild(xmlSAT.CreateElement("IVA")).InnerText = saleProduct.IcmsIVAMargin.ToString();
            icms.AppendChild(xmlSAT.CreateElement("PAUTA")).InnerText = saleProduct.IcmsStave.ToString();

            var pis = elementProduct.AppendChild(xmlSAT.CreateElement("PIS"));
            pis.AppendChild(xmlSAT.CreateElement("TIPO")).InnerText = saleProduct.PisType.ToString();
            pis.AppendChild(xmlSAT.CreateElement("CST")).InnerText = saleProduct.CSTPisCofins;
            pis.AppendChild(xmlSAT.CreateElement("PERC")).InnerText = saleProduct.PisPercentage.ToString();

            var cofins = elementProduct.AppendChild(xmlSAT.CreateElement("COFINS"));
            cofins.AppendChild(xmlSAT.CreateElement("TIPO")).InnerText = saleProduct.CofinsType.ToString();
            cofins.AppendChild(xmlSAT.CreateElement("CST")).InnerText = saleProduct.CSTPisCofins;
            cofins.AppendChild(xmlSAT.CreateElement("PERC")).InnerText = saleProduct.CofinsPercentage.ToString();
        }

    }
}