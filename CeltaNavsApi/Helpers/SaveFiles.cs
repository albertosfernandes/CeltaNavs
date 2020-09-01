using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CeltaNavsApi.Helpers
{
    public class SaveFiles
    {
        public static void SaveXmlSat(string xmlSat)
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath("~//XmlSat//Envios//");
                string XmlSatEnvioFileName = "XmlSatEnvio" + DateTime.Now.ToString("MMDDhhmm") + ".xml";


                using (var fluxoArquivo = new FileStream(path + XmlSatEnvioFileName, FileMode.Create))
                using (var gravador = new StreamWriter(fluxoArquivo))
                {
                    gravador.WriteLine(xmlSat);
                }

            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static void SaveXmlResponseSat(string xmlSat)
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath("~//XmlSat//Recibos//");
                string XmlSatEnvioFileName = "XmlResponseSat" + DateTime.Now.ToString("MMDDhhmm") + ".xml";


                using (var fluxoArquivo = new FileStream(path + XmlSatEnvioFileName, FileMode.Create))
                using (var gravador = new StreamWriter(fluxoArquivo))
                {
                    gravador.WriteLine(xmlSat);
                }

            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}