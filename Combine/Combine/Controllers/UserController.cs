using Combine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

namespace Combine.Controllers
{
    public class UserController : Controller
    {
        string auth;

        // GET: User
        public ActionResult Register()
        {


            Session["login"] = "false";
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

        // GET: User/Details/5


        // GET: User/Create
        [HttpPost]
        public ActionResult Register(UserModel user)
        {
            user.usertype = "user";
            user.profilePic = "Not Assigned";
            user.lastPassChange = System.DateTime.Now;
            user.lastloggedin = System.DateTime.Now;
            user.isActive = true;
            user.isVerified = true;
            

            string json = JsonConvert.SerializeObject(user);
            HttpClient cli = new HttpClient();
            cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);

            var ts = cli.PostAsync("http://localhost:52357/rssApi/users/register", new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json"));
            ts.Wait();
            HttpResponseMessage res = ts.Result;
            var read = res.Content.ReadAsStringAsync();
            read.Wait();
            string data = read.Result;
            var resData = JsonConvert.DeserializeObject<string>(data);
            TempData["stat"] = res.StatusCode.ToString();
            if (resData == "Registered Successfully")
            {
                string path = Path.Combine(Server.MapPath($"/Uploads/"),user.username );
                Directory.CreateDirectory(path);
                TempData["status"] = resData;
                return RedirectToAction("Login", "User");
                bool p = Addplanbasic(user.username);
            }
            else if(resData== "Username Already is in Use.Please try with different Username")
            {
                TempData["status"] = resData;
                return RedirectToAction("Register","User");
            }
            else
            {
                TempData["status"] = resData;
                return RedirectToAction("Register", "User");
            }
            
            //TempData["msg"] = res.IsSuccessStatusCode;
            
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
            
            if(ViewBag.stat.ToString()=="true" || ViewBag.stat.ToString() == "True")
            {
                Session["login"] = "true";
                Session["username"] = username;
                return RedirectToAction("GoFolder", "FileManager");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        public ActionResult changepassword()
        {
            
            return View();
        }

        public UserModel GetUser(string uname)
        {
           
                uname = Session["username"].ToString();
                HttpClient cli = new HttpClient();
                cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);
                var ts = cli.GetAsync("http://localhost:52357/rssApi/users/getUsers?uname="+uname);
                ts.Wait();
                HttpResponseMessage res = ts.Result;

                var read = res.Content.ReadAsStringAsync();
                read.Wait();
                string data = read.Result;


                UserModel userModel= JsonConvert.DeserializeObject<UserModel>(data);



            return userModel;
           
        }
        [HttpPost]
        public ActionResult changepassword(string cpass, string npass)
        {

            string username = Session["username"].ToString();
            string json = JsonConvert.SerializeObject(new { username, cpass, npass });
            HttpClient cli = new HttpClient();
            cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);

            var ts = cli.PostAsync("http://localhost:52357/rssApi/users/update/password", new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json"));
            ts.Wait();
            HttpResponseMessage res = ts.Result;

            var read = res.Content.ReadAsStringAsync();
            read.Wait();
            string data = read.Result;
            var resData = JsonConvert.DeserializeObject<Boolean>(data);
            ViewBag.stat = resData.ToString();
            return View();

        }

        public ActionResult ShowPlan()
        {
            IsLogin();
            HttpClient cli = new HttpClient();
                cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);
                var ts = cli.GetAsync("http://localhost:52357/rssApi/plans/all");
                ts.Wait();
                HttpResponseMessage res = ts.Result;

                var read = res.Content.ReadAsStringAsync();
                read.Wait();
                string data = read.Result;

                
                DataTable datatable = JsonConvert.DeserializeObject<DataTable>(data);
            


            return View(datatable);
        }
       
        
        public Boolean Addplanbasic(string username)
        {
            int id = 1;
            string json = JsonConvert.SerializeObject(new { username , id});
            HttpClient cli = new HttpClient();
            cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);

            var ts = cli.PostAsync("http://localhost:52357/rssApi/plans/add", new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json"));
            ts.Wait();
            HttpResponseMessage res = ts.Result;

            var read = res.Content.ReadAsStringAsync();
            read.Wait();
            string data = read.Result;
            var resData = JsonConvert.DeserializeObject<Boolean>(data);


            return resData;
        }
        public ActionResult forgotpassword()
        {
            return View();
        }

        public ActionResult SendOTP(string uname)
        {
            UserModel userModel = GetUser(uname);
            string email = userModel.email;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("ppatelsmit86@gmail.com");
                mail.To.Add(email);
                mail.Subject = "OTP for SHARX";
                Random generator = new Random();
                String r = generator.Next(0, 999999).ToString("D6");
                Session["otp"] = r;
                mail.Body = "Your One Time Password for reseting password is"+r+"Dont Share with Anyone.";

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("ppatelsmit86@gmail.com", "Archana12345");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                
            }
            catch (Exception ex)
            {
               
            }
            return View();

        }

        public void otpvalidate(string otp)
        {
            if(otp!=Session["otp"].ToString())
            {
                RedirectToAction("Login", "User");
            }
            else
            {
                RedirectToAction("changepassword", "User");
            }
        }
        public ActionResult GetPlan()
        {
            IsLogin();
            string uname = Session["username"].ToString();
          
            HttpClient cli = new HttpClient();
            cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);
            var ts = cli.GetAsync("http://localhost:52357/rssApi/plans/"+uname);
            ts.Wait();
            HttpResponseMessage res = ts.Result;

            var read = res.Content.ReadAsStringAsync();
            read.Wait();
            string data = read.Result;
            List<UserPlanModel> userPlans = new List<UserPlanModel>();
            UserModel userModel = GetUser(uname);
         
            userPlans = JsonConvert.DeserializeObject<List<UserPlanModel>>(data);
          
            ViewBag.plans = userPlans;
            ViewBag.user = userModel;
            return View(userPlans);
        }
        // POST: User/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public void IsLogin()
        {
            if(Session["login"].ToString()!="true")
            {
                RedirectToAction("Login", "User");
            }
           
        }

        public ActionResult Logout()
        {
            Session["login"] = "false";
            Session["username"] = "general";
            return RedirectToAction("Login", "User");
        }
        partial class AuthRes
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }
        
        
    }
}
