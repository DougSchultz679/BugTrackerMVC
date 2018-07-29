using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public interface IProjectService
    {
        IEnumerable<Project> GetAllProjs();
        IEnumerable<Project> GetOpenProjs();
        IEnumerable<Project> GetUserProjs(string uId);
        IEnumerable<Project> GetOpenUserProjs(string uId);
        Project GetProj(int id);
        bool CreateProj(Project proj);
        bool EditProj(Project proj);
        bool CloseProj(int pId);

        ICollection<ApplicationUser> GetProjUsers(int pId);
        ICollection<string[]> GetProjUsersRoles(int pId);

    }
}
