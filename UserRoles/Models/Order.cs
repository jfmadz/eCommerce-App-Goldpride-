using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Configuration;
using PayPal.Api;

namespace UserRoles.Models
{
    //[Bind(Exclude ="OrderId")]
    public  class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }
        public string OrderName { get; set; }
        public string Distance { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string PaymentType { get; set; }

        public string DriverID { get; set; }


        [DisplayName("Order Acknowledged")]
        public bool Seen { get; set; }

        [DisplayName("Items Collected By Customer")]
        public bool PickUp { get; set; }

        [DisplayName("Out For Delivery")]
        public bool OutForDel { get; set; }

        [DisplayName("Items Delivered To Customer")]
        public bool Delivered { get; set; }

        [DisplayName("Item Returned By Customer")]
        public bool Returned { get; set; }

        [DisplayName("Out For Retrieval Of Items")]
        public bool OutForCol { get; set; }

        [DisplayName("Items Retrieved")]
        public bool Collected { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DisplayName("Name")]
        [StringLength(160)]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Surname is required")]
        [DisplayName("Surname")]
        [StringLength(160)]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Please enter your PhoneNumber")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string CustomerPhone { get; set; }
        [DisplayName("Email Address")]

        [Required(ErrorMessage = "Email is is not valid.")]

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name ="Date when items are required")]
        //[Required(ErrorMessage = "Collection date is required")]
        //[StringLength(70)]
        public DateTime CollDate { get; set; }
        //[Required(ErrorMessage ="Please enter your address")]
        //[Display(Name ="Address")]
        //[StringLength(255)]
        //public string Address { get; set; }

        [Display(Name ="Expected return date ")]
        public System.DateTime ExpectedReturnDate { get; set; }

        [Display(Name ="Actual Date of Return")]
        public System.DateTime ActualReturnDate { get; set; }

        public DateTime ReminderDate { get; set; }//Collection

        public DateTime SelfReturnReminder { get; set; }

        [Display(Name ="Number of days late")]
        public int? DateDiff { get; set; }
        [UIHint("SignaturePad")]
        public byte[] MySignature { get; set; }

        [UIHint("SignaturePad")]
        public byte[] ConSig { get; set; }

        public int QuantReturned { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        //public virtual ICollection<Map> Maps { get; set; }


    }
}