using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_PRN.Models;

namespace Project_PRN.Controllers {
    public class BillsController : Controller {
        private ProjectPRNEntities3 db = new ProjectPRNEntities3();

        // GET: Bills
        public ActionResult CheckOut() {
            return View();
        }

        public JsonResult GetLocalJson() {
            string json = "";
            using (StreamReader r = new StreamReader(Path.Combine(Server.MapPath("~/Content/data.json")))) {

                json = r.ReadToEnd();
            }
            Console.WriteLine(json);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        // GET: Bills/Details/5
        public ActionResult Details(long? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill bill = db.Bills.Find(id);
            if (bill == null) {
                return HttpNotFound();
            }
            return View(bill);
        }

        // GET: Bills/Create
        public ActionResult Create() {
            ViewBag.userid = new SelectList(db.Accounts, "userID", "email");
            ViewBag.productid = new SelectList(db.Products, "productID", "title");
            return View();
        }

        // POST: Bills/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BillID,userName,address,phoneNumber,email,orderTime,payment,userid,productid,quantity,status")] Bill bill) {
            if (ModelState.IsValid) {
                db.Bills.Add(bill);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userid = new SelectList(db.Accounts, "userID", "email", bill.userid);
            ViewBag.productid = new SelectList(db.Products, "productID", "title", bill.productid);
            return View(bill);
        }

        // GET: Bills/Edit/5
        public ActionResult Edit(long? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill bill = db.Bills.Find(id);
            if (bill == null) {
                return HttpNotFound();
            }
            ViewBag.userid = new SelectList(db.Accounts, "userID", "email", bill.userid);
            ViewBag.productid = new SelectList(db.Products, "productID", "title", bill.productid);
            return View(bill);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BillID,userName,address,phoneNumber,email,orderTime,payment,userid,productid,quantity,status")] Bill bill) {
            if (ModelState.IsValid) {
                db.Entry(bill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userid = new SelectList(db.Accounts, "userID", "email", bill.userid);
            ViewBag.productid = new SelectList(db.Products, "productID", "title", bill.productid);
            return View(bill);
        }

        // GET: Bills/Delete/5
        public ActionResult Delete(long? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill bill = db.Bills.Find(id);
            if (bill == null) {
                return HttpNotFound();
            }
            return View(bill);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id) {
            Bill bill = db.Bills.Find(id);
            db.Bills.Remove(bill);
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
