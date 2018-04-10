using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.Helpers
{
    public class NotificationEmailSender
    {
        public async Task<ActionResult> SendAssignmentNotiMsg (string callbackUrl, string targetEmail)
        {
            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user = new ApplicationUser();

                msg.Body = "You have been assigned a development ticket!" + Environment.NewLine + "Please click the following link to review the details: " + "<a href=\"" + callbackUrl + "\">New Ticket</a>";
                msg.Destination = targetEmail;
                msg.Subject = "Developer Assignment Notification";

                await ems.SendMailAsync(msg);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
            return new EmptyResult();
        }

        public async Task<ActionResult> SendTktChangeLogMsg(string callbackUrl, string targetEmail)
        {
            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user = new ApplicationUser();

                msg.Body = "Your assigned ticket has new changes to it!" + Environment.NewLine + "Please click the following link to review the details: " + "<a href=\"" + callbackUrl + "\">New Ticket</a>";
                msg.Destination = targetEmail;
                msg.Subject = "Ticket Details Update Notification";

                await ems.SendMailAsync(msg);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
            return new EmptyResult();
        }

        public async Task<ActionResult> SendCommentCreateNoti(string callbackUrl, string targetEmail)
        {
            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user = new ApplicationUser();

                msg.Body = "Your assigned ticket has a new comment added to it!" + Environment.NewLine + "Please click the following link to review the details: " + "<a href=\"" + callbackUrl + "\">New Comment</a>";
                msg.Destination = targetEmail;
                msg.Subject = "Ticket Comment Creation Notification";

                await ems.SendMailAsync(msg);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
            return new EmptyResult();
        }

        public async Task<ActionResult> SendContactFormNoti(ContactMsg contactForm)
        {
            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();

                msg.Subject = contactForm.Subject;
                msg.Body = "New contact request from: "+contactForm.FromName+Environment.NewLine+
                    "Contact request email: "+contactForm.FromEmail+ Environment.NewLine + Environment.NewLine +
                    "Message body:"+ Environment.NewLine +contactForm.Body;
                msg.Destination = "vxpx17@gmail.com";

                await ems.SendMailAsync(msg);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
            return new EmptyResult();
        }
    }
}