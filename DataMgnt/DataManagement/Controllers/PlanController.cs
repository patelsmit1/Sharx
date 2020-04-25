using DataManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManagement.Controllers
{
    [RoutePrefix("rssApi/plans")]
    [Authorize]
    public class PlanController : ApiController
    {
        RSS_DB_EF db = new RSS_DB_EF();

        [HttpGet]
        [Route("all")]
        public IQueryable<PlanModel> displayPlans()
        {   
                    return db.Plans;
        }

        [HttpGet]
        [Route("{uname}")]
        public List<UserPlanModel> getPlan( [FromUri] string uname)
        {

            List<UserPlanModel> userPlanModels = new List<UserPlanModel>();
            userPlanModels = db.UserPlans.Where(u => u.username == uname).ToList();
            //         return db.UserPlans.Where(u => u.username == uname);
            return userPlanModels;
            
            
        }

        [HttpPost]
        [Route("add")]
        public bool addPlan(AddPlanModel user)
        {
            using (db)
            {
                try
                {
                    PlanModel p = db.Plans.Find(user.id);
                    if (p == null)
                        return false;
                    UserPlanModel up = new UserPlanModel();
                    int? max = 0;
                    foreach (UserPlanModel t in db.UserPlans.Where(u => u.username == user.username))
                    {
                        if (t != null && t.priority > max)
                            max = t.priority;
                    }
                    up.id = user.id;
                    up.username = user.username;
                    up.expiryTime = DateTime.Now.AddDays((double)p.validity);
                    up.storageRemaining = p.storageBenefit;
                    up.subTime = DateTime.Now;
                    up.priority = max+1;
                    db.UserPlans.Add(up);
                    db.SaveChanges();
                    return true;

                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }

      
    }
}
