using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_PRN.Controllers {
    public class HomeController : Controller {
        public ViewResult Index() {

            return View();
        }

        public ViewResult Error() {

            return View();
        }
        public ViewResult Manager() {

            return View();
        }

        public ActionResult Contact() {

            return View();
        }

    }
}