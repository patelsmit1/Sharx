using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataManagement.Models;
using System.Web.Http;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace DataManagement
{
    public class ValidApiUser
    {
        public static bool isValidApiUser(string token)
        {
            /*RSS_DB_EF ctx = new RSS_DB_EF();
            TokenUsersModel user = ctx.Tokens.Find(token);
            if (user != null)
                return true;*/
            return false;
        }
        public static bool SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            
            var verifyurl = "http://localhost:52357/users/verify/" + activationCode;
            var reseturl = "http://localhost:27896/Users/Reset/" + activationCode;



            var fromEmail = new MailAddress("EMAIL_ID", "RSS Platform");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "PASS_OF_MAIL_ID";

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created";

                body = "<br/><br/>We are excited to tell you that your RSM System account is " +
                    " successfully created. Please click on the below link to verify your acccount " +
                    "<br/><br/><button value='Verify'><a href='" + verifyurl + "'></a></button>";

            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + reseturl + ">" + reseturl + "</a>";
            }
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                try
                {
                    smtp.Send(message);
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            return true;
        }
    }
}