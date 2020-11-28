using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserRoles.Models
{
    public class BookEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "Customer Email")]
        public string CEmail { get; set; }

        [Display(Name = "First Name")]
        public string FName { get; set; }

        [Display(Name = "Last Name")]
        public string LName { get; set; }

        [Display(Name = "Cell Number")]
        public string Cell { get; set; }

        public bool Setup { get; set; }
        [Display(Name ="Number of guests")]
        [Range(50, 300)]
        public int Quantity { get; set; }
        [Display(Name ="Date of Payment")]
        public DateTime DateOfOrder { get; set; }
        public DateTime ReminderDayBefore { get; set; }
        public DateTime Reminder { get; set; }
        //[Required]
        //[Range(1,500)]
        //public int Quantity { get; set; }
        [Required(ErrorMessage ="Please Choose a valid date")]
        public DateTime Date { get; set; }
        //[Required]
        //[ForeignKey("NumberOfGuests")]
        //[Display(Name = "Number of guests")]
        //public int Key { get; set; }
        [Required]
        [ForeignKey("Location")]
        [Display(Name = "Hall Name")]
        public int locationID { get; set; }
        [Display(Name ="Event Complete")]
        public bool Completed { get; set; }
        public virtual NumberOfGuests NumberOfGuests { get; set;}
        public virtual Location Location { get; set; }
        public virtual ICollection<EventOrderDetail> EventOrderDetails { get; set; }
        //[Required]
        //[ForeignKey("Package")]
        //[Display(Name = "Package")]
        //public int PackageId { get; set; }
        //public virtual Package Package { get; set; }
    }
}