using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.PrintService
{
    public class Print
    {
     
        private Font regular = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
        private ModelSaleRequest saleRequest = new ModelSaleRequest();
        private static List<ModelSaleRequestProduct> listSaleProducts = new List<ModelSaleRequestProduct>();
     

        //obtem impressora default
        public static string GetDefaultPrinterName()
        {
            PrinterSettings prtSettings = new PrinterSettings();
            var printers = PrinterSettings.InstalledPrinters;

            foreach (string printer in printers)
            {                
                prtSettings.PrinterName = printer;
                if (prtSettings.IsDefaultPrinter)
                    break;
            }
            return prtSettings.PrinterName;
        }

        public void ToPrint(ModelSaleRequest _saleRequest, List<ModelSaleRequestProduct> _listProducts)
        {            
            this.saleRequest = _saleRequest;
            Print.listSaleProducts = _listProducts;
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocumentOnPrintPage;
            printDocument.Print();
        }


        private static void PrintDocumentOnPrintPage(object sender, PrintPageEventArgs e)
        {
            Font bold = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            Font regularItens = new Font(FontFamily.GenericSansSerif, 6, FontStyle.Regular);

            e.Graphics.DrawString("Empresa: Empresa Teste", bold, Brushes.Black, 10, 25);
            e.Graphics.DrawString("Pedido: 22", bold, Brushes.Black, 20, 35);


            foreach (ModelSaleRequestProduct p in listSaleProducts)
            {
                string produto = p.Product.NameReduced;
                e.Graphics.DrawString(produto.Length > 20 ? produto.Substring(0, 20) + "..." : produto, regularItens, Brushes.Black, 20, 35);
                //graphics.DrawString(FormataMonetario.format(iv.valorUn), regularItens, Brushes.Black, 155, offset);
                e.Graphics.DrawString(Convert.ToString(p.Quantity), regularItens, Brushes.Black, 215, 35);
                //graphics.DrawString(FormataMonetario.format(iv.total), regularItens, Brushes.Black, 250, offset);

            }
        }
        

        //private void printPage(object send, PrintPageEventArgs e)
        //{
        //    Graphics graphics = e.Graphics;
        //    int offset = 105;

        //    //print header
        //    graphics.DrawString("Empresa Teste", bold, Brushes.Black, 20, 0);            
        //    graphics.DrawLine(Pens.Black, 20, 30, 310, 30);
        //    graphics.DrawString("CUPOM NÃO FISCAL", bold, Brushes.Black, 80, 35);
        //    graphics.DrawLine(Pens.Black, 20, 50, 310, 50);
        //    graphics.DrawString("PEDIDO: 22", regular, Brushes.Black, 20, 60);
        //    graphics.DrawLine(Pens.Black, 20, 75, 310, 75);

        //    //header
        //    graphics.DrawString("PRODUTO", regular, Brushes.Black, 20, 80);
        //    //graphics.DrawString("UNIT.", regular, Brushes.Black, 150, 80);
        //    graphics.DrawString("QTD.", regular, Brushes.Black, 200, 80);
        //    //graphics.DrawString("TOTAL", regular, Brushes.Black, 245, 80);
        //    graphics.DrawLine(Pens.Black, 20, 95, 310, 95);

        //    //itens            
        //    foreach (ModelSaleRequestProduct p in listSaleProducts)
        //    {
        //        string produto = p.Product.NameReduced;
        //        graphics.DrawString(produto.Length > 20 ? produto.Substring(0, 20) + "..." : produto, regularItens, Brushes.Black, 20, offset);
        //        //graphics.DrawString(FormataMonetario.format(iv.valorUn), regularItens, Brushes.Black, 155, offset);
        //        graphics.DrawString(Convert.ToString(p.Quantity), regularItens, Brushes.Black, 215, offset);
        //        //graphics.DrawString(FormataMonetario.format(iv.total), regularItens, Brushes.Black, 250, offset);
        //        offset += 20;
        //    }
        //    //total
        //    graphics.DrawLine(Pens.Black, 20, offset, 310, offset);
        //    offset += 5;

        //    //decimal total = 0;
        //    //foreach (ItemVenda iv in venda.items)
        //    //{
        //    //    total += iv.total;
        //    //}
        //    //graphics.DrawString("TOTAL R$: ", bold, Brushes.Black, 20, offset);
        //    //graphics.DrawString(FormataMonetario.format(total), bold, Brushes.Black, 230, offset);
        //    //offset += 15;

        //    graphics.DrawLine(Pens.Black, 20, offset, 310, offset);
        //    offset += 5;

        //    //bottom
        //    graphics.DrawString("Data: " + DateTime.Now.ToString("dd/MM/yyyy"), regularItens, Brushes.Black, 20, offset);
        //    graphics.DrawString("HORA: " + DateTime.Now.ToString("HH:mm:ss"), regularItens, Brushes.Black, 220, offset);

        //    e.HasMorePages = false;

        //}
    }
}
