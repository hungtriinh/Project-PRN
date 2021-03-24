using Project_PRN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_PRN.Controllers
{
    public class HomeController : Controller
    {
        private ProjectPRNEntities3 db = new ProjectPRNEntities3();
        public ViewResult Index()
        {

            return View();
        }

        public ViewResult Error()
        {

            return View();
        }
        public ViewResult Manager()
        {

            return View();
        }

        public ActionResult Contact()
        {

            if (Session["user"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToRoute(new
                {
                    controller = "Accounts",
                    action = "SignIn",
                    id = UrlParameter.Optional
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContact([Bind(Include = "userid, username, email, content, date, contactid")] Contact contact)
        {
            db.Configuration.ProxyCreationEnabled = false;
            
            if (ModelState.IsValid)
            {
                var userId = Int32.Parse(Session["user"].ToString());
                Account currAccount = db.Accounts.Find(userId);
                contact.userid = userId;
                contact.username = currAccount.userName;
                contact.date = DateTime.Now;
                contact.Account = currAccount;
                db.Contacts.Add(contact);
                db.SaveChanges();
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                });
            }
            return RedirectToAction("Contact");
        }
    }
}