using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserRoles.Models
{
    public class Decor
    {
        [Key]
        [Display(Name = "ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DecorCode { get; set; }
        [Required]
        [Display(Name = "Decor Item Name")]
        [MinLength(3)]
        [MaxLength(255)]
        public string DecorName { get; set; }

        [Required]
        [ForeignKey("Category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        
        [DataType(DataType.MultilineText)]
        [MinLength(3)]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public double Price { get; set; }

        public byte[] Image { get; set; }

        public string ImageType { get; set; }



        public bool IsActive { get; set; }

        public virtual Category Category { get; set; }
    }
}