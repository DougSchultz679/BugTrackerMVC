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
        Project GetProject(int id);

    }
}
