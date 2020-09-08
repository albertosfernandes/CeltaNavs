using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CeltaNavs.Repository;

namespace CeltaNavsApi.Helpers
{
    public class Menu
    {
        public static string MenuListGroup(List<ModelExpansibleGroup> listOfGroup)
        {
            string XML = "";
            try
            {
                XML += $"<WRITE_AT LINE=26 COLUMN=1>3 para subir</WRITE_AT>";
                XML += $"<WRITE_AT LINE=27 COLUMN=1>6 para descer</WRITE_AT>";

                XML += "<SELECT LIN=6 COL=2 SIZE=40 QTD=10 UP=B3 DOWN=B6 RIGHT=B1 LEFT=B2 NAME=_SELGROUP TYPE_RETURN=3 INDEX=";

                foreach (var group in listOfGroup)
                {

                    XML += $"{group.ExpansibleGroupId},";
                }

                XML += ">";
                foreach (var group in listOfGroup)
                {
                    string cod = group.ExpansibleGroupId.ToString();

                    string desc = group.Name;
                                                         
                    XML += $"{cod}%20%20%20%20%20|%20{desc},";
                }
                XML += "</SELECT> ";               

                return XML;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static string MenuListProducts(List<CeltaNavs.Repository.ModelProduct> listOfProducts)
        {
            string XML = "";
            try
            {
                XML += $"<WRITE_AT LINE=26 COLUMN=1>3 para subir</WRITE_AT>";
                XML += $"<WRITE_AT LINE=27 COLUMN=1>6 para descer</WRITE_AT>";

                XML += "<SELECT LIN=6 COL=2 SIZE=40 QTD=10 UP=B3 DOWN=B6 RIGHT=B1 LEFT=B2 NAME=_SELPROD TYPE_RETURN=3 INDEX=";

                foreach (var product in listOfProducts)
                {

                    XML += $"{product.InternalCodeOnERP},";
                }

                XML += ">";
                foreach (var product in listOfProducts)
                {
                    string cod = Formatted.FormatCode(product);

                    string desc = Formatted.FormatDescription(product);
                                                        
                    XML += $"{cod}%20|%20{desc},";
                }
                XML += "</SELECT> ";               

                return XML;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
      
        public static string SaleRequestItens(List<ModelSaleRequestProduct> listOfItensCard)
        {
            string XML = "";
            try
            {
                XML += string.Format("{0,-7}| {1,-21}| {2,3}", "Codigo", "Descricao", "Quant." + "<BR>");
                XML += "----------------------------------------<BR>";

                int i = 0;
                foreach (var item in listOfItensCard)
                {
                    CeltaNavs.Repository.ModelProduct prod = new CeltaNavs.Repository.ModelProduct();
                    string cod = item.Product.PriceLookupCode;
                    prod.NameReduced = item.Product.NameReduced;
                    string desc = Formatted.FormatDescriptionSummary(prod);
                    string quant = item.Quantity.ToString();

                    XML += string.Format("{0,-7}| {1,-21}| {2,3}", cod, desc, quant + "<BR>");
                    i++;
                    if (i > 19)
                    {
                        XML += "Proximos 19 itens, pressione 4.";
                        break;
                    }
                }

                return XML;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static string SaleRequestItensTemp(List<ModelSaleRequestProductTemp> listOfItensCardTemp)
        {
            string XML = "";
            try
            {
                XML += string.Format("{0,-7}| {1,-21}| {2,3}", "Codigo", "Descricao", "Quant." + "<BR>");
                XML += "----------------------------------------<BR>";

                int i = 0;
                foreach (var item in listOfItensCardTemp)
                {
                    CeltaNavs.Repository.ModelProduct prod = new CeltaNavs.Repository.ModelProduct();
                    string cod = item.Product.PriceLookupCode;
                    prod.NameReduced = item.Product.NameReduced;
                    string desc = Formatted.FormatDescriptionSummary(prod);
                    string quant = item.Quantity.ToString();

                    XML += string.Format("{0,-7}| {1,-21}| {2,3}", cod, desc, quant + "<BR>");
                    i++;
                    if (i > 19)
                    {
                        XML += "Impossível adicionar mais itens a tela.";
                        break;
                    }
                }

                return XML;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static string SaleOrderTotalQuantity(List<ModelSaleRequestProduct> listOfItensCard)
        {
            decimal soma = 0;
            foreach (var item in listOfItensCard)
            {
                soma += item.Quantity;
            }

            return soma.ToString();
        }

        public static string SaleOrderTotalQuantityTemp(List<ModelSaleRequestProductTemp> listOfItensCard)
        {
            decimal soma = 0;
            foreach (var item in listOfItensCard)
            {
                soma += item.Quantity;
            }

            return soma.ToString();
        }

        public static string MenuListSaleRequestProducts(List<ModelSaleRequestProduct> listOfItensCard)
        {
            string XML = "";
            try
            {
                XML += "<SELECT LIN=6 COL=1 SIZE=40 QTD=10 UP=B3 DOWN=B6 RIGHT=B1 LEFT=B2 NAME=_SELMOVID TYPE_RETURN=3 INDEX=";

                foreach (var item in listOfItensCard)
                {

                    XML += $"{item.SaleRequestProductId},";
                }

                XML += ">";
                foreach (var product in listOfItensCard)
                {
                    ModelProduct p = new ModelProduct();
                    p.PriceLookupCode = product.Product.PriceLookupCode;
                    p.NameReduced =  product.Product.NameReduced;

                    string cod = Formatted.FormatCode(p);
                    string desc = Formatted.FormatDescription20positions(p);
                    string quant = product.Quantity.ToString("0.00");

                    XML += $"{cod}%20|%20{desc}|%20{quant},";
                }
                XML += "</SELECT> ";

                return XML;
            }
            catch (Exception err)
            {
                throw err;
            }
        }        

        public static string MenuListCards(List<ModelSaleRequest> listOfCard)
        {
            string XML = "";
            try
            {
                XML += $"<WRITE_AT LINE=26 COLUMN=1>3 para subir</WRITE_AT>";
                XML += $"<WRITE_AT LINE=27 COLUMN=1>6 para descer</WRITE_AT>";

                XML += "<SELECT LIN=6 COL=1 SIZE=40 QTD=20 UP=B3 DOWN=B6 RIGHT=B1 LEFT=B2 NAME=_CARD TYPE_RETURN=3 INDEX=";

                foreach (var item in listOfCard)
                {

                    XML += $"{item.PersonalizedCode},";
                }
                XML += ">";

                foreach (var card in listOfCard)
                {
                    string cardnumer = card.PersonalizedCode;

                    XML += $"%20{cardnumer},";
                }
                XML += "</SELECT> ";

                return XML;
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        public static string MenuListTables(List<ModelSaleRequest> listOfTables)
        {
            string XML = "";
            try
            {
                XML += $"<WRITE_AT LINE=26 COLUMN=1>3 para subir</WRITE_AT>";
                XML += $"<WRITE_AT LINE=27 COLUMN=1>6 para descer</WRITE_AT>";

                XML += "<SELECT LIN=6 COL=1 SIZE=40 QTD=20 UP=B3 DOWN=B6 RIGHT=B1 LEFT=B2 NAME=_TABLE TYPE_RETURN=3 INDEX=";

                foreach (var item in listOfTables)
                {

                    XML += $"{item.PersonalizedCode},";
                }
                XML += ">";

                foreach (var t in listOfTables)
                {
                    string tablenumber = t.PersonalizedCode;

                    XML += $"%20{tablenumber},";
                }
                XML += "</SELECT> ";

                return XML;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static string MenuSalesSitef(List<ModelSaleMovementFinalization> listOfSaleMovementFinalizations)
        {
            string XML = "";
            try
            {
                XML += $"<WRITE_AT LINE=26 COLUMN=1>3 para subir</WRITE_AT>";
                XML += $"<WRITE_AT LINE=27 COLUMN=1>6 para descer</WRITE_AT>";

                XML += "<SELECT LIN=6 COL=1 SIZE=40 QTD=20 UP=B3 DOWN=B6 RIGHT=B1 LEFT=B2 NAME=_STEF TYPE_RETURN=3 INDEX=";

                foreach (var item in listOfSaleMovementFinalizations)
                {

                    XML += $"{item.NavsFinalizationId},";
                }
                XML += ">";

                foreach (var sale in listOfSaleMovementFinalizations)
                {
                    string value = sale.Value.ToString("0.00");
                    string doc = sale.CODAUT;

                    XML += $"%20Valor:{value}%20%20Doc:{doc},";
                }
                XML += "</SELECT> ";
               
                return XML;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static string MenuEnterprises(List<ModelEnterprise> listOfEnterprises)
        {
            try
            {
                string XML = "";
                XML += $"<WRITE_AT LINE=26 COLUMN=1>3 para subir</WRITE_AT>";
                XML += $"<WRITE_AT LINE=27 COLUMN=1>6 para descer</WRITE_AT>";

                XML += "<SELECT LIN=6 COL=1 SIZE=40 QTD=20 UP=B3 DOWN=B6 RIGHT=B1 LEFT=B2 NAME=_ENTID TYPE_RETURN=3 INDEX=";
                foreach (var item in listOfEnterprises)
                {

                    XML += $"{item.EnterpriseId},";
                }
                XML += ">";

                foreach (var enterprise in listOfEnterprises)
                {
                    string enterpriseCode = enterprise.PersonalizedCode;
                    string enterpriseName = enterprise.FantasyName;                    

                    XML += $"%20Empresa:{enterpriseCode}%20%20Nome:{enterpriseName},";
                }
                XML += "</SELECT> ";

                return XML;
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        public static string MenuPdvs(List<ModelPdv> listOfPdvs)
        {
            string XML = "";
            try
            {
                XML += $"<WRITE_AT LINE=26 COLUMN=1>3 para subir</WRITE_AT>";
                XML += $"<WRITE_AT LINE=27 COLUMN=1>6 para descer</WRITE_AT>";

                XML += "<SELECT LIN=6 COL=1 SIZE=40 QTD=20 UP=B3 DOWN=B6 RIGHT=B1 LEFT=B2 NAME=_PDVID TYPE_RETURN=3 INDEX=";
                foreach (var item in listOfPdvs)
                {

                    XML += $"{item.PdvId},";
                }
                XML += ">";

                foreach (var pdv in listOfPdvs)
                {
                    string pdvNumber = pdv.PdvNumber.ToString();                     

                    XML += $"%20PDV:{pdvNumber},";
                }
                XML += "</SELECT> ";

                return XML;

            }
            catch(Exception err)
            {
                throw err;
            }
        }

    }
}