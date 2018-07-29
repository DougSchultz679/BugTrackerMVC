using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class AdminViewProjectDetailsModel
    {
        public IEnumerable<ApplicationUser> UsersOnProject { get; set; }
        public ICollection<string[]> SelectedRolesByUser { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
    }
}