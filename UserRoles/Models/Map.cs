using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UserRoles.Models
{
    public class Map
    {
        [Key]
        public int Id { get; set; }

        public double Distance { get; set; }
        public string Duration { get; set; }
        public double Cost { get; set; }
        public string Email { get; set; }
        public string DelAddress { get; set; }
        public int orderID { get; set; }
        //public virtual Order Order { get; set; }
      

    }

}