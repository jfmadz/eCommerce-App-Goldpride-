using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserRoles.Models
{
    public class Category
    {
        
        [Display(Name = "ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required]
        [Display(Name ="Event Category")]
        [MinLength(3),MaxLength(80)]
        public string Category_Name { get; set; }
        
        [Display(Name ="Description")]
        [DataType(DataType.MultilineText)]
        [MinLength(3),MaxLength(255)]
        public string Description { get; set; }

        public virtual ICollection<Decor> Decors { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
        public virtual ICollection<Meeting> Meetings { get; set; }
        //public virtual ICollection<BookEvent> BookEvents { get; set; }


    }
}