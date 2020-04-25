using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataManagement.Models
{
    public class TokenUserModel
    {
        [Key]
        public String username { get; set; }
        [Required]
        public String password { get; set; }
        [Required]
        public String name { get; set; }
        [Required]
        public String type { get; set; }
    }
}