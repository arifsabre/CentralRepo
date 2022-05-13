using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class PartyMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RailwayId")]
        public int RailwayId { get; set; }

        [Display(Name = "Railway")]
        public string RailwayName { get; set; }//d

        [Display(Name = "RlyShortName")]
        public string RlyShortName { get; set; }//d

        [Display(Name = "Party Code")]
        public int AcCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Party Name")]
        public string AcDesc { get; set; }

        [Display(Name = "Cont. Person")]
        public string ContPer { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Display(Name = "Address 3")]
        public string Address3 { get; set; }

        [Display(Name = "Address 4")]
        public string Address4 { get; set; }

        [Display(Name = "State Name")]
        public string StateName { get; set; }

        [Display(Name = "State Code")]
        public string StateCode { get; set; }

        [Display(Name = "TIN")]
        public string TinNo { get; set; }

        [Display(Name = "GSTIN")]
        public string GSTinNo { get; set; }

        [Display(Name = "Phone")]
        public string PhoneOff { get; set; }

        [Display(Name = "Fax No")]
        public string FaxNo { get; set; }

        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }

        public string Email { get; set; }

        public List<PartyMdl> PartyList { get; set; }
    }
}