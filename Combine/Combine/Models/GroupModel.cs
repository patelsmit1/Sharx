using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Combine.Models
{
    public class GroupModel
    {
        [Key]
        public string id { get; set; }
        [Required]
        public string groupName { get; set; }

        [ForeignKey("user")]
        [Required]
        public string owner { get; set; }
        public int reqPending { get; set; }

        public virtual UserModel user { get; set; }

        public virtual ICollection<GroupFileSharingModel> groupfiles { get; set; }
        public virtual ICollection<GroupMemberModel> groupmembers { get; set; }
    }
}