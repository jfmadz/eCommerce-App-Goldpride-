using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserRoles.Models
{
    public class ProductType
    {
        [Key]
        [Display(Name = "ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductCategoryId { get; set; }
        [Required]
        [Display(Name = "Product Category")]
        [MinLength(3), MaxLength(80)]
        public string ProductCategory_Name { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        [MinLength(3), MaxLength(255)]
        public string Category_Description { get; set; }

        public virtual ICollection<ItemsHire> ItemsHires { get; set; }

    }
}