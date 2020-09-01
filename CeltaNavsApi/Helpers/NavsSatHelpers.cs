using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CeltaNavsApi.Helpers
{
    public class NavsSatHelpers
    {
        public static string SendSaleMovement(string xmlSale, string status, ModelNavsSetting settings)
        {
            string result = String.Empty;
            status = "";

            if (!ConsultSATOperacionalStatus(settings.SatAddressSharePdv, settings.SatPortSharePdv))
            {
                status = "SAT OFF-LINE";
                return string.Empty;
            }
            else
            {
                var service = new Service.CeltaSATService();
                service.Timeout = 10000;
                //service.Url = CeltaBSCrossService.GetCompleteUrl(uriSatService, isActive ? CeltaPdvGlobal.Pdv.Parameters.SATUrlSharing : CeltaPdvGlobal.Pdv.Parameters.ConcentratorParameters.UrlSATSharing);
                service.Url = $"http://{settings.SatAddressSharePdv}:{settings.SatPortSharePdv}/sat/CeltaSATService.asmx";
                result = service.SendSaleMovement(xmlSale, out status);
                return result;
            }

        }

        public static bool ConsultSATOperacionalStatus(string satAddress, string satPort)
        {
            Service.CeltaSATService service = new Service.CeltaSATService();
            service.Timeout = 3000;
            service.Url = $"http://{satAddress}:{satPort}/sat/CeltaSATService.asmx";
            var result = service.ConsultOperacionalStatus();

            if (result.ToUpperInvariant().Contains("RESPOSTA COM SUCESSO") ||
                   result.ToUpperInvariant() == "OK" ||
                   result.ToUpperInvariant() == "SAT OPERACIONAL")
                return true;
            else
                return false;
        }

    }
}