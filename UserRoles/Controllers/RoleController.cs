using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using UserRoles.Models;

namespace UserRoles.Controllers
{
    
   // [Authorize(Roles = "Admin")]
    public class RoleController : ApplicationBaseController
    {
       private ApplicationDbContext db = new ApplicationDbContext();
        ApplicationDbContext context;

        public RoleController()
        {
            context = new ApplicationDbContext();
        }
        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        // GET: Role
       
        public ActionResult Index()
        {
            var Roles = context.Roles.ToList();
            return View(Roles);

            
        }

        ///<summary>
        ///Create a New Role
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }

        ///<summary>
        ///Create a New Role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult Create(IdentityRole Role)
        {
            context.Roles.Add(Role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}