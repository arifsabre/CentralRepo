using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class IndentLedgerMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }

        [Display(Name = "IndentId")]
        public int IndentId { get; set; }

        [Display(Name = "StrIndentNo")]
        public string StrIndentNo { get; set; }

        [Display(Name = "VNo")]
        public int VNo { get; set; }

        [Display(Name = "VDate")]
        public string VDate { get; set; }

        [Display(Name = "IndentBy")]
        public int IndentBy { get; set; }

        [Display(Name = "IndentByName")]
        public string IndentByName { get; set; }

        [Display(Name = "HODUserId")]
        public int HODUserId { get; set; }

        [Display(Name = "HODUserName")]
        public string HODUserName { get; set; }

        [Display(Name = "ItemSlNo")]
        public int ItemSlNo { get; set; }

        [Display(Name = "ItemId")]
        public int ItemId { get; set; }

        [Display(Name = "ItemCode")]
        public string ItemCode { get; set; }

        [Display(Name = "ItemDesc")]
        public string ItemDesc { get; set; }

        [Display(Name = "IndQty")]
        public double IndQty { get; set; }

        [Display(Name = "UnitId")]
        public int UnitId { get; set; }

        [Display(Name = "UnitName")]
        public string UnitName { get; set; }//d

        [Display(Name = "AppQty")]
        public double AppQty { get; set; }

        [Display(Name = "ExpectedDT")]
        public string ExpectedDT { get; set; }

        [Display(Name = "ExpectedDTStr")]
        public string ExpectedDTStr { get; set; }

        [Display(Name = "StockQty")]
        public double StockQty { get; set; }

        [Display(Name = "IssuedQty")]
        public double IssuedQty { get; set; }

        [Display(Name = "PurchaseMode")]
        public string PurchaseMode { get; set; }

        [Display(Name = "PurchaseModeName")]
        public string PurchaseModeName { get; set; }

        [Display(Name = "PRequiredQty")]
        public double PRequiredQty { get; set; }

        [Display(Name = "PurchasedQty")]
        public double PurchasedQty { get; set; }

        [Display(Name = "ApproxRate")]
        public double ApproxRate { get; set; }

        [Display(Name = "PurchaseRate")]
        public double PurchaseRate { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "IssueBalance")]
        public double IssueBalance { get; set; }

        [Display(Name = "PurchaseBalance")]
        public double PurchaseBalance { get; set; }

        [Display(Name = "SlipNo")]
        public int SlipNo { get; set; }

        [Display(Name = "VendorId")]
        public int VendorId { get; set; }

        [Display(Name = "VendorName")]
        public string VendorName { get; set; }

        [Display(Name = "IndentStatus")]
        public string IndentStatus { get; set; }

        [Display(Name = "IndentStatusName")]
        public string IndentStatusName { get; set; }

        [Display(Name = "HODApprovalOn")]
        public string HODApprovalOn { get; set; }

        [Display(Name = "HODApprovalOnStr")]
        public string HODApprovalOnStr { get; set; }

        [Display(Name = "AdminUserId")]
        public int AdminUserId { get; set; }

        [Display(Name = "AdminUserName")]
        public string AdminUserName { get; set; }

        [Display(Name = "AdminApprovalOn")]
        public string AdminApprovalOn { get; set; }

        [Display(Name = "AdminApprovalOnStr")]
        public string AdminApprovalOnStr { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AdminQuery")]
        public string AdminQuery { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "HODReply")]
        public string HODReply { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "HODRemarks")]
        public string HODRemarks { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "IndentorReply")]
        public string IndentorReply { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AdminRemarks")]
        public string AdminRemarks { get; set; }

        [Display(Name = "ExecutedBy")]
        public int ExecutedBy { get; set; }

        [Display(Name = "ExecutedByName")]
        public string ExecutedByName { get; set; }

        [Display(Name = "ExecutedOn")]
        public string ExecutedOn { get; set; }

        [Display(Name = "ExecutedOnStr")]
        public string ExecutedOnStr { get; set; }

        [Display(Name = "PurchaseStatus")]
        public string PurchaseStatus { get; set; }

        [Display(Name = "IssueStatus")]
        public string IssueStatus { get; set; }

        [Display(Name = "compcode")]
        public int CompCode { get; set; }

        [Display(Name = "finyear")]
        public string FinYear { get; set; }

        [Display(Name = "StkRecid")]
        public int StkRecId { get; set; }//d

        [Display(Name = "ItemValue")]
        public double ItemValue { get; set; }//d

        [Display(Name = "AproxValue")]
        public double ApproxValue { get; set; }//d

    }
}