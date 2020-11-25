using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserRoles.Models;


namespace UserRoles.Controllers
{
    public class DriverVMController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult DDelivery()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;

            List<DriverVM> VMlist = new List<DriverVM>();

            var col = (from i in db.Maps
                       join x in db.Orders on i.orderID equals x.OrderID
                       where i.Distance > 0 && x.Delivered == false 
                       /*&& x.OutForDel == false*/ && x.Collected ==false 
                       && x.Collected == false 
                       && x.DriverID==ID
                     
                       select new
                       {
                           i.DelAddress,
                           i.Email,
                           x.CustomerName,
                           x.CustomerPhone,
                           x.OrderID,
                           x.Collected,
                           x.Delivered,
                           x.OutForCol,
                           x.OutForDel,
                           x.DriverID
                       }).Distinct();

            foreach (var item in col)
            {
               

                DriverVM obj = new DriverVM();
                obj.Email = item.Email;
                obj.CustomerPhone = item.CustomerPhone;
                obj.Name = item.CustomerName;
                obj.DeliveryAddress = item.DelAddress;
                obj.OutForDel = item.OutForDel;
                obj.Delivered = item.Delivered;
                obj.OutForCol = item.OutForCol;
                obj.Collected = item.Collected;
                obj.OrderID = item.OrderID;
                obj.DriverID = item.DriverID;


                VMlist.Add(obj);

            }

            return View(VMlist.ToList());
        }

       

        // GET: DriverVM
        public ActionResult DCollection()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;

            List<DriverVM> VMlist = new List<DriverVM>();

            var col = (from i in db.Maps
                       join x in db.Orders on i.orderID equals x.OrderID
                       where x.Delivered == true && x.OutForDel == true
                       && x.DriverID==ID && x.Collected == false
                     
                       select new
                       {
                           i.DelAddress,
                           i.Email,
                           x.CustomerName,
                           x.CustomerPhone,
                           x.OrderID,
                           x.Collected,
                           x.Delivered,
                           x.OutForCol,
                           x.OutForDel,
                           x.DriverID
                       }).Distinct();

            foreach (var item in col )
            {
                DriverVM obj = new DriverVM();
                obj.Email = item.Email;
                obj.CustomerPhone = item.CustomerPhone;
                obj.Name = item.CustomerName;
                obj.DeliveryAddress = item.DelAddress;
                obj.OutForDel = item.OutForDel;
                obj.Delivered = item.Delivered;
                obj.OutForCol = item.OutForCol;
                obj.Collected = item.Collected;
                obj.OrderID = item.OrderID;
                obj.DriverID = item.DriverID;

                VMlist.Add(obj);

            }

            return View(VMlist.ToList());
        }

        [HttpPost]
        public ActionResult AcceptOrder( DriverVM driverVM)
        {



            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;

            if (ModelState.IsValid)
            {
                driverVM.DriverID = ID;

                db.DriverVMs.Add(driverVM);
                db.SaveChanges();

                return RedirectToAction("DDelivery", "DriverVM");
            }
            return View(driverVM);
            //return RedirectToAction("DDelivery", "DriverVM");

        }

        public ActionResult Accept()
        {
            List<DriverVM> VMlist = new List<DriverVM>();

            var col = (from i in db.Maps
                       join x in db.Orders on i.Id equals x.OrderID
                       where i.Distance > 0 && x.Delivered == false && x.OutForDel == false
                       
                       select new
                       {
                           i.DelAddress,
                           i.Email,
                           x.CustomerName,
                           x.CustomerPhone,
                           x.OrderID,
                           x.Collected,
                           x.Delivered,
                           x.OutForCol,
                           x.OutForDel,
                           x.DriverID
                       }).Distinct();

            foreach (var item in col)
            {
                DriverVM obj = new DriverVM();
                obj.Email = item.Email;
                obj.CustomerPhone = item.CustomerPhone;
                obj.Name = item.CustomerName;
                obj.DeliveryAddress = item.DelAddress;
                obj.OutForDel = item.OutForDel;
                obj.Delivered = item.Delivered;
                obj.OutForCol = item.OutForCol;
                obj.Collected = item.Collected;
                obj.OrderID = item.OrderID;
                obj.DriverID = item.DriverID;

                VMlist.Add(obj);

            }
            return View(VMlist.ToList());
        }
    }
}