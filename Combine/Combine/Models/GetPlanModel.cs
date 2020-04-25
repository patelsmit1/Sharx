using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Combine.Models
{
    public class GetPlanModel
    {
        [Key]
        [ForeignKey("plan")]
        [Column(Order = 1)]
        public int? id { get; set; }
        [Key]
        [ForeignKey("user")]
        [Column(Order = 2)]
        public string username { get; set; }
        [Required]
        public DateTime subTime { get; set; }
        [Required]
        public DateTime expiryTime { get; set; }
        [Required]
        public Double? storageRemaining { get; set; }
        public int? priority { get; set; }

        [Key]
        public string Username { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string fname { get; set; }
        [Required]
        public string lname { get; set; }
        [Required]
        public DateTime dob { get; set; }
        [Required]
        public string email { get; set; }
        public DateTime lastloggedin { get; set; }
        public DateTime lastPassChange { get; set; }
        public string usertype { get; set; }
        public bool isActive { get; set; }
        public bool isVerified { get; set; }
        public string profilePic { get; set; }

        [Key]
        public int? Id { get; set; }
        [Required]
        public Double? price { get; set; }
        [Required]
        public Double? storageBenefit { get; set; }
        [Required]
        public int? validity { get; set; }
        [Required]
        public string type { get; set; }
        [Required]
        public string description { get; set; }



    }
}