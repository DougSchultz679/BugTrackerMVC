using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class AdminViewProjectDetailsModel
    {
        public ICollection<ApplicationUser> UsersOnProject { get; set; }
        public IList<string[]> SelectedRolesByUser { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
    }
}