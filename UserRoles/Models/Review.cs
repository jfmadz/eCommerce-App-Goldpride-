using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserRoles.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentID { get; set; }
        [Required]
        public string Rating { get; set; }
        public string Comment { get; set; }
        public string Date { get; set; }
        public string UserEmail { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public bool isActive { get; set; }

    }
}