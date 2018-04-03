using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Authorize]
        public ActionResult Index(int? filterState, bool? includeClosed)
        {
            IEnumerable<Ticket> tickets = new List<Ticket>();
            if (includeClosed == true)
            {
                tickets = db.Tickets
                .Include(t => t.AssignedUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Priority)
                .Include(t => t.Project)
                .Include(t => t.Status)
                .Include(t => t.Type)
                .ToList();
                ViewBag.includeClosedTkts = true;
            } else
            {
                tickets = db.Tickets
                .Include(t => t.AssignedUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Priority)
                .Include(t => t.Project)
                .Include(t => t.Status)
                .Include(t => t.Type)
                .ToList()
                .Where(t => t.Closed == false);
                ViewBag.includeClosedTkts = false;
            }

            var userid = User.Identity.GetUserId();

            switch (filterState)
            {
                //DEFAULTS. All tickets shown
                case null:
                    ViewBag.Filtered = false;
                    return View(tickets);
                case 0:
                    ViewBag.Filtered = false;
                    return View(tickets);
                //PROJECT FILTER. only tickets that belong to projects user is assigned to are returned
                case 1:
                    ViewBag.Filtered = true;
                    ProjectHelper helper = new ProjectHelper();
                    var projs = helper.ListUserProjects(User.Identity.GetUserId());
                    return View(tickets.Where(t => projs.Select(p => p.Id).Contains(t.ProjectId)));
                //DEV FILTER. only tickets user assigned to are returned
                case 2:
                    ViewBag.Filtered = true;
                    return View(tickets.Where(t => t.AssignedUserId == userid));
                //SUBMITTER FILTER. only tickets user owns are returned
                case 3:
                    ViewBag.Filtered = true;
                    return View(tickets.Where(t => t.OwnerUserId == userid));
            }
            return HttpNotFound();
        }

        // GET: Tickets/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            TicketPriority tTyp = db.TicketPriorities.Find(4);
            var userid = User.Identity.GetUserId();

            //Project List is limited to projects the submitter is assigned to. 
            ViewBag.ProjectId = new SelectList(db.Projects
                .Where(p=>p.ProjectUser
                .Select(pu=>pu.Id)
                .Contains(userid)).ToList()
                , "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", 1);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", 4);

            Ticket model = new Ticket();
            model.TicketPriorityId = 4;
            return View(model);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create([Bind(Include = "Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = DateTimeOffset.Now;
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.AssignedUserId = "1712e9df-c37b-402d-9d27-d8568f975b79";
                ticket.TicketStatusId = 1;
                ticket.Closed = false;

                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles ="Admin,Submitter,Developer,ProjectManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();

            ViewBag.AssignedUserId = new SelectList(db.Users
                .Where(u=>u.Project
                .Select(p=>p.Id)
                .Contains(ticket.ProjectId))
                .ToList(), "Id", "FullName", ticket.AssignedUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            if (User.IsInRole("Admin"))
            {
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            } else {
                ViewBag.ProjectId = new SelectList(db.Projects
                                .Where(p => p.ProjectUser
                                .Select(pu => pu.Id)
                                .Contains(userId)).ToList()
                                , "Id", "Name", ticket.ProjectId);
            }
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Submitter,Developer,ProjectManager")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Created,DueDate,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var oldTkt = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                ticket.AssignedUser = db.Users.Find(ticket.AssignedUserId);

                //ticket notification sending via email for assigned developers
                if (oldTkt.AssignedUserId != ticket.AssignedUserId)
                {
                    ticket.AssignedUser = db.Users.Find(ticket.AssignedUserId);
                    if (ticket.AssignedUserId != "1712e9df-c37b-402d-9d27-d8568f975b79" ||
                    ticket.AssignedUserId != null)
                    {
                        NotificationEmailSender nes = new NotificationEmailSender();
                        TicketNotification tktNoti = new TicketNotification();
                        tktNoti.TicketId = ticket.Id;
                        tktNoti.UserId = ticket.AssignedUserId;

                        var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);
                        string targetEmail = User.Identity.Name;
                        await nes.SendAssignmentNotiMsg(callbackUrl, targetEmail);

                        db.TicketNotifications.Add(tktNoti);
                    }
                }

                //history object creation
                foreach (var prop in typeof(Ticket).GetProperties())
                {
                    if (prop.Name != null && prop.Name.In("AssignedToUser", "OwnerUser", "Status", "Priority", "Type", "Project", "Title", "Description","DueDate"))
                    {
                        string oldval = "", newval ="";
                         
                        switch (prop.Name)
                        {
                            default:
                            if (oldTkt.GetType().GetProperty(prop.Name).GetValue(oldTkt)!=null) oldval = oldTkt.GetType().GetProperty(prop.Name).GetValue(oldTkt).ToString();
                                newval = ticket.GetType().GetProperty(prop.Name).GetValue(ticket).ToString();
                                break;
                            case "AssignedToUser":
                                ticket.AssignedUser = db.Users.Find(ticket.AssignedUserId);
                                oldval = oldTkt.AssignedUser.FullName;
                                newval = ticket.AssignedUser.FullName;
                                break;
                            case "OwnerUser":
                                ticket.OwnerUser = db.Users.Find(ticket.OwnerUserId);
                                oldval = oldTkt.OwnerUser.FullName;
                                newval = ticket.OwnerUser.FullName;
                                break;
                            case "Status":
                                ticket.Status = db.TicketStatuses.Find(ticket.TicketStatusId);
                                oldval = oldTkt.Status.Name;
                                newval = ticket.Status.Name;
                                break;
                            case "Priority":
                                ticket.Priority = db.TicketPriorities.Find(ticket.TicketPriorityId);
                                oldval = oldTkt.Priority.Name;
                                newval = ticket.Priority.Name;
                                break;
                            case "Type":
                                ticket.Type = db.TicketTypes.Find(ticket.TicketTypeId);
                                oldval = oldTkt.Type.Name;
                                newval = ticket.Type.Name;
                                break;
                            case "Project":
                                ticket.Project = db.Projects.Find(ticket.ProjectId);
                                oldval = oldTkt.Project.Name;
                                newval = ticket.Project.Name;
                                break;
                        }
                                              
                        if (oldval != newval)
                        {
                            var ticketHistory = new TicketHistory() {
                                TicketId = ticket.Id,
                                UserId = User.Identity.GetUserId(),
                                Property = prop.Name,
                                OldValue = oldval,
                                NewValue = newval,
                                Changed = DateTime.Now
                            };

                            db.TicketHistories.Add(ticketHistory);
                        }
                    }
                }

                if (ticket.AssignedUserId == null)
                {
                    ticket.AssignedUserId = "1712e9df-c37b-402d-9d27-d8568f975b79";
                }

                //Send a change notification to the assigned developer as long as the modifier is another submitter, admin or PM
                if (ticket.AssignedUserId != "1712e9df-c37b-402d-9d27-d8568f975b79" && User.Identity.GetUserId() != ticket.AssignedUserId)
                {
                    NotificationEmailSender nes = new NotificationEmailSender();
                    TicketNotification tktNoti = new TicketNotification();
                    tktNoti.TicketId = ticket.Id;
                    tktNoti.UserId = ticket.AssignedUserId;

                    var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);
                    string targetEmail = User.Identity.Name;

                    await nes.SendTktChangeLogMsg(callbackUrl, targetEmail);

                    db.TicketNotifications.Add(tktNoti);
                }

                ticket.Updated = DateTimeOffset.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
            }

            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return RedirectToAction("Index");
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles ="Admin,ProjectManager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,ProjectManager")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            ticket.Closed = true;
            ticket.TicketStatusId = 4;
            ticket.Status = db.TicketStatuses.Find(4);

            var prjTktComments = db.TicketComments.Where(tc => tc.TicketId == ticket.Id);
            foreach (var tc in prjTktComments)
            {
                tc.Closed = true;
                db.Entry(tc).State = EntityState.Modified;
            }

            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
