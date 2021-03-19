using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Project_PRN.Models;
using System.Configuration;

namespace Project_PRN.Controllers {
    public class ProductsController : Controller {
        private ProjectPRNEntities3 db = new ProjectPRNEntities3();

        // GET: Products
        public ActionResult Product() {
            return View();
        }

        public JsonResult ProductJson(int? index, string searchText, int? userID) {
            if (index == null) {
                index = 1;
            }
            int pageSize = 9;
            int pageNumber = (index ?? 1);
            db.Configuration.ProxyCreationEnabled = false;
            var listProduct = db.Products.ToList().Select(product => new Product {
                productID = product.productID,
                title = product.title,
                author = product.author,
                description = product.description,
                shortDescription = product.shortDescription,
                image = product.fullImagePath(),
                price = product.price,
                quantity = product.quantity,
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

        public ActionResult UploadProduct() {
            return View();
        }

        public ActionResult ProductDetail(int? productID) {
            ViewData["productID"] = productID;
            return View();
        }

        public JsonResult ProductDetailJson(int? productID) {
            db.Configuration.ProxyCreationEnabled = false;
            Product products = db.Products.ToList().Select(product => new Product {
                productID = product.productID,
                title = product.title,
                author = product.author,
                description = product.description,
                shortDescription = product.shortDescription,
                image = product.fullImagePath(),
                price = product.price,
                quantity = product.quantity,
                sold = product.sold,
                postTime = product.postTime,
                categoriesID = product.categoriesID,
                userID = product.userID,
                rate = product.calculateRate(),
                Account = db.Accounts.Find(product.userID),
                Category = db.Categories.Find(product.categoriesID),
                Evaluates = db.Evaluates.ToList().Select(evaluate => new Evaluate {
                    evaluateID = evaluate.evaluateID,
                    evaluateContent = evaluate.evaluateContent,
                    rate = evaluate.rate,
                    date = evaluate.date,
                    userID = evaluate.userID,
                    productID = evaluate.productID,
                    Account = db.Accounts.Find(evaluate.userID),
                    Product = db.Products.Find(evaluate.productID),



                }).Where(e => e.productID == productID).ToList()

            }).Where(p => p.productID == productID).First();
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoriesJson() {
            db.Configuration.ProxyCreationEnabled = false;
            List<Category> listCategories = db.Categories.ToList();
            return Json(listCategories, JsonRequestBehavior.AllowGet);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null) {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create() {
            ViewBag.userID = new SelectList(db.Accounts, "userID", "email");
            ViewBag.categoriesID = new SelectList(db.Categories, "categoriesID", "categoriesName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "productID,title,author,description,shortDescription,image,price,quantity,sold,postTime,categoriesID,userID")] Product product, HttpPostedFileBase image) {
            if (ModelState.IsValid) {
                product.userID = Int32.Parse(Session["user"].ToString());
                product.postTime = DateTime.Now;
                string imgPath = ConfigurationManager.ConnectionStrings["imagePath"].ToString();
                try {
                    if (image != null) {
                        string path = Path.Combine(Server.MapPath($"~{imgPath}"), Path.GetFileName(image.FileName));
                        image.SaveAs(path);
                        product.image = image.FileName;
                    }
                    ViewBag.FileStatus = "File uploaded successfully.";
                } catch (Exception) {

                    ViewBag.FileStatus = "Error while file uploading.";
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToRoute(new {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                });
            }
            return RedirectToAction("UploadProduct");
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null) {
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
        public ActionResult Edit([Bind(Include = "productID,title,author,description,shortDescription,image,price,quantity,sold,postTime,categoriesID,userID")] Product product) {
            if (ModelState.IsValid) {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userID = new SelectList(db.Accounts, "userID", "email", product.userID);
            ViewBag.categoriesID = new SelectList(db.Categories, "categoriesID", "categoriesName", product.categoriesID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null) {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
