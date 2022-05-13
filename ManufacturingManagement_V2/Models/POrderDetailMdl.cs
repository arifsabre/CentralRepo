using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class POrderDetailMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ItemRecId { get; set; }
        public int POrderId { get; set; }
        public int TenderId { get; set; }//d
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }//d
        public string ShortName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Item Description")]
        public string ItemName { get; set; }
        public int Unit { get; set; }
        public string UnitName { get; set; }//d
        public int ConsigneeId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Consignee")]
        public string ConsigneeName { get; set; }//d

        [Display(Name = "Order Qty")]
        public double OrdQty { get; set; }
        public double DspQty { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }

        [Display(Name = "Discount/Unit")]
        public double Discount { get; set; }
        
        [Display(Name = "Excise Duty %")]
        public double ExciseDutyPer { get; set; }

        [Display(Name = "Excise Duty Amount")]
        public double ExciseDutyAmount { get; set; }

        [Display(Name = "SGST %")]
        public double VATPer { get; set; }

        [Display(Name = "SGST Amount")]
        public double VATAmount { get; set; }

        [Display(Name = "CGST %")]
        public double SATPer { get; set; }

        [Display(Name = "CGST Amount")]
        public double SATAmount { get; set; }

        [Display(Name = "IGST %")]
        public double CSTPer { get; set; }

        [Display(Name = "IGST Amount")]
        public double CSTAmount { get; set; }

        [Display(Name = "Entry Tax %")]
        public double EntryTaxPer { get; set; }

        [Display(Name = "Entry Tax Amount")]
        public double EntryTaxAmount { get; set; }

        [Display(Name = "Freight Rate")]
        public double FreightRate { get; set; }

        [Display(Name = "Freight Amount")]
        public double FreightAmount { get; set; }

        [Display(Name = "Net Amount")]
        public double NetAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Billing Unit")]
        public string BillingUnit { get; set; }

        [Display(Name = "Bill Passing Officer")]
        public string BillPO { get; set; }

        [Display(Name = "Inspection Clause")]
        public string InspClause { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DP Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
         ApplyFormatInEditMode = true)]
        public DateTime DelvDate { get; set; }

        [Display(Name = "Transit Depot")]
        public string TransitDepot { get; set; }

        [Display(Name = "Mode Of Dispatch")]
        public string ModeOfDisp { get; set; }

        [Display(Name = "Delivery Terms")]
        public string DelvTerms { get; set; }
        public string DlvStatus { get; set; }

        [Display(Name = "Status")]
        public string DlvStatusName { get; set; }//d
        public bool IsVerified { get; set; }

        public string Remarks { get; set; }

        [Display(Name = "Way Bill")]
        public string WayBill { get; set; }

        [Display(Name = "OEC")]
        public string OCE { get; set; }

        [Display(Name = "Verbal Commitment")]
        public string VerbalCommitment { get; set; }

        [Display(Name = "Delay Reason")]
        public string DelayReason { get; set; }

        [Display(Name = "Actual DP")]
        public DateTime ActualDP { get; set; }

        [Display(Name = "Non-GST")]
        public bool IsNonGst { get; set; }

        [Display(Name = "GST Inclusive")]
        public bool IsInclGst { get; set; }

        [Display(Name = "Unit Rate")]
        public double UnitRate { get; set; }

        [Display(Name = "PO Item No")]
        public int ItemSlNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Commencement Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
         ApplyFormatInEditMode = true)]
        public DateTime CommDate { get; set; }
        public double ExQty { get; set; }//?

        [System.Web.Mvc.AllowHtml]
        public string POItemsHtml { get; set; }//d
        public int RailwayId { get; set; }//d
        public string POInfo { get; set; }//d
    }
}