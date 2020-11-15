using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserRoles.Models;

namespace UserRoles.Controllers
{
    public class PackageVMController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult GetPackage()
        {
            List<PackageVM> VMList = new List<PackageVM>();

            var pack = (from i in db.Packages
                        join x in db.PackageImages
                        on i.PackageId equals x.packageId




                        select new
                        {
                            i.PackageId,
                            i.packageName,
                            i.Description,
                            i.CategoryId,
                            i.IsActive,
                            i.Price,
                            x.ImageID,
                            x.Image,
                            //x.packageId

                        }).Distinct()/*.ToList()*/;
            foreach (var item in pack)
            {
                PackageVM obj = new PackageVM();
                //obj.PackageId = item.PackageId;
                obj.packageName = item.packageName;
                obj.Description = item.Description;
                obj.CategoryId = item.CategoryId;
                obj.IsActive = item.IsActive;
                obj.Price = item.Price;
                obj.ImageID = item.ImageID;
                obj.Image = item.Image;
                //obj.packId = item.packageId;

                VMList.Add(obj);
            }
            return View(pack.ToList());
        }
        // GET: PackageVM
        public ActionResult Index()
        {
            return View();
        }
    }
}