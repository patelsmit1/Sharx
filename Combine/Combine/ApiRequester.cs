using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Combine
{
    public class ApiRequester
    {
        private static string baseUrl = "http://localhost:52357/rssApi/";
        public async static Task<HttpResponseMessage> requestToApi(object data, string auth, string url, HttpMethod method)
        {

            string json = JsonConvert.SerializeObject(data);
            HttpClient cli = new HttpClient();
            cli.DefaultRequestHeaders.Add("Authorization", "bearer " + auth);
            Task<HttpResponseMessage> res;
            if (method==HttpMethod.Put)
                res = cli.PutAsync(baseUrl+url, new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json"));
            else
                res= cli.PostAsync(baseUrl + url, new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json"));
            res.Wait();
            var response = res.Result;
            return response;
        }
        public async static Task<HttpResponseMessage> requestToApi(string auth, string url,HttpMethod method)
        {

            HttpClient cli = new HttpClient();
            cli.DefaultRequestHeaders.Add("Authorization", "bearer " + auth);

            Task<HttpResponseMessage> res;
            if(method==HttpMethod.Delete)
                res = cli.DeleteAsync(baseUrl + url);
            else
                res = cli.GetAsync(baseUrl + url);
            res.Wait();
            var response = res.Result;
            return response;
        }
        



    }
}