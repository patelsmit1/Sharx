using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Combine.Models;


namespace Combine.Controllers
{
    public class GroupController : Controller
    {
        // GET: Group
        string auth;
        [HttpGet]
        public ActionResult Index()
        {
            Session["login"] = "False";
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
        [HttpPost]
        public ActionResult Index(GroupModel group)
        {
            try
            {
                //Setting owner current user in session
                group.owner = "MMR";

                string json = JsonConvert.SerializeObject(group);
                HttpClient cli = new HttpClient();
                cli.DefaultRequestHeaders.Add("Authorization", "bearer " + (string)Session["auth"]);

                var ts = cli.PostAsync("http://localhost:52357/rssApi/groups/create", new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json"));
                ts.Wait();
                HttpResponseMessage res = ts.Result;
                var tsk = res.Content.ReadAsStringAsync();
                tsk.Wait();
                var id = JsonConvert.DeserializeObject(tsk.Result);
                if (id != null)
                    ViewData["gid"] = id;
                else
                    ViewData["gid"] = "NULL";
                TempData["stat"] = res.StatusCode.ToString();
                TempData["auth"] = TempData["auth"];
                return View();
            }
            catch (Exception e)
            {
                TempData["stat"] = e.Source;
                return View();
            }
        }

        public ActionResult AddMembers()
        {
            GroupMemberCreateModel gc = new GroupMemberCreateModel();
            gc.users = new List<string> {
                "smit",
            };
            gc.owner = "MMR";
            gc.id = "MMRsdp";

            var tsk = ApiRequester.requestToApi(gc, (string)Session["auth"], "groups/add", HttpMethod.Post);
            tsk.Wait();
            var res = tsk.Result; ;
            var reader = res.Content.ReadAsStringAsync();
            reader.Wait();
            var data = reader.Result;
            var id = JsonConvert.DeserializeObject(data);
            if ((bool)id)
                TempData["gid"] = "True";
            else
                TempData["gid"] = "false";
            TempData["stat"] = "Completed";


            return RedirectToAction("Index");
        }
        public ActionResult AckUser()
        {
            bool st = true;
            string username = "smit";
            string id = "MMRsdp";

            var tsk = ApiRequester.requestToApi(id, (string)Session["auth"], "groups/ack/" + username + "/" + st, HttpMethod.Post);
            tsk.Wait();
            var res = tsk.Result; ;
            var reader = res.Content.ReadAsStringAsync();
            reader.Wait();
            var data = reader.Result;
            var stat = JsonConvert.DeserializeObject(data);
            if ((bool)stat)
                TempData["gid"] = "True";
            else
                TempData["gid"] = "false";
            TempData["stat"] = res.StatusCode.ToString();

            return View("Index");
            //return RedirectToAction("Index");
        }
        public ActionResult AddFile()
        {
            string filetoken = "M521R521R520";
            string username = "MMR";
            string id = "MMRFirstGRPSystem.Random";

            var tsk = ApiRequester.requestToApi(id, (string)Session["auth"], "groups/add/file/" + username + "/" + filetoken, HttpMethod.Post);
            tsk.Wait();
            var res = tsk.Result; ;
            var reader = res.Content.ReadAsStringAsync();
            reader.Wait();
            var data = reader.Result;
            var stat = JsonConvert.DeserializeObject(data);
            if ((bool)stat)
                TempData["gid"] = "True";
            else
                TempData["gid"] = "false";
            TempData["stat"] = res.StatusCode.ToString();

            return View("Index");
        }

        public ActionResult GetGroups()
        {
            string username = "MMR";
            bool st = true;
            var tsk = ApiRequester.requestToApi((string)Session["auth"], "groups/get/" + username + "/" + st, HttpMethod.Get);
            tsk.Wait();
            var res = tsk.Result; ;
            var reader = res.Content.ReadAsStringAsync();
            reader.Wait();
            var data = reader.Result;
            var stat = JsonConvert.DeserializeObject(data);

            if (stat == null)
                TempData["stat"] = "NULL";
            else
            {
                foreach (var g in (List<GroupModel>)stat)
                {
                    TempData["gid"] += g.groupName + " ";
                }
            }
            TempData["stat"] = res.StatusCode.ToString();

            return View("Index");
        }

        public ActionResult deleteFile()
        {
            string token = "MM521RR521RR520";
            string id = "MMRFirstGRPSystem.Random";
            string username = "Mayank";

            var tsk = ApiRequester.requestToApi((string)Session["auth"], "groups/remove/file/" + id + "/" + username + "/" + token, HttpMethod.Delete);
            tsk.Wait();
            var res = tsk.Result; ;
            var reader = res.Content.ReadAsStringAsync();
            reader.Wait();
            var data = reader.Result;
            var stat = JsonConvert.DeserializeObject(data);

            if ((bool)stat)
                TempData["gid"] = "True";
            else
                TempData["gid"] = "false";
            TempData["stat"] = res.StatusCode.ToString();
            return View("Index");
        }

        public ActionResult deleteGroup()
        {
            string id = "LAST";
            string owner = "MMR";
            var tsk = ApiRequester.requestToApi((string)Session["auth"], "groups/remove/" + owner + "/" + id, HttpMethod.Delete);
            tsk.Wait();
            var res = tsk.Result; ;
            var reader = res.Content.ReadAsStringAsync();
            reader.Wait();
            var data = reader.Result;
            var stat = JsonConvert.DeserializeObject(data);

            if ((bool)stat)
                TempData["gid"] = "True";
            else
                TempData["gid"] = "false";
            TempData["stat"] = res.StatusCode.ToString();
            return View("Index");
        }
        partial class AuthRes
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }

    }
}