using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UserRoles.Models
{
    public class NumberOfGuests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name ="Number")]
        public int Key { get; set; }
        public int NumberOfPeople { get; set; }

        public virtual ICollection<BookEvent> BookEvents { get; set; }
    }
}