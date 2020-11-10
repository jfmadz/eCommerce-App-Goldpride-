using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UserRoles.Models
{
    public class OrderVM
    {
        [Key]
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal  Price { get; set; }
        public decimal  Total { get; set; }
        public string CustomerName { get; set; }
        public string Surname { get; set; }
        public string CustomerPhone { get; set; }
        public string Email { get; set; }
        public string CustomerAddress { get; set; }
    }
}