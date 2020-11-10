using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserRoles.Models;

namespace UserRoles.Controllers
{
    public class DecorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Decors
        public ActionResult Index(string searchString)
        {
            var decors = db.Decors.Include(d => d.Category);
            if ((!string.IsNullOrEmpty(searchString)))
            {
                decors = decors.Where(s => s.DecorName.Contains(searchString));
            }
            return View(decors.ToList());
        }
        public ActionResult DecorInentory(string searchString)
        {
            var list = db.Decors.Take(10);
            if ((!string.IsNullOrEmpty(searchString)))
            {
                list = list.Where(s => s.DecorName.Contains(searchString));
            }
            return View(list.ToList());
        }
        [Authorize(Roles = "Admin")]
        public ActionResult DecorInventoryAdmin(string searchString)
        {
            var list = db.Decors.Take(10);
            if ((!string.IsNullOrEmpty(searchString)))
            {
                list = list.Where(s => s.DecorName.Contains(searchString));
            }
            return View(list.ToList());
        }
        // GET: Decors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Decor decor = db.Decors.Find(id);
            if (decor == null)
            {
                return HttpNotFound();
            }
            return View(decor);
        }

        // GET: Decors/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name");
            return View();
        }

        // POST: Decors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DecorCode,DecorName,CategoryId,Description,Price,Image,ImageType,IsActive")] Decor decor,HttpPostedFileBase image1)
        {
            if (image1 != null)
            {
                decor.Image = new byte[image1.ContentLength];
                image1.InputStream.Read(decor.Image, 0, image1.ContentLength);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please add image");
            }
            if (ModelState.IsValid)
            {
                
                db.Decors.Add(decor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name", decor.CategoryId);
            return View(decor);
        }

        // GET: Decors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Decor decor = db.Decors.Find(id);
            if (decor == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name", decor.CategoryId);
            return View(decor);
        }

        // POST: Decors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DecorCode,DecorName,CategoryId,Description,Price,Image,ImageType,IsActive")] Decor decor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(decor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name", decor.CategoryId);
            return View(decor);
        }

        // GET: Decors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Decor decor = db.Decors.Find(id);
            if (decor == null)
            {
                return HttpNotFound();
            }
            return View(decor);
        }

        // POST: Decors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Decor decor = db.Decors.Find(id);
            db.Decors.Remove(decor);
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
