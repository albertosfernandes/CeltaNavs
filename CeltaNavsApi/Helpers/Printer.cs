using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CeltaNavsApi.Helpers
{
    public class Printer
    {
        

        public static string Print(string _CARDPAY, ICollection<ModelSaleRequestProduct> listOfProduct, ModelSaleRequest saleRequest, ModelNavsSetting navsSettings, bool isCoupon)
        {            
            string XML = "";
            try
            {
                if (!isCoupon)
                {
                    XML += "<PRNFNT SIZE=2>";
                    XML += $"<PRINTER>-----------------------<BR>";
                    XML += $"Pedido: {_CARDPAY}<BR>";
                    XML += $"Total: {saleRequest.TotalLiquid.ToString("0.00")}<BR>";
                    if (saleRequest.Peoples > 1)
                    {
                        XML += $"Dividir valor: {saleRequest.Peoples.ToString()}<BR>";
                        XML += $"Subtotal: {(saleRequest.TotalLiquid / Convert.ToDecimal(saleRequest.Peoples)).ToString("0.00")}<BR>";
                    }                    
                    XML += $"-----------------------<BR></PRINTER>";                
                }               
                decimal totalsale = 0;

                XML += "<PRNFNT SIZE=4>";
                XML += "<PRINTER>";
                XML += string.Format("{0,-20}| {1,-3}| {2,-3}| {3,-3}", "Descricao           ", "Unit.", "Quant.", "Total" + "<BR>");
                XML += "----------------------------------------<BR>";


                foreach (var item in listOfProduct)
                {
                    string desc;                    
                    var product = item.Product;
                    if (product.NameReduced.Length > 20)
                    {
                        desc = product.NameReduced.Substring(0, 20);
                    }
                    else
                    {
                        desc = product.NameReduced.Substring(0);
                        for (int i = product.NameReduced.Length; i < 20; i++)
                        {
                            desc += " ";
                        }
                    }                    

                    string salePrice = item.Product.SaleRetailPraticedString;
                    if (salePrice.Length < 6)
                    {
                        for (int i = salePrice.Length; i < 6; i++)
                        {
                            salePrice += " ";
                        }
                    }
                    decimal total = (item.Quantity * Convert.ToDecimal(salePrice));
                    totalsale += total;
                    
                    XML += string.Format("{0,-3}| {1,-3}| {2,-3}| {3,3}", desc, salePrice, item.Quantity, total.ToString("0.00") + "<BR>");

                }
                XML += "</PRINTER>";

                    XML += "<CHGPRNFNT SIZE=2 FACE=FONTE2 BOLD>";                    
                    XML += "<PRINTER>";                    
                    XML += "<BR>-----------------------<BR><BR>";
                    XML += $"Total R$: {totalsale.ToString("0.00")}<BR><BR><BR><BR><BR></PRINTER>";

                return XML;
            }
            catch(Exception err)
            {
                throw err;
            }
            
        }


        public static string PrintQrCode(string qrCodeValue)
        {
            string XML = "";
            try
            {
                XML += $"<GENERATE_QR_CODE SIZE=3 QR_ECLEVEL=0 KEEP_FILE=0 SPACES=16 FILE_NAME=testeQRA.bmp NO_PRINT=0 ERR_QR=VAR_R>";
                XML += $"      {qrCodeValue}";
                XML += $"</GENERATE_QR_CODE>";

                XML += $"<PRINTER>----------------------------------------<BR><BR><BR><BR><BR>";                
                XML += $"</PRINTER>";
                return XML;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static string PrintCodeBar(string codeBarValue)
        {
            string XML = "";
            try
            {
                string value = codeBarValue.Substring(3);                
                XML += $"<code128 type=c width=1 height=64 spaces=28 orientation=horizontal error=error_ret>";
                XML += $"{value}";
                XML += $"</code128>";

                XML += $"<PRINTER><BR><BR></PRINTER>";                

                return XML;
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        public static string Header(ModelSaleMovement nSaleMov, int couponNumber)
        {
            string XML = "";
            try
            {

                XML += "<CHGPRNFNT SIZE=4 FACE=FONTE2>";
                XML += "<PRINTER>";                
                XML += $"<BR>{nSaleMov.Enterprises.FantasyName}<BR>";
                XML += $"CNPJ:{nSaleMov.Enterprises.CNPJ}<BR>";
                XML += "</printer>";

                XML += "<CHGPRNFNT SIZE=4 FACE=FONTE1>";
                XML += "<PRINTER>";
                XML += $"Caixa: {nSaleMov.Pdvs.PdvNumber} - Cupom: {couponNumber} - Pedido: {nSaleMov.PersonalizedCode}";
                XML += "<BR>----------------------------------------<BR>";
                XML += "</printer>";


                XML += "<CHGPRNFNT SIZE=2 FACE=FONTE2>";
                XML += "<PRINTER>";
                XML += "    CUPOM     FISCAL   <BR>";
                XML += "</printer>";

                return XML;
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        public static string PrintPayments(List<ModelSaleMovementFinalization> listOfnSaleOrderFinalization)
        {
            string XML = "";
            try
            {
                XML += "<PRNFNT SIZE=2>";
                XML += $"<PRINTER><BR>FORMAS DE PAGAMENTO<BR>";
                XML += "</PRINTER>";

                XML += "<PRNFNT SIZE=4>";
                XML += "<PRINTER>";
                foreach (var payment in listOfnSaleOrderFinalization)
                {
                    if (payment.FinalizationId == 1)
                    {
                        XML += $"Dinheiro: R$ {payment.Value.ToString("0.00")}<BR>";
                        if(payment.PayBackValue > 0)
                        {
                            XML += $"Troco: R$ {payment.PayBackValue.ToString("0.00")}<BR>";
                        }
                    }
                        
                    else
                        if (payment.FinalizationId == 5)
                        XML += $"Cartao Debito:  {payment.Value.ToString("0.00")}<BR>";
                    else
                        if (payment.FinalizationId == 6)
                        XML += $"Cartao Credito:  {payment.Value.ToString("0.00")}<BR>";
                }
                XML += $"</PRINTER>";

                return XML;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}