using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Microsoft.Ajax.Utilities;

namespace UserRoles.Models
{
    public class ItemsHire
    {
        [Key]
        [Display(Name = "ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        [Required]
        [DisplayName("Name of Product")]
        public string ProductName { get; set; }


        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        //[Required]
        //[ForeignKey("Category")]
        //[Display(Name = "Category")]
        //public int CategoryId { get; set; }

        [Required]
        [ForeignKey("ProductType")]
        [Display(Name = "Product Type")]
        public int ProductCategoryId { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "value must be above 0 ")]
        public int? Quantity { get; set; }
        [Display(Name = "Update Product Quantity")]
        [Range(0, 10000, ErrorMessage = "value must be above 0 ")]
        public int? QtyUpdate { get; set; }
        [Required]
        public decimal Price { get; set; }
        public byte[] Image { get; set; }

        public string ImageType { get; set; }  
      //public virtual IEnumerable<Image> {get;set;}
        public virtual Category Category { get; set; }
        public virtual ProductType ProductType { get; set; }
       // public virtual InvoiceVM InvoiceVM { get; set; }
    }
}