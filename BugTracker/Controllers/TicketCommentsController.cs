using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        public ActionResult Create(int ticketId)
        {
            ViewBag.TicketId = ticketId;
            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Submitter,Developer,ProjectManager")]
        public async Task<ActionResult> Create([Bind(Include = "TicketId,Body,FileUrl")] TicketComment ticketComment, HttpPostedFileBase uploadedFile)
        {
            if (ModelState.IsValid)
            {
                //rule out empty files and file types we don't want
                if (uploadedFile.IsAllowedFileType() == true)
                {
                    if (uploadedFile.ContentType.In("image/bmp", "image/gif", "image/jpeg", "image/png") != true)
                    {
                        string fileName = Path.GetFileName(uploadedFile.FileName);
                        uploadedFile.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                        ticketComment.FileUrl = "/Uploads/" + fileName;
                    }
                    // now we validate images are web-friendly
                    else if ((uploadedFile.ContentType.In("image/bmp", "image/gif", "image/jpeg", "image/png") && (FileUploadValidator.IsWebFriendlyImage(uploadedFile) == true)))
                    {
                        string fileName = Path.GetFileName(uploadedFile.FileName);
                        uploadedFile.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                        ticketComment.FileUrl = "/Uploads/" + fileName;
                    } else ticketComment.FileUrl = null;
                }

                //sent a ticket noti 
                if (ticketComment.Ticket.AssignedUserId != "1712e9df-c37b-402d-9d27-d8568f975b79" && User.Identity.GetUserId() != ticketComment.Ticket.AssignedUserId)
                {
                    NotificationEmailSender nes = new NotificationEmailSender();
                    TicketNotification tktNoti = new TicketNotification();
                    tktNoti.TicketId = ticketComment.TicketId;
                    tktNoti.UserId = ticketComment.Ticket.AssignedUserId;

                    var callbackUrl = Url.Action("Details", "TicketComments", new { id = ticketComment.Id }, protocol: Request.Url.Scheme);
                    string targetEmail = User.Identity.Name;

                    await nes.SendCommentCreateNoti(callbackUrl, targetEmail);

                    db.TicketNotifications.Add(tktNoti);
                }

                ticketComment.Created = DateTimeOffset.Now;
                ticketComment.UserId = User.Identity.GetUserId();
                ticketComment.Closed = false;

                db.TicketComments.Add(ticketComment);
                db.SaveChanges();

                var tkt = db.TicketComments.Include("Ticket").FirstOrDefault(tc => tc.Id == ticketComment.Id);
                return RedirectToAction("Details", "Tickets", new { id = tkt.Ticket.Id });
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Edit/5
        [Authorize(Roles = "Admin,Submitter,Developer,ProjectManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }

            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Submitter,Developer,ProjectManager")]
        public ActionResult Edit([Bind(Include = "Id,TicketId,Body,Created,UserId")] TicketComment ticketComment, HttpPostedFileBase uploadedFile)
        {
            if (ModelState.IsValid)
            {
                //rule out empty files and file types we don't want
                if (uploadedFile.IsAllowedFileType() == true)
                {
                    if (uploadedFile.ContentType.In("image/bmp", "image/gif", "image/jpeg", "image/png") != true)
                    {
                        string fileName = Path.GetFileName(uploadedFile.FileName);
                        uploadedFile.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                        ticketComment.FileUrl = "/Uploads/" + fileName;
                    }
                    // now we validate images are web-friendly
                    else if ((uploadedFile.ContentType.In("image/bmp", "image/gif", "image/jpeg", "image/png") && (FileUploadValidator.IsWebFriendlyImage(uploadedFile) == true)))
                    {
                        string fileName = Path.GetFileName(uploadedFile.FileName);
                        uploadedFile.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                        ticketComment.FileUrl = "/Uploads/" + fileName;
                    } else ticketComment.FileUrl = null;
                }

                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        [Authorize(Roles = "Admin,Submitter,Developer,ProjectManager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [Authorize(Roles = "Admin,Submitter,Developer,ProjectManager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            ticketComment.Closed = true;

            db.Entry(ticketComment).State = EntityState.Modified;
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
