using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_PRN.Models;

namespace Project_PRN.Controllers
{
    public class EvaluatesController : Controller
    {
        private ProjectPRNEntities3 db = new ProjectPRNEntities3();

        // GET: Evaluates
        public ActionResult Evaluate()
        {
            return View();
        }

        // GET: Evaluates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluate evaluate = db.Evaluates.Find(id);
            if (evaluate == null)
            {
                return HttpNotFound();
            }
            return View(evaluate);
        }

        // GET: Evaluates/Create
        public ActionResult Create()
        {
            ViewBag.userID = new SelectList(db.Accounts, "userID", "email");
            ViewBag.productID = new SelectList(db.Products, "productID", "title");
            return View();
        }

        // POST: Evaluates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "evaluateID,evaluateContent,rate,date,userID,productID")] Evaluate evaluate)
        {
            if (ModelState.IsValid)
            {
                db.Evaluates.Add(evaluate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userID = new SelectList(db.Accounts, "userID", "email", evaluate.userID);
            ViewBag.productID = new SelectList(db.Products, "productID", "title", evaluate.productID);
            return View(evaluate);
        }

        // GET: Evaluates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluate evaluate = db.Evaluates.Find(id);
            if (evaluate == null)
            {
                return HttpNotFound();
            }
            ViewBag.userID = new SelectList(db.Accounts, "userID", "email", evaluate.userID);
            ViewBag.productID = new SelectList(db.Products, "productID", "title", evaluate.productID);
            return View(evaluate);
        }

        // POST: Evaluates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "evaluateID,evaluateContent,rate,date,userID,productID")] Evaluate evaluate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaluate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userID = new SelectList(db.Accounts, "userID", "email", evaluate.userID);
            ViewBag.productID = new SelectList(db.Products, "productID", "title", evaluate.productID);
            return View(evaluate);
        }

        // GET: Evaluates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluate evaluate = db.Evaluates.Find(id);
            if (evaluate == null)
            {
                return HttpNotFound();
            }
            return View(evaluate);
        }

        // POST: Evaluates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Evaluate evaluate = db.Evaluates.Find(id);
            db.Evaluates.Remove(evaluate);
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
