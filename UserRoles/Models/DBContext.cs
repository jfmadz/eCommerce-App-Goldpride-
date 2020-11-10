using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using UserRoles.Models;

namespace EventBook.Models
{
    public class DBContext : DbContext
    {
        public DBContext() : base("name=DefaultConnection") { }

       
    }
}