using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FintrakBanking.AccessSubsediary
{
    public class SubsediaryParentController : ISubsediaryParentController
    {
        private HttpClient httpClient;
        public SubsediaryParentController()
        {
            this.httpClient = new HttpClient();
          //  this.httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["SubsediaryAPIUrl"] + "api/v1/credit/limitvalidations");
        }

        private string getUrl(string countryCode, string url)
        {
            return "https://fintrakcredit360api2.azurewebsites.net/";
            return ConfigurationManager.AppSettings[$"{countryCode}APIUrl"] + url;


        }

        public async Task<object> post(string countryCode, string url, object body)
        {

            var token = HttpContext.Current.Request.Headers["Authorization"];
            var absoluteURL = getUrl(countryCode, url);
            this.httpClient.DefaultRequestHeaders.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.DefaultRequestHeaders.Add("Authorization", token);
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var responseString = await this.httpClient.PostAsync($"{absoluteURL}{url}", content);
            var result = responseString.Content.ReadAsAsync<object>();

            return result.Result;
        }

        public async Task<object> put(string countryCode, string url, object body)
        {
            var token = HttpContext.Current.Request.Headers["Authorization"];
            var absoluteURL = getUrl(countryCode, url);
            this.httpClient.DefaultRequestHeaders.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.DefaultRequestHeaders.Add("Authorization", token);
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var responseString = await this.httpClient.PutAsync($"{absoluteURL}{url}", content);
            var result = responseString.Content.ReadAsAsync<object>();

            return result.Result;
        }
        public async Task<object> get(string countryCode, string url)
        {
            var token = HttpContext.Current.Request.Headers["Authorization"];
            var absoluteURL = getUrl(countryCode, url);
            this.httpClient.DefaultRequestHeaders.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.DefaultRequestHeaders.Add("Authorization", token);

            var responseString = await this.httpClient.GetAsync($"{absoluteURL}{url}");
            var result = responseString.Content.ReadAsAsync<object>();

            return result.Result;

        }
        public async Task<object> delete(string countryCode, string url)
        {
            var token = HttpContext.Current.Request.Headers["Authorization"];
            var absoluteURL = getUrl(countryCode, url);



            this.httpClient.DefaultRequestHeaders.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.DefaultRequestHeaders.Add("Authorization", token);

            var responseString = await this.httpClient.DeleteAsync($"{absoluteURL}{url}");
            var result = responseString.Content.ReadAsAsync<object>();

            return result.Result;

        }

        public string  getCountryCode()
        {
           string countryCode = HttpContext.Current.Request.Headers["X-COUNTRYCODE"];
            return countryCode;
        }
    }
}
