using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quartz;

namespace UserRoles.Controllers
{
    public class HomeController : ApplicationBaseController
    {
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return View("AdminIndex");
            else if (User.IsInRole("Driver"))
                return RedirectToAction("DDelivery","DriverVM");
            else
            {
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();

        }
        public ActionResult Map()
        {
           

            return View();

        }
        [HttpPost]
        public ActionResult Map(FormCollection form)
        {
            string Distance = form["dvDistance"];
            return View();
        }
        public ActionResult Camera()
        {
            return View();
        }

    }

    
}