using iText.IO.Source;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UserRoles.Models
{
    public class PackageImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID   { get; set; }
         public byte[] Image { get; set; }
        public int packageId { get; set; }

        public virtual Package Package { get; set; }
    }
}