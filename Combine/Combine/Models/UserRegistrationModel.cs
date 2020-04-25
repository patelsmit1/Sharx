using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Combine.Models
{
    public class UserRegistrationModel
    {
        public UserModel user { get; set; }
        public string token { get; set; }
    }
}