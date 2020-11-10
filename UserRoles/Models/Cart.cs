using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UserRoles.Models
{
    public class Cart
    {
        [Key]
        public int RecordID { get; set; }
        public OrderDetail OrderDetail { get; set; }
        
        public ItemsHire ItemsHire { get; set; }
       
        [Range(0, 10000, ErrorMessage = "value must be above 1 "  )]
        public int Quantity { get; set; }
        public Cart (ItemsHire itemsHire, int quantity)
        {
            
            ItemsHire = itemsHire;
            Quantity = quantity;
        }
       
        
    }
}