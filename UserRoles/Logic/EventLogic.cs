using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventBook.Models;
using UserRoles.Models;

namespace EventBook.Logic
{
    public class EventLogic
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public List<Event> Getdata()
        {
            return db.Events.ToList();
        }

        public List<Event> GetAll()
        {
            return Getdata().Select(x => new Event
            {
                EventId = x.EventId,
                EventType = x.EventType,
                Start = x.Start,
                End = x.End,
                Fname = x.Fname,
                contactNum = x.contactNum,

            }).ToList();
        }



        public Event FindById(int id)
        {
            return GetAll().FirstOrDefault(x => x.Start.Equals(id));
        }


        public IQueryable<Event> coreSearchAdmin(string sDate)
        {

            var result = GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(sDate))
            {
                try
                {
                    result = result.Where(x => x.EventType.ToLower().Contains(sDate.ToLower()));
                }
                catch
                {
                    return null;
                }
            }



            return result;
        }
    }
}