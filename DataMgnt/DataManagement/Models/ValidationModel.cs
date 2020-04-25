using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataManagement.Models
{
    public class ValidationModel
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string pass { get; set; }
    }
}