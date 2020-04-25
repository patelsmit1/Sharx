using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataManagement.Models
{
    public class PasswordUpdateModel
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string cpass { get; set; }
        [Required]
        public string npass { get; set; }
    }
}