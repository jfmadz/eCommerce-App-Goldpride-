using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace UserRoles.Models
{
    public class Meeting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int messageID { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required]
        public string Date { get; set; }

        //[Required]
        [ForeignKey("Category")]
        [Display(Name = "Event Type")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public System.DateTime Start { get; set; }

        //[Required]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public System.DateTime End { get; set; }

        [Required(ErrorMessage ="Please Enter All Attendees")]
        [Display(Name ="Attendees")]
         public string Attend { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Please Enter a description of what was discussed")]
        [Display(Name = "Voice to Text")]
        public string Discussion { get; set; }

        [AllowHtml]
        [Display(Name ="Discussion")]
        public string ckeditor { get; set; }


        public virtual Category Category { get; set; }

    }
}