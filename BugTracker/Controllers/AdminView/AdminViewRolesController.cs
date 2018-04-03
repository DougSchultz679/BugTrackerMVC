using BugTracker.Models;
using BugTracker.Models.Helpers;
using BugTracker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class AdminViewRolesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            UserRolesHelper helper = new UserRolesHelper();

            var adminModelList = new List<AdminViewRolesModel>();
            IList<string> selected = new List<string>();
            var usersLessInactive = db.Users.Where(u=>u.FullName!="[Leave Unassigned]");

            foreach (var u in usersLessInactive)
            {
                AdminViewRolesModel adminModel = new AdminViewRolesModel {
                    SelectedRoles = helper.ListUserRoles(u.Id).ToArray(),
                    User = u
                };
                           
                adminModelList.Add(adminModel);
            }
            return View(adminModelList);
        }

        //GET: EditUser
        [Authorize(Roles = "Admin")]
        public ActionResult Edit (string id)
        {
            
            var user = db.Users.Find(id);
            UserRolesHelper helper = new UserRolesHelper();
            AdminViewRolesModel adminModel = new AdminViewRolesModel {
                Roles = new MultiSelectList(db.Roles, "Name", "Name", helper.ListUserRoles(id)),
                SelectedRoles = helper.ListUserRoles(user.Id).ToArray(),
                User = user
            };

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(adminModel);
        }

        //POST: EditUser
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminViewRolesModel model)
        {
            var user = db.Users.Find(model.User.Id);
            UserRolesHelper helper = new UserRolesHelper();

            foreach (var rolermv in db.Roles.Select(r => r.Name).ToList()) {
                helper.RemoveUserFromRole(user.Id, rolermv); 
            }

            foreach (var roleadd in model.SelectedRoles) {
                helper.AddUserToRole(user.Id, roleadd);
            }

            return RedirectToAction("Index");
        }
    }
}