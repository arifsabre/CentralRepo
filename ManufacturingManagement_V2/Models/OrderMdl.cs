using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class OrderMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "OrderId")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Order No")]
        public int OrderNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Order Date")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]//ok working
        public string OrderDate { get; set; }//note DateTime

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Vendor")]
        public int VendorId { get; set; }

        [Display(Name = "Delv. Schedule")]
        public string DelvSchedule { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Delv. Date")]
        public string DelvDate { get; set; }//note DateTime

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Order Type")]
        public int OrderTypeId { get; set; }

        [Display(Name = "Spl. Instruction")]
        public string SpecialInst { get; set; }

        [Display(Name = "Ref. Detail")]
        public string RefDetail { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Project")]//get/change its definition from tbl_order
        public string ItemCategory { get; set; }

        [Display(Name = "Revision No")]
        public string RevisionNo { get; set; }

        [Display(Name = "Packing")]
        public string Packing { get; set; }

        [Display(Name = "Excise")]
        public string Excise { get; set; }

        [Display(Name = "GST")]
        public string SaleTax { get; set; }

        [Display(Name = "Transport Mode")]
        public string TrpMode { get; set; }

        [Display(Name = "Freight")]
        public string Freight { get; set; }

        [Display(Name = "Insurance")]
        public string Insurance { get; set; }

        [Display(Name = "Delv. Place")]
        public string DelvPlace { get; set; }

        [Display(Name = "Inspection")]
        public string Inspection { get; set; }

        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; }

        [Display(Name = "TDS")]
        public string TDS { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Net Amount")]
        public double NetAmount { get; set; }

        [Display(Name = "Order Amount")]
        public string OrderAmount { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Currency")]
        public string Currency { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Vendor Address")]
        public int VendorAddId { get; set; }

        [Display(Name = "Vendor")]
        public string VendorName { get; set; }//d

        [Display(Name = "Order Type")]
        public string OrderTypeName { get; set; }//d

        [Display(Name = "Fin Year")]
        public string FinYear { get; set; }//d

        [Display(Name = "Indent Ids")]
        public string IndentIds { get; set; }//d

        [Display(Name = "Indent No")]
        public string IndentNo{ get; set; }//d

        public bool SaveAsNew { get; set; }

        [Display(Name = "IsCancelled")]
        public bool IsCancelled { get; set; }

        [Display(Name = "Order Date")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]//ok working
        public string CancelledOn { get; set; }//note DateTime

        [Display(Name = "Reason")]
        public string Reason { get; set; }

        public List<OrderLedgerMdl> Ledgers { get; set; }
    }
}