using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserRoles.Models
{
    public class Location
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name ="Hall")]
        public int locationID { get; set; }
        [Required]
        [Display(Name = "Area")]
        public string Area { get; set; }
        [Required]
        [Display(Name = "Hall Name")]
        public string hallName { get; set; }
        [Required]
        [Display(Name="Capacity")]
        public int maxCapacity { get; set; }

        public virtual ICollection<BookEvent> BookEvents { get; set; }
    }
}