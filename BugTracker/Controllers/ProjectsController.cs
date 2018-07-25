using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.Helpers;
using BugTracker.Models.ViewModels;
using BugTracker.Services;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //TODO make interface
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService ProjectService)
        {
            _projectService = ProjectService ?? throw new ArgumentNullException(nameof(ProjectService));
        }

        // GET: Projects
        [Authorize]
        public ActionResult Index(bool? projectUserFilter, bool? includeClosed)
        {
            string userId = User.Identity.GetUserId();

            try
            {
                if (includeClosed == true && projectUserFilter == true)
                {
                    ViewBag.closedIncluded = true;
                    ViewBag.projUserFiltered = true;
                    return View(_projectService.GetOpenUserProjs(userId));
                } else if (projectUserFilter == true)
                {
                    ViewBag.projUserFiltered = true;
                    return View(_projectService.GetUserProjs(userId));
                } else if (includeClosed == true)
                {
                    ViewBag.closedIncluded = true;
                    return View(_projectService.GetOpenProjs());
                } else
                    return View(_projectService.GetAllProjs());
            } catch (Exception ex)
            {
                //TODO: Implement logging/security services/ default error page. 

                return HttpNotFound();
            }          
        }

        // GET: Projects/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            ProjectHelper pHelper = new ProjectHelper();
            UserRolesHelper rHelper = new UserRolesHelper();
            var projectUsers = pHelper.UsersOnProject(id);
            List<string[]> rolesByUser = new List<string[]>();

            foreach (var u in projectUsers)
            {
                rolesByUser.Add(rHelper.ListUserRoles(u.Id).ToArray());
            }

            AdminViewProjectDetailsModel ViewModel = new AdminViewProjectDetailsModel {
                Project = project,
                ProjectId = id,
                UsersOnProject = projectUsers,
                SelectedRolesByUser = rolesByUser
            };

            return View(ViewModel );
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Closed = false;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            project.Closed = true;
            var prjTkts = db.Tickets.Where(t=>t.ProjectId==project.Id);
            IEnumerable<TicketComment> prjTktComments = new List<TicketComment>();

            foreach(var t in prjTkts)
            {
                t.Closed = true;
                t.TicketStatusId = 4;
                t.Status = db.TicketStatuses.Find(4);
                db.Entry(t).State = EntityState.Modified;

                prjTktComments = db.TicketComments.Where(tc => tc.TicketId == t.Id);
                foreach (var tc in prjTktComments)
                {
                    tc.Closed = true;
                    db.Entry(tc).State = EntityState.Modified;
                }
            }

            db.Entry(project).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
