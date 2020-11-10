using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserRoles.Models
{
    public class DriverVM
    {
        [Key]
        public int DriverVMID { get; set; }

        public int OrderID { get; set; }
        public string Email { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string DriverID { get; set; }

        public string Name { get; set; }
        [DisplayName("Out For Delivery")]
        public bool OutForDel { get; set; }

        [DisplayName("Items Delivered To Customer")]
        public bool Delivered { get; set; }

        [DisplayName("Out For Retrieval Of Items")]
        public bool OutForCol { get; set; }

        [DisplayName("Items Retrieved")]
        public bool Collected { get; set; }
    }
}