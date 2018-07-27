using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Helpers
{
    public class ProjectHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        public Exception AddUserToProject(string userId, int projectId)
        {
            try
            {
                var prj = db.Projects.Find(projectId);
                var usr = db.Users.Find(userId);
                if (!prj.ProjectUser.Contains(usr))
                {
                    prj.ProjectUser.Add(usr);
                    db.SaveChanges();
                }
                return null;
            } catch (Exception ex)
            {
                return ex;
            }
        }

        public Exception RemoveUserFromProject(string userId, int projectId)
        {
            try
            {
                var prj = db.Projects.Find(projectId);
                var usr = db.Users.Find(userId);
                if (prj.ProjectUser.Contains(usr))
                {
                    prj.ProjectUser.Remove(usr);
                    db.SaveChanges();
                }
                return null;
            } catch (Exception ex)
            {
                return ex;
            }
        }

        public IEnumerable<Project> ListUserProjects(string userId)
        {
            return db.Users.Find(userId).Project.ToList();
        }

        public string[] ListUserProjectsInStringArray(string userId)
        {
            var projList = ListUserProjects(userId);
            string[] stringList = new string[projList.Count()];

            for (short i = 0; i < projList.Count(); i++)
            {
                stringList[i] = projList.ElementAt(i).Name;
            }
            return stringList;
        }

        public int[] ListUserProjectIdsIntArray(string userId)
        {
            var projList = ListUserProjects(userId);
            int[] intList = new int[projList.Count()];

            for (short i = 0; i < projList.Count(); i++)
            {
                intList[i] = projList.ElementAt(i).Id;
            }
            return intList;
        }

        public bool IsUserOnProject(string userId, int projId)
        {
            var prj = db.Projects.Find(projId);
            var usr = db.Users.Find(userId);
            if (prj.ProjectUser.Contains(usr))
            return true;
            return false;
        }
        
        public ICollection<ApplicationUser> UsersNotOnProject (int projId)
        {
            Project prj = db.Projects.Find(projId);
            var usersLessInactive = userManager.Users.Where(u => u.FullName != "[Leave Unassigned]").ToList();

            return usersLessInactive
                .Where(u => !u.Project
                .Select(p=>p.Id)
                .Contains(prj.Id)).ToList();
        }
    }
}