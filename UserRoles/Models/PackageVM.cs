using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UserRoles.Models
{
    public class PackageVM
    {
        [Key]
        public int PackageVMID { get; set; }
        public int PackageId    { get; set; }
        public string packageName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public int CategoryId { get; set; }

        public int ImageID { get; set; }
        public byte[] Image { get; set; }
        //public int packId { get; set; }

    }
}