using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ContactMsg
    {
        [Required, Display(Name ="Name")]
        public string FromName { get; set; }
        [Required, Display(Name ="Email"), EmailAddress]
        public string FromEmail { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required, Display(Name = "Message Body")]
        [StringLength(500, ErrorMessage = "The {0} can be at most {1} characters long.")]
        public string Body { get; set; }
        public string Dummy { get; set; }
    }
}