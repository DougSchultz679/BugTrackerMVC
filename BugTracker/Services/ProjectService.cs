using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models.Helpers;


namespace BugTracker.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ProjectHelper _projHelper;
        private readonly UserRolesHelper _rolesHelper;

        public ProjectService (ApplicationDbContext DbContext, ProjectHelper ProjHelper, UserRolesHelper URolesHelper)
        {
            _dbContext = DbContext;
            _projHelper = ProjHelper;
            _rolesHelper = URolesHelper;
        }

        public IEnumerable<Project> GetAllProjs()
        {
            return _dbContext.Projects.ToList();
        }

        public IEnumerable<Project> GetOpenProjs()
        {
            return this.GetAllProjs().Where(p => p.Closed == false);
        }

        public IEnumerable<Project> GetUserProjs(string uId)
        {
            return GetAllProjs().Where(p => p.ProjectUser
                    .Select(f => f.Id)
                    .Contains(uId));
        }

        public IEnumerable<Project> GetOpenUserProjs(string uId)
        {
            return GetAllProjs().Where(p => p.ProjectUser
                    .Select(f => f.Id)
                    .Contains(uId))
                    .Where(p => p.Closed == false);
        }

        public Project GetProject(int id)
        {
            return _dbContext.Projects.Find(id);
        }

        public IEnumerable<ApplicationUser> GetProjectUsers (int pId)
        {
             return _dbContext.Projects.Find(pId).ProjectUser;
        }

        public ICollection<string[]> GetProjectUsersRoles(int pId)
        {
            var users = GetProjectUsers(pId);
            ICollection<string[]> result = new List<string[]>();

            foreach (var u in users)
            {
                result.Add(_rolesHelper.ListUserRoles(u.Id).ToArray());
            }

            return result;
        }
    }
}