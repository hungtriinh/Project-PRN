using Project_PRN.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Project_PRN.Controllers {
    public class ManagerController : Controller {
        private ProjectPRNEntities3 db = new ProjectPRNEntities3();
        public ActionResult ContactManager() {
            return View();
        }

        public JsonResult ContactJson() {
            db.Configuration.ProxyCreationEnabled = false;
            List<Contact> contacts = db.Contacts.ToList().Select(contact => new Contact {
                userid = contact.userid,
                email = contact.email,
                content = contact.content,
                date = contact.date,
                contactid = contact.contactid,
                subject = contact.subject,
                status = contact.status,
                Account = db.Accounts.Find(contact.userid)
            }).Where(c => c.status == false).ToList();
            return Json(contacts, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AdminBillManager()
        {
            return View();
        }

        public ActionResult StaffBillManager()
        {
            return View();
        }

        public ActionResult BillManager() {
            return View();
        }

        public ActionResult ReplyContact(int? contactId)
        {
            ViewData["contactId"] = contactId;
            return View();
        }

        public JsonResult GetContactByID(int? contactId) {
            db.Configuration.ProxyCreationEnabled = false;
            Contact ct = db.Contacts.ToList().Select(contact => new Contact {
                userid = contact.userid,
                email = contact.email,
                content = contact.content,
                date = contact.date,
                contactid = contact.contactid,
                subject = contact.subject,
                status = contact.status,
                Account = db.Accounts.Find(contact.userid)
            }).Where(c => c.contactid == contactId).FirstOrDefault();
            return Json(ct, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReplyContact(string email, string subject, string content, int? contactId) {
            try {
                if (ModelState.IsValid) {
                    MailAddress senderEmail = new MailAddress("pes2020testing@gmail.com", "BookStore");
                    MailAddress receiverEmail = new MailAddress(email, "Receiver");
                    string password = "pes2020test";
                    string sub = subject;
                    string body = content;
                    SmtpClient smtp = new SmtpClient {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (MailMessage mess = new MailMessage(senderEmail, receiverEmail) {

                        Subject = subject,
                        Body = body
                    }) {
                        mess.IsBodyHtml = true;
                        smtp.Send(mess);
                    }

                    Contact contact = db.Contacts.Find(contactId);
                    contact.status = true;
                    db.Entry(contact).State = EntityState.Modified;
                    db.SaveChanges();
                }

            } catch (Exception) {
                ViewBag.Message = "Some Error";
                return View();
            }
            return RedirectToAction("ContactManager");
        }
    }
}