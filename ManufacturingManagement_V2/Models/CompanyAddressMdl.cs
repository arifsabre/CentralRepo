using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class CompanyAddressMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company")]
        public int CompCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company")]
        public string CmpName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Address1")]
        public string Address1 { get; set; }

        [Display(Name = "Address2")]
        public string Address2 { get; set; }

        [Display(Name = "Address3")]
        public string Address3 { get; set; }

        [Display(Name = "CAddress")]
        public string CAddress { get; set; }

        [Display(Name = "Address Name")]
        public string AddressName { get; set; }

        [Display(Name = "PIN")]
        public string PinCode { get; set; }

        [Display(Name = "State Name")]
        public string StateName { get; set; }

        [Display(Name = "State Code")]
        public string StateCode { get; set; }

        public List<CompanyAddressMdl> AddressList { get; set; }

    }
}