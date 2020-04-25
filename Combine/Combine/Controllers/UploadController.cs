using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Combine.Controllers
{
    public class UploadController : Controller
    {
        string auth;
        // GET: Upload
        public ActionResult General()
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
                Session["username"] = "general";
           
                return View();
           
        }
        
        public ActionResult General1()
        {
            var resData = "";
            if (TempData["token"] != null)
            {
                string t = TempData["token"].ToString();
                resData = "https://localhost:44397/Upload/GetFile?filetoken=" + t;
            }
            if (Request.Files.Count > 0)
            {
               
                var file = Request.Files[0];
                string path = Path.Combine(Server.MapPath($"/Uploads/"), Session["username"].ToString());
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(path + @"/" + file.FileName);
                   
                }
                string token = "";
                DateTime sharingDuration = System.DateTime.Now.AddDays(5);
                DateTime fileDuration = System.DateTime.Now.AddDays(5);
                string filename = file.FileName;
                string username = Session["username"].ToString();
                string url = path + @"/" + file.FileName;
                string type = Path.GetExtension(file.FileName);
                double size = file.ContentLength/1000;
                string json = JsonConvert.SerializeObject(new { token,sharingDuration,filename,fileDuration,username,url,type,size});
                HttpClient cli = new HttpClient();
                cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);

                var ts = cli.PostAsync("http://localhost:52357/rssApi/files/upload", new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json"));
                ts.Wait();
                HttpResponseMessage res = ts.Result;
                var read = res.Content.ReadAsStringAsync();
                read.Wait();
                string data = read.Result;
                resData = JsonConvert.DeserializeObject<string>(data);
                resData=resData.ToString();
                TempData["stat"] = res.StatusCode.ToString();
                TempData["token"] = resData;
                TempData.Keep();
            }
            return Json(new { Success=true,resData}, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetFile(string filetoken)
        {
            HttpClient cli = new HttpClient();
            cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);
            var ts = cli.GetAsync("http://localhost:52357/rssApi/files/filetoken?filetoken="+filetoken);
            ts.Wait();
            HttpResponseMessage res = ts.Result;

            var read = res.Content.ReadAsStringAsync();
            read.Wait();
            string data = read.Result;

            var resData = JsonConvert.DeserializeObject<Models.FileModel>(data);
            string filename = resData.fileName;
            string path = resData.url;
            FileContentResult f = DownloadFile(filename, path);
           
            return File(f.FileContents,f.ContentType);
        }

        


        public FileContentResult DownloadFile(string filename, string path)
        {

            
            string ext = Path.GetExtension(filename);
            string mime = null;
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    mime = "image/jpeg";
                    break;
                case ".pdf":
                    mime = "application/pdf";
                    break;
                case ".mp4":
                    mime = "video/mp4";
                    break;
                case ".zip":
                    mime = "application/zip";
                    break;
                case ".xlsx":
                    mime = "text/csv";
                    break;
                case ".docx":
                    mime = "application/vnd.openxmlformats-officedocument.wordprocessing";



                    break;
                default:
                    mime = "html/text";
                    break;


            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            Response.AppendHeader("Content-Disposition", "inline; filename=" + filename);
            return File(fileBytes, mime);
        }

        partial class AuthRes
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }
    }
}