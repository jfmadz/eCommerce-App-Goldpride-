using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EventBook.Models
{
    public class Validation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string Event = value.ToString();
            if (Event.ToLower() == "Lower")
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Enter event type");
            }
        }
    }

}