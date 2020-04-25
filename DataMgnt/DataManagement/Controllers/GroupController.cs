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
    [RoutePrefix("rssApi/groups")]
    [Authorize]
    public class GroupController : ApiController
    {
        //SqlConnection con = new SqlConnection();
        RSS_DB_EF db = new RSS_DB_EF();

        [HttpPost]
        [Route("create")]
        public string createGroup(GroupModel group)
        {

            UserModel user = db.Users.Find(group.owner);
            if (user == null)
                return null;
            group.id = group.owner + group.groupName;

            group.reqPending = 0;
            db.Groups.Add(group);
            GroupMemberModel gm = new GroupMemberModel();
            gm.id = group.id;
            gm.reqStatus = true;
            gm.username = group.owner;
            db.GroupMembers.Add(gm);
            db.SaveChanges();
            return group.id;
        }

        [HttpPost]
        [Route("add")]
        public Boolean addMembers(GroupMemberCreateModel group)
        {


            /*con.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RSSDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            int[] userid = new int[users.Length];
            try
            {
                using (con)
                {
                    string command = "";
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader rdr;
                    cmd.Connection = con;
                    SqlParameter gid = new SqlParameter("@gid", id);
                    command = "Select id from Group where Id=@gid";
                    cmd.CommandText = command;
                    cmd.Parameters.Add(gid);
                    con.Open();
                    rdr = cmd.ExecuteReader();
                    if (rdr == null)
                    {
                        return false;
                    }
                    command = "Select id from User where username=@uname";
                    cmd.CommandText = command;
                    for (int i = 0; i < users.Length; i++)
                    {
                        SqlParameter uname = new SqlParameter("@uname", users[i]);
                        cmd.Parameters.Add(uname);
                        rdr = cmd.ExecuteReader();
                        if (rdr != null)
                        {
                            userid[i] = (int)rdr["id"];
                            rdr.Close();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    command = "INSERT INTO Group_Member(Id,userid,reqStatus) VALUES(@gid,@uid,0)";
                    cmd.CommandText = command;
                    cmd.Parameters.Add(gid);
                    SqlParameter uid = new SqlParameter();
                    uid.ParameterName = "@uid";
                    for (int i = 0; i < users.Length; i++)
                    {
                        uid.Value = userid[i];
                        cmd.Parameters.Add(uid);
                        if (cmd.ExecuteNonQuery() <= 0)
                        {
                            return false;
                        }
                    }
                    con.Close();
                    return true;

                }
            }
            catch (Exception ex)
            {
                Console.Write("Errors: " + ex.Message);
                return false;
            }*/

            //object[] ob = { group.id };
            GroupModel g = db.Groups.Find(group.id);
            if (g == null)
                return false;
            if (g.owner != group.owner)
                return false;


            List<GroupMemberModel> gms = new List<GroupMemberModel>();

            foreach (string uname in group.users)
            {
                if (db.Users.Find(uname) != null && db.GroupMembers.Find(group.id, uname) == null)
                {
                    GroupMemberModel gm = new GroupMemberModel();
                    gm.id = group.id;
                    gm.username = uname;
                    gm.reqStatus = false;
                    g.reqPending += 1;
                    gms.Add(gm);
                }
            }
            if (db.GroupMembers.Find(group.id, group.owner) == null)
                gms.Add(new GroupMemberModel() { id = group.id, reqStatus = true, username = group.owner });
            db.GroupMembers.AddRange(gms);
            db.Entry<GroupModel>(g).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }

        [HttpPost]
        [Route("ack/{username}/{st}")]
        public Boolean ack([FromBody]string id, bool st, string username)
        {

            /*con.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RSSDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            int reqP = 0, userid;
            try
            {
                using (con)
                {
                    string command = "Select * from Group where Id=@gid";
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader rdr;
                    cmd.CommandText = command;
                    cmd.Connection = con;
                    SqlParameter id = new SqlParameter("@gid", gid);
                    cmd.Parameters.Add(id);
                    con.Open();
                    rdr = cmd.ExecuteReader();
                    if (rdr == null)
                    {
                        return false;
                    }
                    reqP = (int)rdr["reqPending"];
                    if (st == true)
                        reqP--;
                    rdr.Close();
                    command = "Select id from User where username=@uname";
                    SqlParameter uname = new SqlParameter("@uname", username);
                    cmd.Parameters.Add(uname);
                    rdr = cmd.ExecuteReader();
                    if (rdr == null)
                        return false;
                    userid = (int)rdr["id"];
                    rdr.Close();
                    command = "UPDATE Group_Member SET reqStatus = @reqst where Id=@gid and userid=@userid";
                    cmd.CommandText = command;
                    cmd.Parameters.Add(id);
                    SqlParameter reqst = new SqlParameter("@reqst", st);
                    cmd.Parameters.Add(reqst);
                    SqlParameter user = new SqlParameter("@userid", userid);
                    cmd.Parameters.Add(user);
                    if (cmd.ExecuteNonQuery() <= 0)
                        return false;

                    command = "UPDATE Group SET reqPending = @reqp where Id=@gid";
                    cmd.CommandText = command;
                    cmd.Parameters.Add(id);
                    SqlParameter reqp = new SqlParameter("@reqp", reqP);
                    cmd.Parameters.Add(reqp);
                    if (cmd.ExecuteNonQuery() <= 0)
                        return false;
                    con.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("Errors: " + ex.Message);
                return false;
            }*/

            IQueryable<GroupMemberModel> gm;
            //object[] ob = { group.groupId,group.username };
            //db.GroupMembers.Find(ob);

            gm = db.GroupMembers.Where(g => g.id == id && g.username == username);
            if (gm == null)
                return false;
            foreach (GroupMemberModel g in gm)
            {
                g.reqStatus = st;
                db.Entry<GroupMemberModel>(g).State = EntityState.Modified;
            }
            db.SaveChanges();
            return true;


        }

        [HttpPost]
        [Route("add/file/{username}/{filetoken}")]
        public bool addFileToGroup([FromBody]string id, string username, string fileToken)
        {
            object[] c = { id, fileToken };
            GroupFileSharingModel gfs = db.GroupFileSharings.Where(gf => gf.id == id && gf.token == fileToken).FirstOrDefault();
            if (gfs != null)
            {
                return false;
            }
            else
            {
                GroupMemberModel gms = db.GroupMembers.Where(gm => gm.id == id && gm.username == username).FirstOrDefault();
                if (gms == null)
                    return false;
                FileModel file = db.Files.Find(fileToken);
                if (file == null)
                    return false;
                gfs = new GroupFileSharingModel();
                gfs.id = id;
                //gfs.file = file;
                //gfs.group = db.Groups.Find(gId);
                gfs.token = fileToken;

                db.GroupFileSharings.Add(gfs);

                db.SaveChanges();
                return true;

            }

        }

        [HttpDelete]
        [Route("remove/file/{id}/{username}/{filetoken}")]
        public bool deleteFileFromGroup(string id, string username, string fileToken)
        {

            GroupFileSharingModel gfs = db.GroupFileSharings.Where(gf => gf.id == id && gf.token == fileToken).FirstOrDefault();
            if (gfs == null)
                return false;
            FileModel file = db.Files.Find(gfs.token);
            GroupModel group = db.Groups.Find(id);
            if (username != file.username && username != group.owner)
                return false;
            db.GroupFileSharings.Remove(gfs);
            db.SaveChanges();
            return true;


        }

        [HttpDelete]
        [Route("remove/{owner}/{id}")]
        public bool removeGroup(string owner, string id)
        {
            GroupModel group = db.Groups.Find(id);
            if (group == null)
                return false;

            //deleting group files
            if (owner != group.owner)
                return false;
            IQueryable<GroupFileSharingModel> gfs = db.GroupFileSharings.Where(gf => gf.id == id);
            if (gfs != null)
                db.GroupFileSharings.RemoveRange(gfs);

            //deleting grp members
            IQueryable<GroupMemberModel> gms = db.GroupMembers.Where(gm => gm.id == id);
            if (gms != null)
                db.GroupMembers.RemoveRange(gms);
            //deleting grp
            db.Groups.Remove(group);
            db.SaveChanges();
            return true;

        }

        [HttpGet]
        [Route("get/{owner}")]
        public IQueryable<GroupModel> getGroups(string owner)
        {
            var groups = db.Groups.Where(g => g.owner == owner);
            if (groups == null)
                return null;
            return groups;
        }

        [HttpGet]
        [Route("get/{username}/{st}")]
        public List<GroupModel> getGroups(string username, bool st)
        {
            var gms = db.GroupMembers.Where(gm => gm.username == username && gm.reqStatus == st);
            if (gms == null)
                return null;
            List<GroupModel> g = new List<GroupModel>();

            foreach (var gm in gms)
            {
                g.Add(db.Groups.Find(gm.id));
            }
            return g;
        }


    }
}
