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
    public class CartsController : Controller {
        private ProjectPRNEntities3 db = new ProjectPRNEntities3();

        // GET: Carts
        public ActionResult Cart() {
            return View();
        }

        public JsonResult AddToCart(int productID, int quantity) {
            try {
                db.Configuration.ProxyCreationEnabled = false;
                int userID = Int32.Parse(Session["user"].ToString());
                Cart cart = db.Carts.Where(c => c.userid == userID).Where(c => c.productid == productID).FirstOrDefault();
                if (cart == null) {
                    cart = new Cart();
                    cart.userid = userID;
                    cart.productid = productID;
                    cart.quantity = quantity;
                    db.Carts.Add(cart);
                    db.SaveChanges();
                } else {
                    cart.quantity += quantity;
                    db.Entry(cart).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Json("product added successfully!", JsonRequestBehavior.AllowGet);

            } catch {
                return Json("product added fail!", JsonRequestBehavior.AllowGet);
            }


        }

        // GET: Carts/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null) {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Carts/Create
        public ActionResult Create() {
            ViewBag.userid = new SelectList(db.Accounts, "userID", "email");
            ViewBag.productid = new SelectList(db.Products, "productID", "title");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userid,productid,quantity")] Cart cart) {
            if (ModelState.IsValid) {
                db.Carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userid = new SelectList(db.Accounts, "userID", "email", cart.userid);
            ViewBag.productid = new SelectList(db.Products, "productID", "title", cart.productid);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null) {
                return HttpNotFound();
            }
            ViewBag.userid = new SelectList(db.Accounts, "userID", "email", cart.userid);
            ViewBag.productid = new SelectList(db.Products, "productID", "title", cart.productid);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userid,productid,quantity")] Cart cart) {
            if (ModelState.IsValid) {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userid = new SelectList(db.Accounts, "userID", "email", cart.userid);
            ViewBag.productid = new SelectList(db.Products, "productID", "title", cart.productid);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null) {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            Cart cart = db.Carts.Find(id);
            db.Carts.Remove(cart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
