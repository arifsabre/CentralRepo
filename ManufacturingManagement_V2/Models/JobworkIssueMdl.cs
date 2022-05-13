using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class JobworkIssueMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int DispId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Challan No")]
        public string ChallanNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Challan Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime ChallanDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Entry Type")]
        public string EntryType { get; set; }

        [Display(Name = "Entry Type")]
        public string EntryTypeName { get; set; }//d

        [Display(Name = "RemIssueQty")]
        public double RemIssueQty { get; set; }//d

        [Display(Name = "Challan Date")]
        public string ChallanDateStr { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Vendor Id")]
        public int VendorId { get; set; }

        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Vendor Address")]
        public int VendorAddId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RMItemId")]
        public int RMItemId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Item Code")]
        public string RMItemCode { get; set; }//d

        [Display(Name = "Short Name")]
        public string RMShortName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Process")]
        public string ProcessDesc { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Range(0.01, Double.MaxValue, ErrorMessage 
          = "The field {0} must be greater than {1}.")]
        [Display(Name = "Issued Qty")]
        public double IssuedQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Unit")]
        public int UnitId { get; set; }

        [Display(Name = "Unit")]
        public string RMUnitName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Rate")]
        public double Rate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "HSN Code")]
        public string HSNCode { get; set; }

        [Display(Name = "Approx Value")]//d
        public double ApproxValue { get; set; }

        [Display(Name = "Transport Mode")]
        public string TrpMode { get; set; }

        [Display(Name = "Transport Detail")]
        public string TrpDetail { get; set; }

        [Display(Name = "Notes")]
        public string InvNote { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string ChallanItems { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Cancelled On")]
        public bool IsCancelled { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Cancelled On")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime CancelledOn { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string Reason { get; set; }

    }
}