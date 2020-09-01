using CeltaNavs.Domain;
using CeltaNavs.Repository;
using CeltaNavsApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;

namespace CeltaNavsApi.Controllers
{
    public class NavsSettingsController : ApiController
    {
        private string navsIp;
        private string navsPort;

        NavsSettingDao settingsdao = new NavsSettingDao();
        EnterpriseDao enterpriseDao = new EnterpriseDao();
        PdvDao pdvDao = new PdvDao();

        public NavsSettingsController()
        {
            navsIp = WebConfigurationManager.AppSettings.Get("NavsIp");
            navsPort = WebConfigurationManager.AppSettings.Get("NavsPort");
        }
        
        [HttpGet]
        public HttpResponseMessage Get(string _SERIALNUMBER)
        {
            string XML = "";

            ModelNavsSetting navsSettings = new ModelNavsSetting();
            navsSettings = settingsdao.Get(_SERIALNUMBER);

            if (navsSettings.PosSerial == null)
            {
                XML += "<CANCEL_KEY TYPE=DISABLE>";
                XML += $"<CONSOLE>----------------------------------------<BR>";
                XML += $"     MENU DE CONFIGURACOES     <BR>";
                XML += "----------------------------------------<BR><BR>";                               
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsSettings/GetEnterprise HOST=h>";
            }

            XML += $"<CONSOLE>----------------------------------------<BR>";
            XML += $"     Este terminal ja esta cadastrado    <BR>";
            XML += "----------------------------------------<BR><BR>";            
            XML += $"--- Pressione uma tecla para continuar! ---</CONSOLE>";
            XML += "<GET TYPE=ANYKEY>";
            XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/navs HOST=h>";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(XML, Encoding.UTF8, "application/xml")
            };
        }

        [HttpGet]
        public HttpResponseMessage Add(string _ENTPERSONCODE, string _PDVNUMBER, string _TERMINALSERIAL)
        {
            string XML = "";
            try
            {
                ModelNavsSetting settings = new ModelNavsSetting();
                int enterpriseId = enterpriseDao.ReturnId(_ENTPERSONCODE);
                int pdvId = pdvDao.ReturnId(Convert.ToInt32(_PDVNUMBER), enterpriseId);
                if (enterpriseId == 0 || pdvId == 0)
                {
                    XML += $"<CONSOLE>Dados invalidos: Empresa:{enterpriseId}, Pdv:{pdvId}<BR><BR>";
                    XML += $"--- Pressione uma tecla para continuar! ---</CONSOLE>";
                    XML += "<GET TYPE=ANYKEY>";
                    XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/navs HOST=h>";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }
                settings.EnterpriseId = enterpriseId;
                settings.PdvId = pdvId;
                settings.PosSerial = _TERMINALSERIAL;
                settingsdao.Add(settings);


                XML += $"<CONSOLE>Configuracoes salvas com sucesso: <BR>";
                XML += $"Empresa: {_ENTPERSONCODE} - PDV: {_PDVNUMBER} <BR> Terminal POS: {_TERMINALSERIAL}";
                XML += "<BR><BR></CONSOLE>";
                XML += "<GET TYPE=ANYKEY>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navscommands/navs HOST=h>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch (Exception err)
            {
                string message = Formatted.FormatError(err.Message);
                XML += $"<CONSOLE>{message}</CONSOLE>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }


        [HttpGet]
        public HttpResponseMessage GetEnterprise()
        {
            string XML = "";
            try
            {
                var listOfEnterprises = enterpriseDao.GetAll();

                if (listOfEnterprises == null || listOfEnterprises.Count < 1)
                {
                    XML += $"<CONSOLE><BR><BR>Nao existe empresa cadastrada <BR>";
                    XML += "----------------------------------------<BR><BR>";
                    XML += "Entre em contato com seu administrador<BR>";
                    XML += "de sistema.</CONSOLE>";

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

                XML += $"<CONSOLE> Selecione sua empresa <BR>";
                XML += "----------------------------------------<BR><BR></CONSOLE>";

                XML += Menu.MenuEnterprises(listOfEnterprises);

                XML += $"<WRITE_AT LINE=29 COLUMN=1>________________________________________</WRITE_AT>";                
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsSettings/GetPdv HOST=h TIMEOUT=5>";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch(Exception err)
            {
                string message = Formatted.FormatError(err.Message);
                XML += $"<CONSOLE> ERRO<BR>";
                XML += "----------------------------------------<BR><BR>";
                XML += $"{message}</CONSOLE>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPdv(string _ENTID)
        {
            string XML = "";
            try
            {
                var enterprise = enterpriseDao.Get(_ENTID);
                XML += $"<CONSOLE> Empresa: {enterprise.PersonalizedCode} <BR>";
                XML += $"Nome: {enterprise.FantasyName}<BR>";
                XML += "----------------------------------------<BR><BR></CONSOLE>";

                var lisOfPdvs = pdvDao.GetAll(_ENTID);
                if (lisOfPdvs == null || lisOfPdvs.Count < 1)
                {
                    XML += $"<CONSOLE><BR><BR>Nao existe pdv cadastrado <BR>";
                    XML += "para esta empresa.<BR>";                    
                    XML += "----------------------------------------<BR><BR>";
                    XML += "Entre em contato com seu administrador<BR>";
                    XML += "de sistema.</CONSOLE>";

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                    };
                }

                XML += Menu.MenuPdvs(lisOfPdvs);

                XML += $"<WRITE_AT LINE=29 COLUMN=1>________________________________________</WRITE_AT>";
                XML += $"<GET TYPE=HIDDEN NAME=ENTERPRISEID VALUE={_ENTID}>";
                XML += $"<GET TYPE=SERIALNO NAME=_TERMINALSERIAL>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsSettings/Register HOST=h TIMEOUT=5>";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch(Exception err)
            {
                string message = Formatted.FormatError(err.Message);
                XML += $"<CONSOLE> ERRO<BR>";
                XML += "----------------------------------------<BR><BR>";
                XML += $"{message}</CONSOLE>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }

        [HttpGet]
        public HttpResponseMessage Register(string _PDVID, string ENTERPRISEID, string _TERMINALSERIAL)
        {
            string XML = "";
            try
            {
                XML += $"<CONSOLE> Registrando ... <BR>";                
                XML += "----------------------------------------<BR><BR></CONSOLE>";

                ModelNavsSetting settings = new ModelNavsSetting();
                settings.EnterpriseId = Convert.ToInt32(ENTERPRISEID);
                settings.PdvId = Convert.ToInt32(_PDVID);
                settings.PosSerial = _TERMINALSERIAL;

                settingsdao.Add(settings);
                pdvDao.MarkInUse(_PDVID, ENTERPRISEID, _TERMINALSERIAL);

                XML += $"<CONSOLE> Registrado com sucesso <BR>";
                XML += "----------------------------------------<BR><BR></CONSOLE>";
                XML += $"<GET TYPE=HIDDEN NAME=_SERIALNUMBER VALUE={_TERMINALSERIAL}>";
                XML += $"<POST RC_NAME=v IP={navsIp} PORT={navsPort} RESOURCE=/api/navsSettings/get HOST=h>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
            catch(Exception err)
            {
                string message = Formatted.FormatError(err.Message);
                XML += $"<CONSOLE> ERRO<BR>";
                XML += "----------------------------------------<BR><BR>";
                XML += $"{message}</CONSOLE>";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(XML, Encoding.UTF8, "application/xml")
                };
            }
        }

    }
}
