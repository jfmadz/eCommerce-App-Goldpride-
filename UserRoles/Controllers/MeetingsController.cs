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
    public class MeetingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Meetings
        public ActionResult Index(string searchString)
        {
            var list = from u in db.Meetings
                      orderby u.messageID descending
                       select u;

            if ((!string.IsNullOrEmpty(searchString)))
            {
                list = (IOrderedQueryable<Meeting>)list.Where(s => s.Date.Contains(searchString));
            }
            //return View(db.Meetings.ToList());
            return View(list.ToList());
        }
        public ActionResult Test()
        {
            return View();
        }
        public ActionResult CusMeetIndex(string searchString)
        {
            var list = from u in db.Meetings
                       where u.Email == User.Identity.Name
                       select u;
            if ((!string.IsNullOrEmpty(searchString)))
            {
                list = list.Where(s => s.Date.Contains(searchString));
            }
            return View(list.ToList());
           
        }

        // GET: Meetings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            return View(meeting);
        }

        // GET: Meetings/Create
        public ActionResult Create()
        {
             ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name");
            return View();
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "messageID,Email,Date,Start,End,Attend,Discussion,CategoryId,ckeditor")] Meeting meeting)
        {
            if (ModelState.IsValid)
            {

                meeting.Date = DateTime.Today.ToLongDateString();
                meeting.End =Convert.ToDateTime( DateTime.Now.ToShortTimeString());

                db.Meetings.Add(meeting);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name", meeting.CategoryId);
            return View(meeting);
        }

        // GET: Meetings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name",meeting.CategoryId);
            return View(meeting);
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "messageID,Email,Date,Start,End,Attend,Discussion,CategoryId,ckeditor")] Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(meeting).State = EntityState.Modified;
                meeting.Start = meeting.Start;
                meeting.End = meeting.End;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name", meeting.CategoryId);

            return View(meeting);
        }

        // GET: Meetings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            return View(meeting);
        }

        // POST: Meetings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Meeting meeting = db.Meetings.Find(id);
            db.Meetings.Remove(meeting);
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
