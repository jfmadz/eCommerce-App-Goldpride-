
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserRoles.Models;

namespace UserRoles.Controllers
{
    public class InvoiceController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Invoice
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(string searchString)
        {
            ApplicationDbContext db = new ApplicationDbContext(); //dbcontect class
            List<InvoiceVM> VMlist = new List<InvoiceVM>(); // to hold list of Customer and order details

            var customerlist = (from order in db.Orders

                                join Od in db.OrderDetails on order.OrderID equals Od.OrderID select new  { order.CustomerName,order.Surname, order.CustomerPhone, order.CollDate,
                                    order.OrderDate, Od.OrderID, Od.ProductID, Od.Quantity, Od.Price }).ToList();


            // selectnew  { order.Name, Cust.Mobileno, Cust.Address, Ord.OrderDate, Ord.OrderPrice}).ToList();
            //query getting data from database from joining two tables and storing data in customerlist

            foreach (var item in customerlist)

            {

                InvoiceVM objcvm = new InvoiceVM(); // ViewModel

                objcvm.CustomerName = item.CustomerName;

                objcvm.CustomerPhone = item.CustomerPhone;

                objcvm.CustomerAddress = item.CollDate.ToString();

                objcvm.OrderDate = item.OrderDate;

                objcvm.Price = item.Price;

                VMlist.Add(objcvm);

            }
            return View(VMlist.ToList());
        }
       

        // GET: Invoice/Details/5
        public ActionResult Details(int id, string searchString)
        {
            var list = db.InvoiceVMs.Take(10);
            //search code test
            if ((!string.IsNullOrEmpty(searchString)))
            {
                list = list.Where(s => s.Email.Contains(searchString));
            }
            return View(list.ToList());
        }

        // GET: Invoice/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Invoice/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
               
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Invoice/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Invoice/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Invoice/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Invoice/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
