using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class PurchaseOrderMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int POrderId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PO Number")]
        public string PONumber { get; set; }

        public int TenderId { get; set; }

        public int AcCode { get; set; }

        [Display(Name = "Order Placing Authority")]
        public string AcDesc { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PO Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
         ApplyFormatInEditMode = true)]
        public DateTime PODate { get; set; }
        public string PODateStr { get; set; }//d
        public double POValue { get; set; }
        public double Qty { get; set; }

        [Display(Name = "Payment Terms")]
        public string PaymentMode { get; set; }

        [Display(Name = "Quotation No")]
        public string QuotationNo { get; set; }

        [Display(Name = "Free At Station")]
        public string FreeAtStation { get; set; }
        public string OrderStatus { get; set; }
        public string HdnOrderStatus { get; set; }//note

        [Display(Name = "Order Status")]
        public string OrderStatusName { get; set; }
        public int PayingAuthId { get; set; }

        [Display(Name = "Paying Authority")]
        public string PayingAuthName { get; set; }//d

        [Display(Name = "MA Reason")]
        public string MAReason { get; set; }
        public string Remarks { get; set; }

        [Display(Name = "PO Address")]
        public int CompAddId { get; set; }

        [Display(Name = "Order Description")]
        public string TenderDesc { get; set; }

        [Display(Name = "Terms and Conditions")]
        public string TermsCondition { get; set; }

        [Display(Name = "Correctio Required")]
        public bool CorrectionRequired { get; set; }

        [Display(Name = "Tender No")]
        public string TenderNo { get; set; }//d
        public int RailwayId { get; set; }

        [Display(Name = "Railway")]
        public string RailwayName { get; set; }//d

        [Display(Name = "PO Type")]
        public string POType { get; set; }
        public string POTypeName { get; set; }//d
        public string PmtStatus { get; set; }

        [Display(Name = "30% Incremental Order")]
        public bool IsIncOrder { get; set; }

        [Display(Name = "PO Verified")]
        public bool IsPOVerified { get; set; }

        [Display(Name = "Case File No")]
        public string CaseFileNo { get; set; }
        public int WForPOrderId { get; set; }

        [Display(Name = "Warranty For PO")]
        public string WForPONumber { get; set; }//d
        public string TCFileNo { get; set; }//d-from tender
        
        public string EntryType { get; set; }

        [Display(Name = "Case Closed")]
        public bool IsCaseClosed { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Closure Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
         ApplyFormatInEditMode = true)]
        public DateTime ClosureDate { get; set; }
        public string ItemsQty { get; set; }
        public string POInfo { get; set; }
        public List<ModifyAdviceMdl> ModifyAdvList { get; set; }
        public List<POrderDetailMdl> Ledgers { get; set; }

        [Display(Name = "Product Description")]
        public string ProductDescChk { get; set; }

        [Display(Name = "Drawing/Specification")]
        public string DrgSpcChk { get; set; }

        [Display(Name = "Basic Rate")]
        public string BasicRateChk { get; set; }

        [Display(Name = "Taxes")]
        public string TaxesChk { get; set; }

        [Display(Name = "Payment Term")]
        public string PmtTermsChk { get; set; }

        [Display(Name = "Inspection By")]
        public string InspByChk { get; set; }

        [Display(Name = "Consignee Details")]
        public string ConsigneeChk { get; set; }

        [Display(Name = "Delivery Schedule")]
        public string DelvScheduleChk { get; set; }

        [Display(Name = "Consignee-Qty-DP")]
        public string ConsgQtyDPChk { get; set; }

        [Display(Name = "Mode of Dispatch")]
        public string DispatchModeChk { get; set; }

        [Display(Name = "BG/Security Applicable")]
        public string BGSecurityChk { get; set; }

        [Display(Name = "Any Other Point")]
        public string AnyOtherChk { get; set; }

    }

}