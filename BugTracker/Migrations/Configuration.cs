namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //Roles
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }

            //Users
            if (!context.Users.Any(r => r.Email == "Vxpx17@gmail.com"))
            {
                userManager.Create(new ApplicationUser {
                    UserName = "Vxpx17@gmail.com",
                    Email = "Vxpx17@gmail.com",
                    FirstName = "Douglas",
                    LastName = "Schultz"
                }, "Qwer!234");
                userManager.AddToRole(userManager.FindByEmail("vxpx17@gmail.com").Id, "Admin");
            }

            if (!context.Users.Any(r => r.Email == "admin@demo.com"))
            {
                userManager.Create(new ApplicationUser {
                    UserName = "admin@demo.com",
                    Email = "admin@demo.com",
                    FirstName = "Ad",
                    LastName = "Min"
                }, "Qwer!234");
                userManager.AddToRole(userManager.FindByEmail("admin@demo.com").Id, "Admin");
            }

            if (!context.Users.Any(r => r.Email == "developer@demo.com"))
            {
                userManager.Create(new ApplicationUser {
                    UserName = "developer@demo.com",
                    Email = "developer@demo.com",
                    FirstName = "Devel",
                    LastName = "Oper"
                }, "Qwer!234");
                userManager.AddToRole(userManager.FindByEmail("developer@demo.com").Id, "Developer");
            }

            if (!context.Users.Any(r => r.Email == "submitter@demo.com"))
            {
                userManager.Create(new ApplicationUser {
                    UserName = "submitter@demo.com",
                    Email = "submitter@demo.com",
                    FirstName = "Sub",
                    LastName = "Mitter"
                }, "Qwer!234");
                userManager.AddToRole(userManager.FindByEmail("submitter@demo.com").Id, "Submitter");
            }

            if (!context.Users.Any(r => r.Email == "manager@demo.com"))
            {
                userManager.Create(new ApplicationUser {
                    UserName = "manager@demo.com",
                    Email = "manager@demo.com",
                    FirstName = "Man",
                    LastName = "Ager"
                }, "Qwer!234");
                userManager.AddToRole(userManager.FindByEmail("manager@demo.com").Id, "ProjectManager");
            }

            //Ticket Statuses            
            if (!context.TicketStatuses.Any(r => r.Name == "Unassigned"))
            {
                context.TicketStatuses.AddOrUpdate(new TicketStatus { Name = "Unassigned" });
            }

            if (!context.TicketStatuses.Any(r => r.Name == "Assigned"))
            {
                context.TicketStatuses.AddOrUpdate(new TicketStatus { Name = "Assigned" });
            }

            if (!context.TicketStatuses.Any(r => r.Name == "Testing"))
            {
                context.TicketStatuses.AddOrUpdate(new TicketStatus { Name = "Testing" });
            }

            if (!context.TicketStatuses.Any(r => r.Name == "Resolved"))
            {
                context.TicketStatuses.AddOrUpdate(new TicketStatus { Name = "Resolved" });
            }

            //Ticket Types
            if (!context.TicketTypes.Any(r => r.Name == "Frontend"))
            {
                context.TicketTypes.AddOrUpdate(new TicketType { Name = "Frontend" });
            }

            if (!context.TicketTypes.Any(r => r.Name == "Server side"))
            {
                context.TicketTypes.AddOrUpdate(new TicketType { Name = "Server side" });
            }

            if (!context.TicketTypes.Any(r => r.Name == "Database"))
            {
                context.TicketTypes.AddOrUpdate(new TicketType { Name = "Database" });
            }

            //Ticket Priorities
            if (!context.TicketPriorities.Any(r => r.Name == "Urgent"))
            {
                context.TicketPriorities.AddOrUpdate(new TicketPriority { Name = "Urgent" });
            }

            if (!context.TicketPriorities.Any(r => r.Name == "This sprint"))
            {
                context.TicketPriorities.AddOrUpdate(new TicketPriority { Name = "This sprint" });
            }

            if (!context.TicketPriorities.Any(r => r.Name == "Future release"))
            {
                context.TicketPriorities.AddOrUpdate(new TicketPriority { Name = "Future release" });
            }

            if (!context.TicketPriorities.Any(r => r.Name == "TBD"))
            {
                context.TicketPriorities.AddOrUpdate(new TicketPriority { Name = "TBD" });
            }
        }
    }
}
