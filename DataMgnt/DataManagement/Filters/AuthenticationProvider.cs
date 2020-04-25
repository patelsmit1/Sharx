using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using DataManagement.Models;

namespace DataManagement.Filters
{
    public class AuthenticationProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            RSS_DB_EF db = new RSS_DB_EF();

            string uname = context.Request.Headers.Get("Username");
            string pass = context.Request.Headers.Get("Password");
            if(uname==null||pass==null)
            {
                context.SetError("invalid_grant", "Sorry...Credentials are incorrect");
                return;
            }
            TokenUserModel user = db.Tokens.Find(uname);
            if(user!=null&& pass==user.password)
            {
                if(user.type=="admin")
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                }
                else
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                }
                identity.AddClaim(new Claim("username", user.username));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.name));
                context.Validated(identity);
            }
            /*if(context.UserName=="mmr"&&context.Password=="MMR")
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                identity.AddClaim(new Claim("username", "mmr"));
                identity.AddClaim(new Claim(ClaimTypes.Name, "Mayank"));
                context.Validated(identity);
            }*/
            else
            {
                context.SetError("invalid_grant", "Sorry...Credentials are incorrect");
                return;
            }

        }
    }
}