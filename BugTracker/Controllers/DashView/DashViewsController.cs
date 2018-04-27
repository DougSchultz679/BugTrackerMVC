using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.Helpers;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using static BugTracker.Models.Helpers.StructLib;

namespace BugTracker.Controllers.DashView
{
    [RequireHttps]
    public class DashViewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Developer")]
        [HttpGet]
        public ActionResult DashDev()
        {
            ProjectHelper pHelper = new ProjectHelper();
            var projs = pHelper.ListUserProjects(User.Identity.GetUserId());
            var userId = User.Identity.GetUserId();

            ICollection<Ticket> allDevTkts = db.Tickets
                .Where(t => t.AssignedUserId == userId)
                .ToList();
            ICollection<Ticket> priorityTkts = db.Tickets
                .Where(t => t.AssignedUserId == userId)
                .Where(t => t.Closed == false)
                .OrderBy(t => t.DueDate)
                .Take(5).ToList();

            DateTimeOffset weekFromToday = DateTimeOffset.Now.AddDays(7);
            DateTimeOffset monthFromToday = DateTimeOffset.Now.AddDays(28);

            //Conduct calculations for the properties that will determine the shape of the doughnut graphs
            int dutw = 0, dotw = 0, dutm = 0, dotm = 0;

            for (int i = 0; i < allDevTkts.Count(); i++)
            {
                var endTime = allDevTkts.ElementAt(i).DueDate;
                if (endTime < weekFromToday)
                {
                    dutw++;
                    if (allDevTkts.ElementAt(i).Closed == true) dotw++;
                }

                if (endTime < monthFromToday)
                {
                    dutm++;
                    if (allDevTkts.ElementAt(i).Closed == true) dotm++;
                }
            }

            var DoneByDueThisWeek = new Single();
            var DoneByDueThisMonth = new Single();
            if (dutw == 0)
            {
                DoneByDueThisWeek = 1;
            } else DoneByDueThisWeek = 100 * (Single)dotw / dutw;

            if (dutm == 0)
            {
                DoneByDueThisMonth = 1;
            } else DoneByDueThisMonth = 100 * (Single)dotm / dutm;

            var allOpenDevTkts = allDevTkts.Where(t => t.Closed == false).ToList();

            var viewModel = new DashDevViewModel() {
                DevUser = db.Users.Find(userId),
                AllDevTkts = allOpenDevTkts,
                PriorityTkts = priorityTkts,
                TktsDueThisWeek = dutw,
                TktsDoneThisWeek = dotw,
                PercentTktsClosedThisWeek = DoneByDueThisWeek,
                TktsDueThisMonth = dutm,
                TktsDoneThisMonth = dotm,
                PercentTktsClosedThisMonth = DoneByDueThisMonth
            };

            return View(viewModel);
        }


        [Authorize(Roles = "Submitter")]
        [HttpGet]
        public ActionResult DashSub()
        {
            ProjectHelper pHelper = new ProjectHelper();
            var projs = pHelper.ListUserProjects(User.Identity.GetUserId());
            var userId = User.Identity.GetUserId();

            ICollection<Ticket> allSubTkts = db.Tickets
                .Where(t => t.OwnerUserId == userId)
                .ToList();
            ICollection<Ticket> priorityTkts = db.Tickets
                .Where(t => t.OwnerUserId == userId)
                .Where(t => t.Closed == false)
                .OrderBy(t => t.DueDate)
                .Take(5).ToList();

            DateTimeOffset weekFromToday = DateTimeOffset.Now.AddDays(7);
            DateTimeOffset monthFromToday = DateTimeOffset.Now.AddDays(28);

            //Conduct calculations for the properties that will determine the shape of the doughnut graphs
            int dutw = 0, dotw = 0, dutm = 0, dotm = 0;

            for (int i = 0; i < allSubTkts.Count(); i++)
            {
                var endTime = allSubTkts.ElementAt(i).DueDate;
                if (endTime < weekFromToday)
                {
                    dutw++;
                    if (allSubTkts.ElementAt(i).Closed == true) dotw++;
                }

                if (endTime < monthFromToday)
                {
                    dutm++;
                    if (allSubTkts.ElementAt(i).Closed == true) dotm++;
                }
            }

            var DoneByDueThisWeek = new Single();
            var DoneByDueThisMonth = new Single();
            if (dutw == 0)
            {
                DoneByDueThisWeek = 1;
            } else DoneByDueThisWeek = 100 * (Single)dotw / dutw;

            if (dutm == 0)
            {
                DoneByDueThisMonth = 1;
            } else DoneByDueThisMonth = 100 * (Single)dotm / dutm;

            var allOpenSubTkts = allSubTkts.Where(t => t.Closed == false).ToList();

            var viewModel = new DashSubViewModel() {
                SubUser = db.Users.Find(userId),
                AllSubTkts = allOpenSubTkts,
                PriorityTkts = priorityTkts,
                TktsDueThisWeek = dutw,
                TktsDoneThisWeek = dotw,
                PercentTktsClosedThisWeek = DoneByDueThisWeek,
                TktsDueThisMonth = dutm,
                TktsDoneThisMonth = dotm,
                PercentTktsClosedThisMonth = DoneByDueThisMonth
            };
            return View(viewModel);
        }

