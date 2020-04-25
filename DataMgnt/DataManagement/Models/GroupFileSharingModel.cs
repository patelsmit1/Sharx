using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DataManagement.Models
{
    public class GroupFileSharingModel
    {
        [Key]
        [ForeignKey("group")]
        [Column(Order = 1)]
        [Required]
        public string id { get; set; }
        [Key]
        [ForeignKey("file")]
        [Column(Order = 2)]
        [Required]
        public string token { get; set; }

        public virtual FileModel file { get; set; }
        public virtual GroupModel group { get; set; }
    }
}