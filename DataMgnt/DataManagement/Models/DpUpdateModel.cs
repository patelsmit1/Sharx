using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DataManagement.Models
{
    public class DpUpdateModel
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string url { get; set; }
    }
}