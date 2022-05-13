using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class VendorAddressMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int VendorAddId { get; set; }

        [Display(Name = "Vendor Id")]
        public int VendorId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }//d

        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Address")]
        public string VAddress { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Contact Number")]
        public string ContactNo { get; set; }

        [Display(Name = "Email")]
        public string EMail { get; set; }

        [Display(Name = "Fax Number")]
        public string FaxNo { get; set; }

        [Display(Name = "State Name")]
        public string StateName { get; set; }

        [Display(Name = "State Code")]
        public string StateCode { get; set; }

        [Display(Name = "GSTIN")]
        public string GSTinNo { get; set; }
    }
}