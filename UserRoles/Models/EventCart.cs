using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UserRoles.Models
{
    public class EventCart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Key { get; set; }
        public EventOrderDetail EventOrderDetail { get; set; }
        public DateTime Date { get; set; }
        public Package Package { get; set; }
        [Range(0, 10000, ErrorMessage = "value must be above 1 ")]
        public int Quantity { get; set; }
        public EventCart(Package package, int quantity)
        {
            Package = package;
            Quantity = quantity;
        }
    }
}