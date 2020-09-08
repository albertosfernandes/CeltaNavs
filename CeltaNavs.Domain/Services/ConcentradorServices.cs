using CeltaWare.CBS.PDV.Concentrator.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;

namespace CeltaNavsApi.Services
{
    public class ConcentradorServices
    {
        private HttpClient _httpClient = null;
        private AuthenticationHeaderValue _userLoginDefault = new AuthenticationHeaderValue("Basic", "MASTERPDV:PASSWORD");
        

        public ConcentradorServices(CeltaNavs.Repository.ModelNavsSetting settings)
        {
            string url = $"http://{settings.ConcentratorAddress}:{settings.ConcentratorPort}";
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(url);
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Authorization = _userLoginDefault;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public int ExportSaleMovement(ModelSaleMovement bsSaleMovement)
        {
            var content = new ObjectContent<ModelSaleMovement>(bsSaleMovement, new JsonMediaTypeFormatter());
            HttpResponseMessage response = null;
            try
            {
                response = _httpClient.PostAsync("api/APISaleMovement/Insert", content).Result;
                if (response.IsSuccessStatusCode)
                return 0;
                else
                {
                    return -2;
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }        
    }
}