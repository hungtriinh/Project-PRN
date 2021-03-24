using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_PRN.Models;

namespace Project_PRN.Controllers {
    public class AccountsController : Controller {
        private ProjectPRNEntities3 db = new ProjectPRNEntities3();

        // GET: Accounts
        public ActionResult SignIn() {
            if (Session["user"] == null) {
                return View();
            } else {
                return RedirectToRoute(new {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "email,password")] Account account) {
            db.Configuration.ProxyCreationEnabled = false;
            if (ModelState.IsValid) {
                string checkEmail = account.email;
                string checkPassword = account.password;

                //get user's information from database
                Account checkAccount = db.Accounts.Where(a => a.email.Equals(checkEmail)).FirstOrDefault();
                //check is exsisted account
                if (checkAccount != null) {
                    //check if password matches
                    if (BCrypt.Net.BCrypt.Verify(checkPassword, checkAccount.password)) {
                        HttpSessionStateBase session = HttpContext.Session;
                        //add user to session
                        session.Add("user", checkAccount.userID);
                        session.Add("role", checkAccount.role);
                        //reload cart
                        if (Request.Cookies["cart"] != null) {
                            string cartJson = Request.Cookies["cart"].Value;
                            CartsController cartsController = new CartsController();
                            int userId = Int32.Parse(Session["user"].ToString());
                            cartsController.AddToCartWhenLogin(cartJson, userId);
                            Response.Cookies["cart"].Expires = DateTime.Now.AddDays(-1);
                        }
                        return RedirectToRoute(new {
                            controller = "Home",
                            action = "Index",
                            id = UrlParameter.Optional
                        });
                    } else {
                        ViewBag.Message = "Wrong Password!";
                    }
                } else {
                    ViewBag.Message = "Not exsisted account!";
                }
            }
            return View();
        }

        public ViewResult SignUp() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp([Bind(Include = "userID,email,password,userName,role,address,phoneNumber")] Account account) {
            try {
                db.Configuration.ProxyCreationEnabled = false;
                if (ModelState.IsValid) {
                    List<Account> list = db.Accounts.Where(a => a.email.Equals(account.email)).ToList();
                    if (list.Count == 0) {
                        account.role = 2;
                        string pass = account.password;
                        int cost = 12;
                        string newPassword = BCrypt.Net.BCrypt.HashPassword(pass, cost);
                        account.password = newPassword;
                        Console.WriteLine($"{account.userID} - {account.userName} - {account.password} - {account.phoneNumber} - {account.role} - {account.address}");
                        db.Accounts.Add(account);
                        db.SaveChanges();
                        return RedirectToAction("SignIn");
                    } else {
                        ViewBag.Message = "Exsisted Account! Please register again";
                    }
                }
                return View();
            } catch (Exception e) {
                return RedirectToRoute(new {
                    controller = "Home",
                    action = "Error",
                    id = UrlParameter.Optional
                });
            }

        }

        public ActionResult SignOut() {
            try {
                if (Session["user"] != null) {
                    HttpSessionStateBase session = HttpContext.Session;
                    session.Remove("user");
                }
            } catch (Exception e) {
                //chuyen toi trang bao loi
            }
            return RedirectToRoute(new {
                controller = "Home",
                action = "Index",
                id = UrlParameter.Optional
            });
        }

        public ActionResult Edit() {
            if (Session["user"] != null) {
                return View();
            } else {
                return RedirectToAction("SignIn");
            }
        }

        public JsonResult EditJson() {
            db.Configuration.ProxyCreationEnabled = false;
            if (Session["user"] != null) {
                var userId = Int32.Parse(Session["user"].ToString());
                var infor = db.Accounts.Where(a => a.userID == userId).ToList();
                return Json(infor, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "password,userName,address,phoneNumber")] Account account) {
            if (ModelState.IsValid) {
                db.Configuration.ProxyCreationEnabled = false;
                string checkPassword = account.password;
                var userId = Int32.Parse(Session["user"].ToString());
                List<Account> list = db.Accounts.Where(a => a.userID == userId).ToList();
                if (BCrypt.Net.BCrypt.Verify(checkPassword, list[0].password)) {
                    int cost = 12;
                    string newPassword = BCrypt.Net.BCrypt.HashPassword(checkPassword, cost);
                    Account accountUpdated = db.Accounts.Find(userId);
                    accountUpdated.password = newPassword;
                    accountUpdated.userName = account.userName;
                    accountUpdated.address = account.address;
                    accountUpdated.phoneNumber = account.phoneNumber;
                    db.Entry(accountUpdated).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToRoute(new {
                        controller = "Home",
                        action = "Index",
                        id = UrlParameter.Optional
                    });
                } else {
                    return RedirectToAction("Edit");
                }
            }
            return View(account);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
