using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingManagement_V2.Models
{
    public class CompanyMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CompCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company Name")]
        public string CmpName { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }

        [Display(Name = "TIN")]
        public string TinNo { get; set; }

        [Display(Name = "TIN Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime TinDate { get; set; }

        [Display(Name = "Phone (Off.)")]
        public string PhoneOff { get; set; }

        [Display(Name = "Phone (Resi.)")]
        public string PhoneResi { get; set; }

        [Display(Name = "Fax No")]
        public string FaxNo { get; set; }

        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }

        [Display(Name = "Contact Person")]
        public string ContPer { get; set; }

        public string EMail { get; set; }
        public string Website { get; set; }
        public string Footer1 { get; set; }
        public string Footer2 { get; set; }
        public string Footer3 { get; set; }
        public string Footer4 { get; set; }
        public string Footer5 { get; set; }
        public string Footer6 { get; set; }
        public string Footer7 { get; set; }
        public string Footer8 { get; set; }
        public string Footer9 { get; set; }
        public string Footer10 { get; set; }
        public string CIN { get; set; }
        public string CerEccNo { get; set; }

        [Display(Name = "CST No")]
        public string CstNo { get; set; }

        [Display(Name = "CST Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime CstDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        [Display(Name = "Tender C.Code")]
        public string TdCmpCode { get; set; }

        [Display(Name = "PAN")]
        public string PanNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Display Index")]
        public string DispIndex { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "GSTIN")]
        public string GSTinNo { get; set; }

        [Display(Name = "GSTIN Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime GSTinDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "State Code")]
        public string StateCode { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Display(Name = "Account No")]
        public string AccountNo { get; set; }

        [Display(Name = "IFSC Code")]
        public string IfscCode { get; set; }

        [Display(Name = "Employer Address")]
        public string EmployerAddress { get; set; }

        [Display(Name = "Manager Name")]
        public string ManagerName { get; set; }

        [Display(Name = "Manager Address")]
        public string ManagerAddress { get; set; }

        [Display(Name = "PF No")]
        public string PFNo { get; set; }

        [Display(Name = "ESIC No")]
        public string ESICNo { get; set; }

        [Display(Name = "Factory Reg. No")]
        public string FRegNo { get; set; }

        public string SalaryAccountNo { get; set; }
    }

    public class FinYearMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }
        public int CompCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string FinYear { get; set; }
        public string LongFinYear { get; set; }
    }

}