using IdGen;
using Project_PRN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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

        public JsonResult AddBill(string name, string address, string phone, string email, int payment) {
            try {
                Bill bill = new Bill();

                //Declare snowflake algorithm
                DateTime epoch = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                //41 bit for time
                //12 bit for number of shard
                //10 bit for sequence
                IdStructure structure = new IdStructure(41, 12, 10);
                IdGeneratorOptions option = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));
                IdGenerator generator = new IdGenerator(0, option);

                //create new id and declare properties to bill
                long billID = generator.CreateId();

                bill.BillID = billID;
                bill.userName = name;
                bill.address = address;
                bill.phoneNumber = phone;
                bill.email = email;
                bill.payment = payment;
                bill.status = 1;
                bill.orderTime = DateTime.Now;

                //is loged User
                if (Session["user"] == null) {
                    //didn't log in case, storage cart in cookies
                    var serializer = new JavaScriptSerializer();

                    Dictionary<string, int> cart;

                    //check is exsisted cart in cookies
                    if (Request.Cookies["cart"] != null) {
                        //exsisted case, pick up it
                        string cartJson = Request.Cookies["cart"].Value;
                        cart = serializer.Deserialize<Dictionary<string, int>>(cartJson);

                        Dictionary<string, int>.KeyCollection keys = cart.Keys;

                        //add bill to database
                        foreach (string key in keys) {
                            bill.productid = Int32.Parse(key);
                            bill.quantity = cart[key];
                            bill.amount = db.Products.Find(bill.productid = Int32.Parse(key)).price;

                            //add into bill
                            db.Bills.Add(bill);
                            db.SaveChanges();
                        }
                        Response.Cookies["cart"].Expires = DateTime.Now.AddMinutes(-1);
                    } else {
                        //in null case of cart
                        return Json("Please put item into cart before check out!", JsonRequestBehavior.AllowGet);
                    }

                } else {
                    //loged in case
                    Account account = db.Accounts.Find(Int32.Parse(Session["user"].ToString()));
                    bill.userid = account.userID;
                    
                    //load cart infor from database
                    List<Cart> carts = db.Carts.ToList().Select(cart => new Cart {
                        cartid = cart.cartid,
                        userid = cart.userid,
                        productid = cart.productid,
                        quantity = cart.quantity,
                        Account = db.Accounts.Find(cart.userid),
                        Product = db.Products.Find(cart.productid)
                    }).Where(c => c.userid == account.userID).ToList();

                    //check is exsisted any item in cart in database
                    if (carts.Count > 0) {
                        foreach (Cart cart in carts) {
                            bill.productid = cart.productid;
                            bill.quantity = cart.quantity;
                            bill.amount = cart.Product.price;

                            //add bill
                            db.Bills.Add(bill);
                            //remove item from cart
                            db.Carts.Remove(db.Carts.Find(cart.cartid));
                            db.SaveChanges();
                        }
                    } else {
                        //in null case of cart
                        return Json("Please put item into cart before check out!", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("Check Out Success!", JsonRequestBehavior.AllowGet);
            } catch {
                return Json("Check Out Fail!", JsonRequestBehavior.AllowGet);
            }

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
