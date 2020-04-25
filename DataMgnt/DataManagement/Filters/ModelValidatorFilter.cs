using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DataManagement.Filters
{
    public class ModelValidatorFilter:ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //StreamWriter sw = new StreamWriter("Log.txt", true, encoding: Encoding.UTF8);
            try
            {
                //File.AppendAllText("Log.txt", actionContext.ActionDescriptor.ControllerDescriptor.ControllerName + " is Filtered\n");

                if (!actionContext.ModelState.IsValid)
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}