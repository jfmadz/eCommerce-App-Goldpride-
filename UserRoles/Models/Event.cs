using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UserRoles.Models
{
    public class Event
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }

        [Required]
        [DisplayName("Event Type")]
        public string EventType { get; set; }

        [Required]
        [DisplayName("Full Name")]
        public string Fname { get; set; }

        [Required]
        [DisplayName("Start")]
        public System.DateTime Start { get; set; }

        [Required]
        [DisplayName("End")]
        public System.DateTime End { get; set; }

        public DateTime MReminder { get; set; }

        public DateTime Reminder { get; set; }

        [Required]
        [Phone]
        [DisplayName("Tel No.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid Number")]
        public string contactNum { get; set; }

        [Required]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}