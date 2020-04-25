using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using DataManagement.Filters;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;

[assembly: OwinStartup(typeof(DataManagement.OwinStartUp))]

namespace DataManagement
{
    public class OwinStartUp
    {
        public void Configuration(IAppBuilder app)
        {
            //enables cross origin requests
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            //Comfigure OAuthAuthorization server
            var provider = new AuthenticationProvider();

            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = provider
            };
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
