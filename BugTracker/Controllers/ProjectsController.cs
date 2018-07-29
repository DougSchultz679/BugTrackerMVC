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
            try
            {
                AdminViewProjectDetailsModel ViewModel = new AdminViewProjectDetailsModel {
                    Project = _projectService.GetProj(id),
                    ProjectId = id,
                    UsersOnProject = _projectService.GetProjUsers(id),
                    SelectedRolesByUser = _projectService.GetProjUsersRoles(id)
                };

                return View(ViewModel);
            } catch (Exception ex)
            {
                //TODO: Implement logging/security services/ default error page. 

                return HttpNotFound();
            }
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
            try
            {
                if (ModelState.IsValid)
                {
                    _projectService.CreateProj(project);
                    // TODO: handle obverse case
                }
                return RedirectToAction("Index");
            } catch (Exception ex)
            {
                //TODO: send feedback to user??

                return View(project);
            }
            
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit(int? id)
        {
            try
            {
                //TODO: why is the action parameter int id nullable..?
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Project project = _projectService.GetProj((int)id);

                //TODO: is this necessary?
                if (!ModelState.IsValid)
                {
                    return HttpNotFound();
                }
                return View(project);
            } catch (Exception ex)
            {
                //TODO: log, etc
                return HttpNotFound();
            }
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _projectService.EditProj(project);

                    return RedirectToAction("Index");
                }
                // TODO: handle invalid model state with user feedback. 
                return View(project);
            } catch (Exception ex)
            {

                return View(project);
            }
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Delete(int? id)
        {
            try
            {
                //TODO: why is the action parameter int id nullable..?
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Project project = _projectService.GetProj((int)id);

                //TODO: is this necessary?
                if (!ModelState.IsValid)
                {
                    return HttpNotFound();
                }
                return View(project);
            } catch (Exception ex)
            {
                //TODO: log, error page, security service
                return HttpNotFound();
            }
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _projectService.CloseProj(id);
                return RedirectToAction("Index");
            } catch (Exception ex)
            {
                //TODO: tell user what happened
                return View(_projectService.GetProj(id));
            }
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
