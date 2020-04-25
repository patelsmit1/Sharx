using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Combine.Models
{
    public class GroupMemberCreateModel
    {
        [Required]
        public List<string> users { get; set; }
        
        [Required]
        public string owner { get; set; }
        
        [Required]
        public string id { get; set; }
    }
}