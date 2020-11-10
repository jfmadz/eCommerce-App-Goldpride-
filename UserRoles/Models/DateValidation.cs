using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventBook.Models
{
    public class EventvBook : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value != null && ((DateTime)value >= DateTime.Now);
        }
    }
}
