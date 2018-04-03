using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.ViewModels
{
    public class AdminViewProjectModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Projects { get; set; }
        public string[] SelectedProjects { get; set; }
        public int ProjectId { get; set; }
    }
}