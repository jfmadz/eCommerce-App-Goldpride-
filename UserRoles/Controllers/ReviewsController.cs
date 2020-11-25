using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserRoles.Models;
using System.Web.Helpers;
using Quartz;


namespace UserRoles.Controllers
{
   //[Authorize]
    public class ReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult ChartPie()
        {

            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();
            var poor = (from i in db.Reviews
                        where i.Rating == "Poor"
                        select i.Rating).Count();
            var res = (from i in db.Reviews
                       where i.Rating == "Good"
                       select i.Rating).Count();
            var result = (from i in db.Reviews
                          where i.Rating == "Excellent"

                          select i).Count();

            var test = from i in db.Reviews select i.Rating;



            result.ToString().ToList().ForEach(rs => yValue.Add(result));
            res.ToString().ToList().ForEach(rs => yValue.Add(res));
            poor.ToString().ToList().ForEach(rs => yValue.Add(poor));
            //test.ToList().ForEach(rs => xValue.Add(rs.Rating));
            //test.ToList().ForEach(rs => xValue.Add(rs.Rating));
            //test.ToList().ForEach(rs => xValue.Add(rs.Rating));

            //result.ToString().ToList().ForEach(rs => xValue.Add(result));
            //res.ToString().ToList().ForEach(rs => xValue.Add(result));
            //poor.ToString().ToList().ForEach(rs => xValue.Add(poor));



            var chart = new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla)

                .AddTitle("Chart for Review[Pie Chart]")
                .AddLegend("Summary")
                .SetXAxis("Excellent")

                .AddSeries("Default", chartType: "Pie", xValue: new[] { "Excellent", "Good", "Poor" }, yValues: yValue)
                .Write("bmp");


            //return File(chart, "image/bytes");




            return null;

        }
        public JsonResult Cha()
        {
            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();
            var poor = (from i in db.Reviews
                        where i.Rating == "Poor"
                        select i.Rating).Count();
            var res = (from i in db.Reviews
                       where i.Rating == "Good"
                       select i.Rating).Count();
            var result = (from i in db.Reviews
                          where i.Rating == "Excellent"

                          select i.Rating).Count();

            var names = (from i in db.Reviews


                         select i.Rating);
          
            return Json(new { JSONList = poor, res, result, names },JsonRequestBehavior.AllowGet);
        }
        // GET: Reviews
        public ActionResult Index()
        {
            var a = (from i in db.Reviews
                     where i.isActive == true
                     select i);


            return View(a.ToList());
        }

        public ActionResult AdminRevIndex()
        {

            return View(db.Reviews.ToList());
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }
        [Authorize]
        // GET: Reviews/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommentID,Rating,Comment,Date,isActive,User,FName,LName,servRating,useService,recommend")] Review review)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;

            //var a = (from i in db.Orders
            //        where i. == Email
            //        select i).ToString();

            if (ModelState.IsValid /*&& Email==a*/)
            {
                review.UserEmail = Email;
                review.FName = Name;
                review.LName = SName;
                review.Date = DateTime.Now.ToLongDateString();

                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(review);
        }

        // GET: Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommentID,Rating,Comment,Date,isActive,UserEmail,FName,LName,servRating,useService,recommend")] Review review)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;

            if (ModelState.IsValid && User.IsInRole("Admin"))
            {

                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminRevIndex");
            }
            
            else if (ModelState.IsValid && Email == review.UserEmail)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            else
            {
                ViewBag.Error = "You Do Not Have Permission To Edit";
            }

            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Name = currentUser.Name;
            string SName = currentUser.Surname;

            Review review = db.Reviews.Find(id);
            if (Email == review.UserEmail)
            {
                db.Reviews.Remove(review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(review);
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
