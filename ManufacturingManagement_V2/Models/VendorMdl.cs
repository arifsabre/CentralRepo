using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class VendorMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int VendorId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Vendor Name")]
        //working [System.Web.Mvc.AllowHtml]
        [System.Web.Mvc.AllowHtml]
        public string VendorName { get; set; }
    }
}