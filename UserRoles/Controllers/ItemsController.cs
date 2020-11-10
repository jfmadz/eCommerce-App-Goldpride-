using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventBook.Models;
using UserRoles.Models;

namespace EventBook.Controllers
{
    public class ItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Items
        public ActionResult Citems()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }


        // GET: Items/Details/5
        public JsonResult GetItems()
        {
            using (ApplicationDbContext dc = new ApplicationDbContext())
            {
                var items = dc.Items.ToList();
                return new JsonResult { Data = items, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult SaveIt(Item i)
        {
            var status = false;
            using (ApplicationDbContext dc = new ApplicationDbContext())
            {
                if (i.ItemId > 0)
                {
                    var v = dc.Items.Where(a => a.ItemId == i.ItemId).FirstOrDefault();
                    if (v != null)
                    {
                        v.EventType = i.EventType;
                        v.Start = i.Start;
                        v.End = i.End;
                        v.Fullname = i.Fullname;
                        v.Mobilen = i.Mobilen;
                        v.ItemType = i.ItemType;
                    }
                }
                else
                {
                    dc.Items.Add(i);
                }
                dc.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status } };
        }

        [HttpPost]
        public JsonResult DeleteIt(int itemID)
        {
            var status = false;
            using (ApplicationDbContext dc = new ApplicationDbContext())
            {
                var v = dc.Items.Where(a => a.ItemId == itemID).FirstOrDefault();
                if (v != null)
                {
                    dc.Items.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}
