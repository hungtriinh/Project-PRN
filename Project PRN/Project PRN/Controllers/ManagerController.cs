using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_PRN.Controllers {
    public class ManagerController : Controller {
        public ActionResult ContactManager() {
            return View();
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

        public ActionResult ReplyContact()
        {
            return View();
        }
    }
}