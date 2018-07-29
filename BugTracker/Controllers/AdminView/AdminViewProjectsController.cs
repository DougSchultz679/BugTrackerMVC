using BugTracker.Models;
using BugTracker.Models.Helpers;
using BugTracker.Models.ViewModels;
using BugTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers.AdminView
{
    [RequireHttps]
    public class AdminViewProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IProjectService _projService;

        public AdminViewProjectsController(IProjectService ProjService)
        {
            _projService = ProjService ?? throw new ArgumentNullException(nameof(ProjService));
        }

        //GET: Index
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Index(int? id)
        {
            ProjectHelper helper = new ProjectHelper();
            var resultObj = new AdminViewProjectIndexModel {
                UsersOnProject = _projService.GetProjUsers(id.Value),
                UsersOffProject = helper.UsersNotOnProject(id.Value),
                Project = db.Projects.Find(id),
                ProjectId = id.Value
            };
            
            return View(resultObj);
        }

        //POST: AddUserToProject
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult AddUserToProject(string userId, int projId)
        {
            ProjectHelper helper = new ProjectHelper();
            var user = db.Users.Find(userId);
            helper.AddUserToProject(user.Id, projId);

            return RedirectToAction("Index",new {id = projId });
        }

        //POST: RemoveUserFromProject
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult RemoveUserFromProject(string userId, int projId)
        {
            ProjectHelper helper = new ProjectHelper();
            var user = db.Users.Find(userId);
            helper.RemoveUserFromProject(user.Id, projId);

            return RedirectToAction("Index", new { id = projId });
        }
    }
}