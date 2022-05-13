using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ItemReceiptMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }
        //
        [Display(Name = "Indent Id")]
        public int IndentId { get; set; }

        [Display(Name = "Indent No")]
        public string StrIndentNo { get; set; }

        [Display(Name = "Indent Date")]
        public string IndentDate { get; set; }
        //
        [Display(Name = "Order Id")]
        public int OrderId { get; set; }

        [Display(Name = "Order No")]
        public string StrOrderNo { get; set; }

        [Display(Name = "Order Date")]
        public string OrderDate { get; set; }
        //

        public int IndentLgrId { get; set; }
        public int OrderLgrId { get; set; }

        public int ItemId { get; set; }

        [Display(Name = "Item")]
        public string ItemCode { get; set; }

        [Display(Name = "Item Desc.")]
        public string ItemDesc { get; set; }

        [Display(Name = "B. Qty")]
        public double PurchaseBalance { get; set; }

        [Display(Name = "Unit")]
        public string UnitName { get; set; }

        public double Qty { get; set; }

        [Display(Name = "Purchase No")]
        public string PurchaseNo { get; set; }

        [Display(Name = "P. Date")]
        public string PurchaseDateStr { get; set; }

        [Display(Name = "Receipt No")]
        public string VNo { get; set; }

        [Display(Name = "Receipt Date")]
        public string VDateStr { get; set; }

        public StockLedgerMdl StkLgr { get; set; }
    }
}