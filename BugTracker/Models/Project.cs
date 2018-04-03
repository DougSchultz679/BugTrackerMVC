using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { set; get; }
        public string Name { set; get; }
        [AllowHtml]
        public string Description { get; set; }
        public bool Closed { get; set; }

        public virtual ICollection<Ticket> Ticket { get; set; }
        public virtual ICollection<ApplicationUser> ProjectUser { get; set; }

        public Project()
        {
            Ticket = new HashSet<Ticket>();
            ProjectUser = new HashSet<ApplicationUser>();
        }
    }
}