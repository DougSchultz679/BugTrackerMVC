using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models.Helpers;
using System.Data.Entity;

namespace BugTracker.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ProjectHelper _projHelper;
        private readonly UserRolesHelper _rolesHelper;

        public ProjectService(ApplicationDbContext DbContext, ProjectHelper ProjHelper, UserRolesHelper URolesHelper)
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

        public Project GetProj(int id)
        {
            return _dbContext.Projects.Find(id);
        }

        public ICollection<ApplicationUser> GetProjUsers(int pId)
        {
            return _dbContext.Projects.Find(pId).ProjectUser;
        }

        public ICollection<string[]> GetProjUsersRoles(int pId)
        {
            var users = GetProjUsers(pId);
            ICollection<string[]> result = new List<string[]>();

            foreach (var u in users)
            {
                result.Add(_rolesHelper.ListUserRoles(u.Id).ToArray());
            }

            return result;
        }

        public bool CreateProj(Project proj)
        {
            try
            {
                proj.Closed = false;
                _dbContext.Projects.Add(proj);
                _dbContext.SaveChanges();

                return true;
            } catch
            {
                return false;
            }
        }

        public bool EditProj(Project proj)
        {
            try
            {
                _dbContext.Entry(proj).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return true;
            } catch
            {
                return false;
            }
        }

        public bool CloseProj(int pId)
        {
            try
            {
                Project project = _dbContext.Projects.Find(pId);
                project.Closed = true;
                var prjTkts = _dbContext.Tickets.Where(t => t.ProjectId == project.Id);
                IEnumerable<TicketComment> prjTktComments = new List<TicketComment>();

                foreach (var t in prjTkts)
                {
                    t.Closed = true;
                    t.TicketStatusId = 4;
                    t.Status = _dbContext.TicketStatuses.Find(4);
                    _dbContext.Entry(t).State = EntityState.Modified;

                    prjTktComments = _dbContext.TicketComments.Where(tc => tc.TicketId == t.Id);
                    foreach (var tc in prjTktComments)
                    {
                        tc.Closed = true;
                        _dbContext.Entry(tc).State = EntityState.Modified;
                    }
                }

                _dbContext.Entry(project).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return true;
            } catch (Exception ex)
            {
                return false;
            }
            
        }

    }
}