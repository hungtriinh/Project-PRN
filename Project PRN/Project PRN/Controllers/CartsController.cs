using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Project_PRN.Models;

namespace Project_PRN.Controllers {
    public class CartsController : Controller {
        private ProjectPRNEntities3 db = new ProjectPRNEntities3();

        // GET: Carts
        public ActionResult Cart() {
            return View();
        }

        //Add item into cart
        public JsonResult AddToCart(int productID, int quantity) {
            try {
                db.Configuration.ProxyCreationEnabled = false;
                //userID from session
                //check is user logged in
                if (Session["user"] != null) {
                    //logged in case, storage cart in database

                    int userID = Int32.Parse(Session["user"].ToString());

                    //select items from cart with userID and productid
                    Cart cart = db.Carts.Where(c => c.userid == userID).Where(c => c.productid == productID).FirstOrDefault();
                    if (cart == null) {
                        //in null case, add new items to database
                        cart = new Cart();
                        cart.userid = userID;
                        cart.productid = productID;
                        cart.quantity = quantity;
                        db.Carts.Add(cart);
                        db.SaveChanges();
                    } else {
                        //in exsisted case, change quantity
                        cart.quantity += quantity;
                        db.Entry(cart).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                } else {
                    //didn't log in case, storage cart in cookies
                    var serializer = new JavaScriptSerializer();

                    Dictionary<string, int> cart;
                    
                    //check is exsisted cart in cookies
                    if (Request.Cookies["cart"] != null) {
                        //exsisted case, pick up it
                        string cartJson = Request.Cookies["cart"].Value;
                        cart = serializer.Deserialize<Dictionary<string, int>>(cartJson);
                    } else {
                        //not exsisted case, declare new cart
                        cart = new Dictionary<string, int>();
                    }
                    
                    //check is exsisted item in cart
                    if (cart.ContainsKey(productID.ToString())) {
                        //in exsisted case, change quantity
                        int currentQuantity = cart[productID.ToString()];
                        cart[productID.ToString()] = currentQuantity + quantity;
                    } else {
                        //in not exsisted case, add new item to cart
                        cart[productID.ToString()] = quantity;
                    }
                    
                    //save into cookies
                    string cartValue = serializer.Serialize(cart);
                    Response.Cookies["cart"].Value = cartValue;
                    Response.Cookies["cart"].Expires = DateTime.Now.AddDays(30);

                }
                return Json("product added successfully!", JsonRequestBehavior.AllowGet);
            } catch {
                return Json("product added fail!", JsonRequestBehavior.AllowGet);
            }


        }


        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
