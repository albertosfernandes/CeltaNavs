using CeltaNavs.Domain;
using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CeltaNavs.PrintService
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer1 = null;
        private List<ModelSaleRequestProduct> listOfProducts = new List<ModelSaleRequestProduct>();
        private SaleRequestProductDao saleRequestProductDao = new SaleRequestProductDao();
        private EnterpriseDao enterpriseDao = new EnterpriseDao();

        private ModelEnterprise enterprise = new ModelEnterprise();
        private List<ModelSaleRequest> saleRequests = new List<ModelSaleRequest>();
        private string navsAddress;        
        protected HttpClient _httpClient = null;
        private Print meuprint = new Print();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int _interval = Properties.Settings.Default.UpdateTime;
            //_interval *= 60;
            _interval *= 1000;
            timer1 = new Timer();
            this.timer1.Interval = _interval;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {                
                //qual empresa estamos?
                if (String.IsNullOrEmpty(Properties.Settings.Default.EnterprisePersonalizedCode.ToString()))
                {
                    //gerar log erro de empresa Não informada
                    WriteErrorLog("Empresa não informada em arquivo config");
                }
                else
                {             
                    navsAddress = Properties.Settings.Default.NavsCeltaAddressAPI;                    

                    using(var client = new HttpClient())
                    {
                        client.Timeout = new TimeSpan(0, 0, 30);
                        client.BaseAddress = new Uri($"{navsAddress}");

                        var responseHttp = client.GetAsync("/api/APInavsSetting/GetEnterpriseByPersonalizedCode?_personalizedCode=" + Properties.Settings.Default.EnterprisePersonalizedCode);
                        responseHttp.Wait();

                        var result = responseHttp.Result;

                        if (result.IsSuccessStatusCode)
                        {
                            var readResult = result.Content.ReadAsAsync<ModelEnterprise>();
                            readResult.Wait();

                            enterprise = readResult.Result;
                        }
                    }                    
                   
                }
                //ja tenho a empresa vms buscar o que tem para ser impresso!
                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, 30);
                    client.BaseAddress = new Uri($"{navsAddress}");
                    listOfProducts.Clear();

                    //var responseHttp = client.GetAsync("/api/APISaleRequestProduct/GetForPrint?_enterpriseId=" + enterprise.EnterpriseId);
                    // 
                    var responseHttp = client.GetAsync($"/api/APISaleRequest/NewGetAll?_enterpriseId={enterprise.EnterpriseId}&isUsing=0&isCancel=0&isDelivered=0&isPrinted=0");
                    responseHttp.Wait();

                    var result = responseHttp.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var read = result.Content.ReadAsAsync<ModelSaleRequest[]>();
                        read.Wait();

                        var saleRequestResult = read.Result;
                   
                        foreach (var saleRequest in saleRequestResult)
                        {
                            if (saleRequest.Products?.Any() == true)
                            {
                                string headprint = $"Empresa: {enterprise.FantasyName}.\n";
                                headprint += "Pedido: " + saleRequest.PersonalizedCode + ".\n";
                                string message = String.Empty;
                                //tem alguma coisa então é só mandar imprimir
                                foreach (var product in saleRequest.Products)
                                {
                                    message = $"Produto: {product.Product.NameReduced} | Quantidade: {product.Quantity}. \n";

                                    MarkToPrinted(product.SaleRequestProductId);
                                    //listOfProducts.Add(product);
                                }
                                PrintTest(headprint + message);
                                //print.ImprimeVendaVista(listOfProducts);
                                Print p = new Print();
                                p.ToPrint(saleRequest, saleRequest.Products);
                            }
                        }
                    }                                     
                }                

                
            }
            catch(Exception err)
            {
                //gerar log erro de empresa Não informada
                WriteErrorLog("Erro: " + err.Message);
            }                                  
        }

        private void MarkToPrinted(int saleRequestProductId)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 30);
                client.BaseAddress = new Uri($"{navsAddress}");

                var responseHttp = client.GetAsync("/api/APISaleRequestProduct/MarkToPrinted?_saleRequestProductId=" + saleRequestProductId);
                responseHttp.Wait();

                var result = responseHttp.Result;
                if (result.IsSuccessStatusCode)
                {
                    // ok  então
                }
                else
                {
                    WriteErrorLog("Erro ao marcar produto impresso.");
                }
            }
        }

        public static void WriteErrorLog(string Message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\ErrorLog.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }

        public static void PrintTest(string Message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Print.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();

                //depois que imprimir preciso marcar que o produto ja foi impresso!
            }
            catch
            {
            }
        }

        public static void PrinterTeste(string message)
        {
            try
            {
                //obtem impressora default
                var printers = System.Drawing.Printing.PrinterSettings.InstalledPrinters;

               
                using (var printDocument = new System.Drawing.Printing.PrintDocument())
                {
                    foreach (string printer in printers)
                    {
                        printDocument.PrinterSettings.PrinterName = printer;
                        if (printDocument.PrinterSettings.IsDefaultPrinter)
                            break;
                    }
                    //printDocument.PrintPage += printDocument_PrintPage;
                    //printDocument.PrinterSettings.PrinterName = System.Drawing.Printing.PrinterSettings.InstalledPrinters[0];
                    //printDocument.Print();
                }
            }
            catch(Exception err)
            {
                WriteErrorLog("Erro ao imprimir.");
            }
        } 

        protected override void OnStop()
        {
            timer1.Enabled = false;
        }
    }
}
