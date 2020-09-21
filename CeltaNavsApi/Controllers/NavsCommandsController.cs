using CeltaNavs.Domain;
using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using System.Xml;

namespace CeltaNavsApi.Controllers
{
    public class NavsCommandsController : BaseController
    {
        //private string navsIp;
        //private string navsPort;

        //NavsSettingDao settingsdao = new NavsSettingDao();
        //ModelNavsSetting modelSetting = new ModelNavsSetting();

        public NavsCommandsController()
        {            
            //navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            //navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }

        [HttpGet]
        public HttpResponseMessage Navs()
        {
            string XML = "<CONSOLE><BR><BR><BR> Obtendo serial do POS.</CONSOLE>";            
            XML += $"<GET TYPE=SERIALNO NAME=_TERMINALSERIAL>";            
            XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/systemcheck TIMEOUT=6>";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(XML, Encoding.UTF8, "application/xml")
            };
        }

        [HttpGet]
        public HttpResponseMessage SystemCheck(string _TERMINALSERIAL)
        {
            string XML = "";
            try
            {
                XML += "<CONSOLE> ----------------------------------------<BR>";
                XML += "        Verificando requisitos do sistema    <BR>";
                XML += "----------------------------------------<BR></CONSOLE>";
                
                modelSetting = settingsdao.Get(_TERMINALSERIAL);
                if (modelSetting == null)
                {
                    XML += $"<CONSOLE><BR> Pos: {_TERMINALSERIAL} não está registrado: <BR><BR> ";
                    XML += $" Efetue o registro nas configuracoes<BR>";
                    XML += " do Menu Principal</CONSOLE>";
                    XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_TERMINALSERIAL}>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssettings/get HOST=h>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

                XML += $"<CONSOLE><BR><BR>Empresa: {modelSetting.Enterprises.FantasyName} Pos: {modelSetting.PosSerial} </CONSOLE>";
                XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={modelSetting.PosSerial}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/start TIMEOUT=6>";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch (Exception err)
            {
                
                XML += $"<CONSOLE> ERRO<BR>";
                XML += "----------------------------------------<BR><BR>";
                XML += $"{err.Message}</CONSOLE>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }            
        }

        [HttpGet]
        public HttpResponseMessage Start(string _SERIALNUMBER)
        {
            //40 colunas
            // até 28 linhas linha 30 é status, 29 é a linha separação            
            string XML = "<init> <CHGCONFNT IDFONT=MODEL4 SIZE=10 COLOR=000 BGCOLOR=TRANSPARENT KEEPBG>";                
            XML += "<console>----------------------------------------<BR>";
            XML += $"  CELTA NAVS     POS serial:{_SERIALNUMBER}  <BR>";
            XML += "----------------------------------------<BR></console>";
            XML += "<CONLOGO NOCLS=1 NAME=CeltaNavs.bmp X=20 Y=29 WAIT_DISPLAY>";
            XML += "<delay time=1>";

            XML += "<RECTANGLE NAME=RETCARD X=53 Y=200 WIDTH=150 HEIGHT=28 VISIBLE=1 COLOR=ccc> ";
            XML += $"<WRITE_AT LINE=12 COLUMN=8>Informe a mesa/pedido</WRITE_AT>";                        
            XML += $"<WRITE_AT LINE=29 COLUMN=1>__________________________________>_____</WRITE_AT>";
            XML += "<GET TYPE=FIELD NAME=OPC LIN=14 COL=7 SIZE=3 >";
            XML += $"<GET TYPE=HIDDEN NAME=_OPCSERIALNUMBER VALUE={_SERIALNUMBER}>";
            XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/getoptioncards TIMEOUT=6>";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(XML, Encoding.UTF8, "application/xml")
            };
        }


        [HttpGet]
        public HttpResponseMessage GetOptionCards(string OPC, string _OPCSERIALNUMBER)
        {
            string XML = "";

            if (String.IsNullOrEmpty(OPC) || OPC == "")
            {
                XML += "<CONSOLE><BR><BR> Aguarde carregando consulta...</CONSOLE>";
                XML += $"<GET TYPE=HIDDEN NAME=_TABLESERIALNUMBER VALUE={_OPCSERIALNUMBER}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navstables/GetOpenTables HOST=h TIMEOUT=10>";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };               
            }
                        
            XML += $"<GET TYPE=HIDDEN NAME=_TABLE VALUE={OPC}>";
            XML += $"<GET TYPE=HIDDEN NAME=_TSERIAL VALUE={_OPCSERIALNUMBER}>";
            XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navssalerequest/get HOST=h TIMEOUT=5>";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(XML, Encoding.UTF8, "application/xml")
            };                                      
        }       
       
    }
}
