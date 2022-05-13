using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class IndentPurchaseSlipMdl
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PSRecId { get; set; }
        //
        [Display(Name = "Vendor")]
        public int VendorId { get; set; }

        [Display(Name = "Vendor")]
        public string VendorName { get; set; }

        [Display(Name = "SlipNo")]
        public int SlipNo { get; set; }

        public List<PurchaseSlipItemMdl> Ledgers { get; set; }
    }

    public class PurchaseSlipItemMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }

        [Display(Name = "VendorId")]
        public int VendorId { get; set; }

        [Display(Name = "Vendor")]
        public string VendorName { get; set; }

        [Display(Name = "IndentId")]
        public int IndentId { get; set; }

        [Display(Name = "IndentNo")]
        public string StrIndentNo { get; set; }

        [Display(Name = "ItemId")]
        public int ItemId { get; set; }

        [Display(Name = "ItemDesc")]
        public string ItemDesc { get; set; }

        [Display(Name = "Balance Qty")]
        public double PurchaseBalance { get; set; }//d

        [Display(Name = "UnitId")]
        public int UnitId { get; set; }

        [Display(Name = "ApproxRate")]
        public double ApproxRate { get; set; }

        [Display(Name = "TaxPer")]
        public double TaxPer { get; set; }

        [Display(Name = "ItemCode")]
        public string ItemCode { get; set; }//d

        [Display(Name = "Item")]
        public string ItemName { get; set; }//d

        [Display(Name = "Unit")]
        public string UnitName { get; set; }//d

        public bool chkItem { get; set; }//1-set,0-reset

        [Display(Name = "AdminRemarks")]
        public string AdminRemarks { get; set; }//d

        [Display(Name = "IndentByName")]
        public string IndentByName { get; set; }//d

        [Display(Name = "IndentLgrId")]
        public int IndentLgrId { get; set; }//d

    }

}