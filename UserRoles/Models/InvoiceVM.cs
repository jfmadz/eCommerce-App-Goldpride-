using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using UserRoles.Models;


namespace UserRoles.Models
{
    public class InvoiceVM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceNum { get; set; }
        public string Email { get; set; }
        public int OrderID { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string Surname { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public bool PickUp { get; set; }
        public decimal Price { get; set; }

       
    }
}