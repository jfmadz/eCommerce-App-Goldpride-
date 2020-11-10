using iText.IO.Source;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UserRoles.Models
{
    public class EventOrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Key { get; set; }
        public int PackageId { get; set; }

        public int ID { get; set; }
        
        public string packageName { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Deposit { get; set; }
        public string HallAddress { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public virtual Package Package { get; set; }
        
        public virtual BookEvent BookEvent { get; set; }
    }
}