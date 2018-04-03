using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class AdminViewProjectIndexModel
    {
        public ICollection<ApplicationUser> UsersOnProject { get; set; }
        public ICollection<ApplicationUser> UsersOffProject { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
    }
}