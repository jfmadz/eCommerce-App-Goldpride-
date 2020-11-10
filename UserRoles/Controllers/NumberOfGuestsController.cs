using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserRoles.Models;

namespace UserRoles.Controllers
{
    public class NumberOfGuestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: NumberOfGuests
        public ActionResult Index()
        {
            return View(db.NumberOfGuests.ToList());
        }

        // GET: NumberOfGuests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NumberOfGuests numberOfGuests = db.NumberOfGuests.Find(id);
            if (numberOfGuests == null)
            {
                return HttpNotFound();
            }
            return View(numberOfGuests);
        }

        // GET: NumberOfGuests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NumberOfGuests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Key,NumberOfPeople")] NumberOfGuests numberOfGuests)
        {
            if (ModelState.IsValid)
            {
                db.NumberOfGuests.Add(numberOfGuests);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(numberOfGuests);
        }

        // GET: NumberOfGuests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NumberOfGuests numberOfGuests = db.NumberOfGuests.Find(id);
            if (numberOfGuests == null)
            {
                return HttpNotFound();
            }
            return View(numberOfGuests);
        }

        // POST: NumberOfGuests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Key,NumberOfPeople")] NumberOfGuests numberOfGuests)
        {
            if (ModelState.IsValid)
            {
                db.Entry(numberOfGuests).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(numberOfGuests);
        }

        // GET: NumberOfGuests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NumberOfGuests numberOfGuests = db.NumberOfGuests.Find(id);
            if (numberOfGuests == null)
            {
                return HttpNotFound();
            }
            return View(numberOfGuests);
        }

        // POST: NumberOfGuests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NumberOfGuests numberOfGuests = db.NumberOfGuests.Find(id);
            db.NumberOfGuests.Remove(numberOfGuests);
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
