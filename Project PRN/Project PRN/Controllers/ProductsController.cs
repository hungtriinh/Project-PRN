using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Project_PRN.Models;

namespace Project_PRN.Controllers
{
    public class ProductsController : Controller
    {
        private ProjectPRNEntities3 db = new ProjectPRNEntities3();

        // GET: Products
        public ActionResult Product()
        {
            return View();
        }

        public JsonResult ProductJson(int? index, string searchText, int? userID)
        {
            if (index == null)
            {
                index = 1;
            }
            int pageSize = 8;
            int pageNumber = (index ?? 1);
            db.Configuration.ProxyCreationEnabled = false;
            var listProduct = db.Products.ToList().Select(product => new Product
            {
                productID = product.productID,
                title = product.title,
                author = product.author,
                description = product.description,
                shortDescription = product.shortDescription,
                image = product.fullImagePath(),
                price = product.price,
                sold = product.sold,
                postTime = product.postTime,
                categoriesID = product.categoriesID,
                userID = product.userID,
                rate = product.calculateRate(),
                Account = db.Accounts.Find(product.userID),
                Category = db.Categories.Find(product.categoriesID),
                Evaluates = db.Evaluates.Where(e => e.productID == product.productID).ToList()

            }).OrderBy(product => product.postTime).ToPagedList(pageNumber, pageSize);
            return Json(listProduct, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UploadProduct()
        {
            return View();
        }

        public ActionResult ProductDetail()
        {
            return View();
        }

        public JsonResult GetCategoriesJson()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Category> listCategories = db.Categories.ToList();
            return Json(listCategories, JsonRequestBehavior.AllowGet);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.userID = new SelectList(db.Accounts, "userID", "email");
            ViewBag.categoriesID = new SelectList(db.Categories, "categoriesID", "categoriesName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "productID,title,author,description,shortDescription,image,price,quantity,sold,postTime,categoriesID,userID")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userID = new SelectList(db.Accounts, "userID", "email", product.userID);
            ViewBag.categoriesID = new SelectList(db.Categories, "categoriesID", "categoriesName", product.categoriesID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.userID = new SelectList(db.Accounts, "userID", "email", product.userID);
            ViewBag.categoriesID = new SelectList(db.Categories, "categoriesID", "categoriesName", product.categoriesID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "productID,title,author,description,shortDescription,image,price,quantity,sold,postTime,categoriesID,userID")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userID = new SelectList(db.Accounts, "userID", "email", product.userID);
            ViewBag.categoriesID = new SelectList(db.Categories, "categoriesID", "categoriesName", product.categoriesID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