        [Authorize(Roles = "ProjectManager, Admin")]
        [HttpGet]
        public ActionResult DashPM(int? projId)
        {
            ProjectHelper pHelper = new ProjectHelper();
            ICollection<Ticket> pmTkts = new List<Ticket>();
            ICollection<Ticket> priorityTkts = new List<Ticket>();
            var userId = User.Identity.GetUserId();

            var projs = pHelper.ListUserProjectIdsIntArray(userId);

            if (projId.HasValue == false)
            {
                pmTkts = (from a in db.Tickets
                         join b in projs
                         on a.ProjectId equals b
                         select a).ToList();
               priorityTkts = pmTkts
                    .Where(t => t.Closed == false)
                    .OrderBy(t => t.DueDate)
                    .Take(5).ToList();
                ViewBag.Filtered = false;
            } else
            {
                pmTkts = db.Tickets.Where(t => t.ProjectId == projId).ToList();
                priorityTkts = pmTkts.Where(t => t.Closed == false)
                    .OrderBy(t => t.DueDate)
                    .Take(5).ToList();
                ViewBag.Filtered = true;
            }

            DateTimeOffset weekFromToday = DateTimeOffset.Now.AddDays(7);
            DateTimeOffset monthFromToday = DateTimeOffset.Now.AddDays(28);

            //Conduct calculations for the properties that will determine the shape of the doughnut graphs
            int dutw = 0, dotw = 0, dutm = 0, dotm = 0;

            for (int i = 0; i < pmTkts.Count(); i++)
            {
                var endTime = pmTkts.ElementAt(i).DueDate;
                if (endTime < weekFromToday)
                {
                    dutw++;
                    if (pmTkts.ElementAt(i).Closed == true) dotw++;
                }

                if (endTime < monthFromToday)
                {
                    dutm++;
                    if (pmTkts.ElementAt(i).Closed == true) dotm++;
                }
            }

            var DoneByDueThisWeek = new Single();
            var DoneByDueThisMonth = new Single();
            if (dutw == 0)
            {
                DoneByDueThisWeek = 1;
            } else DoneByDueThisWeek = 100 * (Single)dotw / dutw;

            if (dutm == 0)
            {
                DoneByDueThisMonth = 1;
            } else DoneByDueThisMonth = 100 * (Single)dotm / dutm;

            var openPmTkts = pmTkts.Where(t => t.Closed == false).ToList();

            var viewModel = new DashPMViewModel() {
                PMUser = db.Users.Find(userId),
                PmTkts = openPmTkts,
                PriorityTkts = priorityTkts,
                TktsDueThisWeek = dutw,
                TktsDoneThisWeek = dotw,
                PercentTktsClosedThisWeek = DoneByDueThisWeek,
                TktsDueThisMonth = dutm,
                TktsDoneThisMonth = dotm,
                PercentTktsClosedThisMonth = DoneByDueThisMonth
            };

            if (pHelper.ListUserProjectsInStringArray(userId).DoesStringArrayHaveValue() == true)
            {
                ViewBag.projId = new SelectList(db.Projects
                .Where(p => p.ProjectUser
                .Select(pu => pu.Id)
                .Contains(userId)).ToList()
                , "Id", "Name");
            } else
            {
                var standIn = new List<SelectListStandin>();
                standIn.Add(new SelectListStandin {
                    Name = "You have not been assigned to a project!",
                    Id = null
                });

                ViewBag.projId = new SelectList(standIn, "Id", "Name");
            }

            return View(viewModel);
        }
    }
}