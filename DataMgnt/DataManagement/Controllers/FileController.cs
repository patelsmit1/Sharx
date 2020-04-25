using DataManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManagement.Controllers
{
    [RoutePrefix("rssApi/files")]
    [Authorize]
    public class FileController : ApiController
    {
        //SqlConnection con = new SqlConnection();
        RSS_DB_EF db = new RSS_DB_EF();
        
        [HttpPost]
        [Route("upload")]
        public String upload([FromBody] FileModel file)
        {
            int flag = 0;
            while (flag == 0)
            {

                file.token = Guid.NewGuid().ToString();
                var files = db.Files.ToList();
                flag = 1;
                foreach (FileModel f in files)
                {
                    if (file.token == f.token)
                    {
                        flag = 0;
                        break;
                    }

                }
            }
            db.Files.Add(file);
            IQueryable<UserPlanModel> up = db.UserPlans.Where(u => u.username == file.username).OrderBy(u => u.priority);
            flag = 0;
            foreach (UserPlanModel u in up)
            {
                if (u.storageRemaining >= file.size)
                {
                    u.storageRemaining -= file.size;
                    db.Entry<UserPlanModel>(u).State = EntityState.Modified;
                    flag = 1;
                    break;
                }
            }
            if (flag == 0)
                return null;
            db.SaveChanges();
            return file.token;
        }
        [HttpGet]
        [Route("filename")]
        public string findFile(string username,string filename,string type)
        {
            var x = db.Files.Where(u => (u.fileName == filename) && (u.type == type) && (u.username == username)).FirstOrDefault();
            try {
                return x.token;
            }
            catch(Exception e)
            {
                return "File Not Found.";
            }
            
        }
        [HttpGet]
        [Route("filetoken")]
        public FileModel findFile(string filetoken)
        {
            
            /*con.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RSSDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            try
            {
                using (con)
                {
                    string command = "Select * from File where token=@token";
                    SqlCommand cmd = new SqlCommand(command, con);
                    SqlParameter tok = new SqlParameter("@token", token);
                    cmd.Parameters.Add(tok);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    FileModel file = new FileModel();
                    if (rdr != null)
                    {
                        file.token = (string)rdr["token"];
                        file.fileName = (string)rdr["fileName"];
                        file.fileDuration = (DateTime)rdr["fileDuration"];
                        file.sharingDuration = (DateTime)rdr["sharingDuration"];
                        file.type = (string)rdr["type"];
                        file.url = (string)rdr["url"];
                        file.username = null;
                        rdr.Close();
                        con.Close();
                        return file; ;
                    }
                    else
                    {
                        rdr.Close();
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.Write("Errors: " + ex.Message);
                return null;
            }*/
            try
            {
                FileModel f;
                f = db.Files.Find(filetoken);
                return f;
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        [HttpGet]
        [Route("all/{username}")]
        public IQueryable<FileModel> getFiles(string username)
        {
            /*List<FileModel> files = new List<FileModel>();
            con.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RSSDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            try
            {
                using (con)
                {
                    string command = "Select id from User where username=@uname";
                    SqlCommand cmd = new SqlCommand(command, con);
                    SqlParameter uname = new SqlParameter("@uname", username);
                    cmd.Parameters.Add(uname);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr == null)
                        return null;
                    command = "Select * from File where userid=@userid";
                    cmd.CommandText = command;
                    SqlParameter userid = new SqlParameter("@userid", (int)rdr["id"]);
                    cmd.Parameters.Add(userid);
                    rdr = cmd.ExecuteReader();
                    if (rdr == null)
                        return null;
                    FileModel file = new FileModel();
                    while(rdr.Read())
                    {
                        file.token = (string)rdr["token"];
                        file.fileName = (string)rdr["fileName"];
                        file.fileDuration = (DateTime)rdr["fileDuration"];
                        file.sharingDuration = (DateTime)rdr["sharingDuration"];
                        file.type = (string)rdr["type"];
                        file.url = (string)rdr["url"];
                        file.username = username;
                        files.Add(file);
                    }
                    rdr.Close();
                    return files;
                }
            }
            catch (Exception ex)
            {
                Console.Write("Errors: " + ex.Message);
                return null;
            }*/
           
            try
            {
                return db.Files.Where(f => f.username == username);
            }catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        [HttpDelete]
        [Route("delete/{filetoken}")]
        public bool deleteFile([FromUri] string filetoken)
        {
            
            using (db)
            {  
                try
                {
                    FileModel file;
                    file = db.Files.Find(filetoken);
                    if (file == null)
                        return false;

                    IQueryable<GroupFileSharingModel> gfs = db.GroupFileSharings.Where(gf => gf.token == filetoken);
                    if(gfs!=null)
                    {
                        db.GroupFileSharings.RemoveRange(gfs);
                    }
                    db.Files.Remove(file);
                    db.SaveChanges();
                    return true;
                }catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }        
    }
}
