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

        public ProjectService (ApplicationDbContext DbContext, ProjectHelper ProjHelper)
        {
            _dbContext = DbContext;
            _projHelper = ProjHelper;
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

    }
}