using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Helpers
{
    public class UserRolesHelper
    {
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInRole(string UserId, string Role)
        {
            try
            {
                return userManager.IsInRole(UserId, Role);
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool AddUserToRole(string UserId, string Role)
        {
            //var result = new ApplicationUser();
           if (!IsUserInRole(UserId, Role))
                {
                    userManager.AddToRole(UserId, Role);
                    return true;
                }
            return false;
        }

        public bool RemoveUserFromRole(string UserId, string Role)
        {
            try
            {
                var result = userManager.RemoveFromRole(UserId, Role);
                return result.Succeeded;
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public List<ApplicationUser> UsersInRole(string role)
        {
            //return userManager.Users.Select(p => p.Roles.Contains == role);

            List<ApplicationUser> roleUsers = new List<ApplicationUser>();
            List<ApplicationUser> users = userManager.Users.ToList();

            foreach (var u in users)
            {
                if (IsUserInRole(u.Id, role))
                {
                    roleUsers.Add(u);
                }
            }
            return roleUsers;
        }

        public List<ApplicationUser> UsersNotInRole(string role)
        {
            List<ApplicationUser> roleUsers = new List<ApplicationUser>();
            List<ApplicationUser> users = userManager.Users.ToList();

            foreach (var u in users)
            {
                if (!IsUserInRole(u.Id, role))
                {
                    roleUsers.Add(u);
                }
            }
            return roleUsers;
        }

        public IList<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }

        public List<IdentityRole> ListRolesUserNotAssigned(string userId)
        {
            var usrRoles = ListUserRoles(userId);
            var allRoles = db.Roles.ToList();

            for(int i = 0; i < usrRoles.Count(); i++)
            {
                for(var k = 0; k < allRoles.Count(); k++)
                {
                    if (allRoles[i].Name == usrRoles[k])
                    {
                        allRoles.RemoveAt(i);
                    }
                }
            }

            return allRoles;
        }
    }
}