using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Combine.Controllers
{
    public class FileManagerController : Controller
    {
        // GET: FileManager
        string auth;
       

        public ActionResult Index()
        {
            
            HttpClient cli = new HttpClient();
            cli.DefaultRequestHeaders.Add("UserName", "mmr");
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


            return View();
        }

        public ActionResult GoFolder(string dpath)
        {
            IsLogin();
            string Id = Session["username"].ToString();
            string path = Path.Combine(Server.MapPath("~/Uploads/"), Id + dpath);
            int pos = path.IndexOf("Uploads");
            string showpath = path.Substring(pos+8);
            var files = Directory.GetFiles(path);
            var folders = Directory.GetDirectories(path);
            List<string> filepaths = new List<string>();
            List<string> filenames = new List<string>();
            List<long> filelengths = new List<long>();
            List<string> fileext = new List<string>();
            List<string> folderpaths = new List<string>();
            List<string> foldernames = new List<string>();


            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                filelengths.Add(fileInfo.Length);
                fileext.Add(fileInfo.Extension);
                filenames.Add(fileInfo.Name);
                filepaths.Add(fileInfo.FullName);
            }

            foreach (var folder in folders)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                foldernames.Add(directoryInfo.Name);
                folderpaths.Add(directoryInfo.FullName);


            }
            string combindedString = string.Join(",", filenames);
            ViewBag.filepaths = filepaths;
            ViewBag.fileexts = fileext;
            ViewBag.filelengths = filelengths;
            ViewBag.filenames = filenames;
            ViewBag.foldernames = foldernames;
            ViewBag.folderpaths = folderpaths;
            ViewBag.dpath = dpath;
            ViewBag.st = combindedString;
            ViewBag.path = path;
            ViewBag.showpath = showpath;
            return View();
        }

        public JsonResult SearchFile(string filename, string dpath)
        {
            string path = Path.Combine(Server.MapPath("~/Uploads/"), Session["username"].ToString() + dpath);


            var files = Directory.GetFiles(path, "*" + filename + "*");

            return Json(files, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUrl(string filename)
        {

            string username = Session["username"].ToString();
            string type = Path.GetExtension(filename);
           
            string json = JsonConvert.SerializeObject(new { username, filename,type });
            HttpClient cli = new HttpClient();
            cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);
            //var ts = cli.PostAsync("http://localhost:52357/rssApi/files/filename", new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json"));

           var ts = cli.GetAsync("http://localhost:52357/rssApi/files/filename?username="+username+"&filename="+filename+"&type="+type);
            ts.Wait();
            HttpResponseMessage res = ts.Result;

            var read = res.Content.ReadAsStringAsync();
            read.Wait();
            string data = read.Result;
            var resData = JsonConvert.DeserializeObject<string>(data);

            if (resData == "File Not Found.")
            {
                ViewBag.url = resData;

            }
            else
            {
                string url = "https://localhost:44397/Upload/GetFile?filetoken=" + resData;
                ViewBag.url = url;
            }
            return View();
        }

        public ActionResult ViewDetails(string filename, string dpath)
        {
            IsLogin();
            string filepath = Path.Combine(Server.MapPath("~/Uploads/"), Session["username"].ToString() + @"\" + dpath + @"\" + filename);
            FileInfo fileInfo = new FileInfo(filepath);
            long l=(fileInfo.Length)/1048576;
            string ext=(fileInfo.Extension);
            int pos = filepath.IndexOf("Uploads");
            string showpath = filepath.Substring(pos + 8);
            DateTime dt = fileInfo.LastWriteTime;
            DateTime ut = fileInfo.CreationTime;
            ViewBag.filename = filename;
            ViewBag.l = l;
            ViewBag.ext = ext;
            ViewBag.showpath = showpath;
            ViewBag.dt = dt;
            ViewBag.ut = ut;
            ViewBag.user = Session["username"].ToString();
           
            return View();
        }
        public ActionResult DownloadFile(string filename, string dpath)
        {
            string currentpath = Path.Combine(Server.MapPath("~/Uploads/"), Session["username"].ToString() + @"\" + dpath);
            string filepath = Path.Combine(Server.MapPath("~/Uploads/"), Session["username"].ToString() + @"\" + dpath + @"\" + filename);


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
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);

            Response.AppendHeader("Content-Disposition", "inline; filename=" + filename);
            return File(fileBytes, mime);
        }

        public void IsLogin()
        {
            if (Session["login"].ToString() != "true")
            {
                RedirectToAction("Login", "User");
            }

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

        partial class AuthRes
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }
    }
}