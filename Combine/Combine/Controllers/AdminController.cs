using DataManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Combine.Controllers
{
    public class AdminController : Controller
    {
        string auth;
        // GET: Admin
        public ActionResult Index()
        {
            
            HttpClient cli = new HttpClient();
            cli.DefaultRequestHeaders.Add("Username", "mmr");
            cli.DefaultRequestHeaders.Add("Password", "MMR");
            var ts = cli.PostAsync("http://localhost:52357/token", new StringContent("grant_type=password", encoding: Encoding.UTF8, mediaType: "application/json"));
            ts.Wait();
            HttpResponseMessage res = ts.Result;
            var read = res.Content.ReadAsStringAsync();
            read.Wait();
            string data = read.Result;
            var resData = JsonConvert.DeserializeObject<AuthRes>(data);
            auth = resData.access_token;
            Session["auth"] = auth;

            TempData["stat"] = auth;
            TempData["msg"] = false;
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string pass)
        {
            string json = JsonConvert.SerializeObject(new { username, pass });
            HttpClient cli = new HttpClient();
            cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);

            var ts = cli.PostAsync("http://localhost:52357/rssApi/users/validate", new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json"));
            ts.Wait();
            HttpResponseMessage res = ts.Result;

            var read = res.Content.ReadAsStringAsync();
            read.Wait();
            string data = read.Result;
            var resData = JsonConvert.DeserializeObject<Boolean>(data);
            ViewBag.stat = resData;
            Session["login"] = resData;
            Session["username"] = username;
            Session["type"] = "admin";


            TempData.Keep();

            return View();
        }
        public ActionResult GetUsers()
        {
            IsLogin();
           
                HttpClient cli = new HttpClient();
                cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);
                var ts = cli.GetAsync("http://localhost:52357/rssApi/users/all");
                ts.Wait();
                HttpResponseMessage res = ts.Result;

                var read = res.Content.ReadAsStringAsync();
                read.Wait();
                string data = read.Result;
                
               
                DataTable datatable= JsonConvert.DeserializeObject<DataTable>(data);
                
               

                return View(datatable);
          
        }
        public void IsLogin()
        {
            if (Session["login"].ToString() != "true")
            {
                RedirectToAction("Login", "User");
            }

        }
        partial class AuthRes
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }
        
    }
}