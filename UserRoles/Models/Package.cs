using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace UserRoles.Models
{
    public class Package
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PackageId { get; set; }
        [Required]
        [Display(Name ="package name")]
        public string packageName { get; set; }
        [Required]
        [Display(Name ="cost")]
        public decimal Price { get; set; }
        [AllowHtml]
        [Required]
        [Display(Name = "Package description")]
        public string Description { get; set; }
        [Display(Name ="upload image")]
        public byte[] imagePack { get; set; }

        [Display(Name ="Active or Not")]
        public bool IsActive { get; set; }

        [Required]
        [ForeignKey("Category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<PackageImages> PackageImages { get; set; }
        //public virtual ICollection<BookEvent> BookEvents { get; set; }

    }
}