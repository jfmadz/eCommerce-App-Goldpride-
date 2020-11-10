using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EventBook.Models
{
    public class Item
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; set; }


        public string EventType { get; set; }

        public string Fullname { get; set; }
        public string Mobilen { get; set; }

        public System.DateTime Start { get; set; }

        public System.DateTime End { get; set; }
        public string ItemType { get; set; }
    }
}