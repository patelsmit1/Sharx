using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DataManagement.Models;

namespace DataManagement.Controllers
{
    [RoutePrefix("rssApi/tokens")]
    [Authorize(Roles ="admin")]
    public class TokenController : ApiController
    {
        private RSS_DB_EF db = new RSS_DB_EF();

        // GET: api/Token
        [Route("get/all")]
        public IQueryable<TokenUserModel> GetTokens()
        {
            return db.Tokens;
        }

        // GET: api/Token/5
        [Route("get/{username}")]
        
        public TokenUserModel GetTokenUserModel(string username)
        {
            TokenUserModel tokenUserModel = db.Tokens.Find(username);
            if (tokenUserModel == null)
            {
                return null;
            }

            return tokenUserModel;
        }

        // PUT: api/Token/5
        [HttpPut]
        [Route("update/pass/{username}")]
        public bool UpdatePass(string username,[FromBody] string password)
        {


            TokenUserModel user = db.Tokens.Find(username);
            if (user == null)
                return false;
            user.password = password;
            db.Entry<TokenUserModel>(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            
        }
        [HttpPut]
        [Route("update/type/{username}")]
        public bool updateType(string username, [FromBody] string type)
        {


            TokenUserModel user = db.Tokens.Find(username);
            if (user == null)
                return false;
            user.type = type;
            db.Entry<TokenUserModel>(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }


        }
        [HttpPut]
        [Route("update/username/{username}")]
        public bool updateUsername(string username, [FromBody] string newUsername)
        {


            TokenUserModel user = db.Tokens.Find(username);
            if (user == null)
                return false;
            user.username = newUsername;
            db.Entry<TokenUserModel>(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }


        }
        // POST: api/Token
        [HttpPost]
        [Route("create")]
        
        public TokenUserModel PostTokenUserModel(TokenUserModel tokenUserModel)
        {
            
            db.Tokens.Add(tokenUserModel);

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            return tokenUserModel;
        }

        // DELETE: api/Token/5
        [HttpDelete]
        [Route("delete/{username}")]
        public bool DeleteTokenUserModel(string username)
        {
            TokenUserModel tokenUserModel = db.Tokens.Find(username);
            if (tokenUserModel == null)
            {
                return false;
            }

            db.Tokens.Remove(tokenUserModel);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TokenUserModelExists(string id)
        {
            return db.Tokens.Count(e => e.username == id) > 0;
        }
    }
}