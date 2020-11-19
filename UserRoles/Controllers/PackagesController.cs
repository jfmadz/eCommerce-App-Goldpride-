using iText.Layout.Element;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserRoles.Models;


namespace UserRoles.Controllers
{
    public class PackagesController : Controller
    {
        public ActionResult CusBrowse(string searchString, int? page)
        {
            var packages = (from i in db.Packages
                            where i.IsActive == true 
                            select i).Include(i => i.Category);
            if ((!string.IsNullOrEmpty(searchString)))
            {
                packages = packages.Where(s => s.packageName.Contains(searchString));
            }
            return View(packages.ToList().ToPagedList(page ?? 1, 5));
        }
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult CusIndex(string searchString, int? page)
        {
            var packages = (from i in db.Packages
                           where i.IsActive == true
                           select i).Include(i => i.Category);
            if ((!string.IsNullOrEmpty(searchString)))
            {
                packages = packages.Where(s => s.packageName.Contains(searchString));
            }
                return View(packages.ToList().ToPagedList(page ?? 1,5));
        }
        // GET: Packages
        public ActionResult Index()
        {
            //var packages = db.Packages.Include(p => p.Category);
            var packages = (from i in db.Packages
                            select i).Include(p => p.Category);
            return View(packages.ToList());
        }
        public ActionResult PackageDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }
        // GET: Packages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // GET: Packages/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name");
            return View();
        }

        // POST: Packages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PackageId,packageName,Price,Description,CategoryId,imagePack,IsActive")] Package package,HttpPostedFileBase image3)
        {
            //Multi image upload start
            //if (ModelState.IsValid)
            //{
            //    db.Packages.Add(package);
            //    if (image3 != null)
            //    {

            //        var imageList = new List<PackageImages>();
            //        foreach (var image in image3)
            //        {
            //            using (var br = new BinaryReader(image.InputStream))
            //            {

            //                var data = br.ReadBytes(image.ContentLength);
            //                var img = new PackageImages { packageId = package.PackageId };
            //                img.Image = data;
            //                imageList.Add(img);

            //            }
            //        }
            //        package.PackageImages = imageList;
            //        package.imagePack = Convert.ToByte(package.PackageImages.) ;

            //    }
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //end

            if (image3 != null)
            {
                package.imagePack = new byte[image3.ContentLength];
                image3.InputStream.Read(package.imagePack, 0, image3.ContentLength);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please add image");
            }
            if (ModelState.IsValid)
            {
                db.Packages.Add(package);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name", package.CategoryId);
            return View(package);
        }

        // GET: Packages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name", package.CategoryId);
            return View(package);
        }

        // POST: Packages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PackageId,packageName,Price,Description,CategoryId, imagePack,IsActive")] Package package, HttpPostedFileBase image2)
        {
            if (ModelState.IsValid)
            {
                var img = (from i in db.Packages
                           where i.PackageId == package.PackageId
                           select i.imagePack).Single();

                if (image2 != null)
                {
                    package.imagePack = new byte[image2.ContentLength];
                    image2.InputStream.Read(package.imagePack, 0, image2.ContentLength);
                    db.Entry(package).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                else if (img != null)
                {
                    package.imagePack = img;
                    db.Entry(package).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                else
                {
                    ViewBag.imageError = "Please add an Image";
                }
            }
                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Category_Name", package.CategoryId);
            return View(package);
        }

        // GET: Packages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // POST: Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Package package = db.Packages.Find(id);
            db.Packages.Remove(package);
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
