using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class JobworkReceiptMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DispId")]
        public int DispId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Receiving Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime RecDate { get; set; }

        [Display(Name = "Receiving Date")]
        public string RecDateStr { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Invoice No")]
        public string InvNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Invoice Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime InvDate { get; set; }

        [Display(Name = "Invoice Date")]
        public string InvDateStr { get; set; }//d

        [Display(Name = "FGItemId")]
        public int FGItemId { get; set; }

        [Display(Name = "Processed Item")]
        public string FGItemCode { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Processed Qty")]
        public double FgItemQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Processed Item Unit")]
        public int UnitId { get; set; }

        [Display(Name = "Unit")]
        public string FGUnitName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Received Qty")]
        public double ReceivedQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Waste Qty")]
        public double WasteQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Inv. Waste Qty")]
        public double InvWQty { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        //display fields from jw issue

        [Display(Name = "Challan No")]
        public string ChallanNo { get; set; }//d

        [Display(Name = "Challan Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime ChallanDate { get; set; }//d

        [Display(Name = "Challan Date")]
        public string ChallanDateStr { get; set; }//d

        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }//d

        [Display(Name = "RM Item Code")]
        public string RMItemCode { get; set; }//d

        [Display(Name = "RM Short Name")]
        public string RMShortName { get; set; }//d

        [Display(Name = "Issued Qty")]
        public double IssuedQty { get; set; }//d

        [Display(Name = "RM Unit")]
        public string RMUnitName { get; set; }//d

        [Display(Name = "Balance Qty")]
        public double RemainingQty { get; set; }//d

        [Display(Name = "Process")]
        public string ProcessDesc { get; set; }//d

    }
}