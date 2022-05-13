using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AccountMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CompCode")]
        public int CompCode { get; set; }

        [Display(Name = "Company")]
        public string CmpName { get; set; }//d
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Account")]
        public int AcCode { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Account Name")]
        public string AcDesc { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecType")]
        public string RecType { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Cash")]
        public string Cash { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Group")]
        public int GrCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Group Name")]
        public string GrDesc { get; set; } //d
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AcType")]
        public string AcType { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Level")]
        public int Lev { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "BSheet")]
        public bool BSheet { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Modify")]
        public bool Modif { get; set; }
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }
        [Display(Name = "Alias")]
        public string AliasName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Opn. Balance")]
        public double OpBalance { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Dr/Cr")]
        public string DrCr { get; set; }

        public AccountAddressMdl Address { get; set; }

        [Display(Name = "RailwayId")]
        public int RailwayId { get; set; }

        [Display(Name = "Railway")]
        public string Railway { get; set; }//d

    }

    public class AccountAddressMdl
    {
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Display(Name = "Account Hindi")]
        public string HAcDesc { get; set; }

        [Display(Name = "Contact Person")]
        public string ContPer { get; set; }

        [Display(Name = "Address1")]
        public string Address1 { get; set; }

        [Display(Name = "Address2")]
        public string Address2 { get; set; }

        [Display(Name = "Address3")]
        public string Address3 { get; set; }

        [Display(Name = "Address4")]
        public string Address4 { get; set; }

        [Display(Name = "TIN")]
        public string TinNo { get; set; }

        [Display(Name = "Tin Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime TinDate { get; set; }

        [Display(Name = "Phone Off")]
        public string PhoneOff { get; set; }

        [Display(Name = "Phone Resi")]
        public string PhoneResi { get; set; }

        [Display(Name = "Fax No")]
        public string FaxNo { get; set; }

        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Area Id")]
        public int AreaId { get; set; }

        [Display(Name = "Cr Limit")]
        public double CrLimit { get; set; }

        [Display(Name = "Party Discount")]
        public double PartyDiscount { get; set; }

        [Display(Name = "Cr Days")]
        public int CrDays { get; set; }

        [Display(Name = "CustomerType")]
        public string CustomerType { get; set; }

        [Display(Name = "CST No")]
        public string CstNo { get; set; }

        [Display(Name = "CST Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime CstDate { get; set; }

        [Display(Name = "CategoryId")]
        public int CategoryId { get; set; }

        [Display(Name = "RailwayId")]
        public int RailwayId { get; set; }

        [Display(Name = "Railway")]
        public string Railway { get; set; }//d

        [Display(Name = "GSTIN")]
        public string GSTinNo { get; set; }

        [Display(Name = "State Code")]
        public string StateCode { get; set; }

        [Display(Name = "State Name")]
        public string StateName { get; set; }

    }
}