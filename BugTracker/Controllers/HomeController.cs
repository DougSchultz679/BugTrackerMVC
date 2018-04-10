using BugTracker.Models;
using BugTracker.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> ContactSend([Bind(Include = "FromName,FromEmail,Subject,Body,Dummy")]ContactMsg contactMsg)
        {
            if (ModelState.IsValid && String.IsNullOrEmpty(contactMsg.Dummy))
            {
                NotificationEmailSender nes = new NotificationEmailSender();

                await nes.SendContactFormNoti(contactMsg);

                return View("ContactConfirmation");
            } else return View("Index");
        }

        public ActionResult ContactConfirmation()
        {
            return View();
        }
    }
}