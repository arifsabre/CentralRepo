using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class StockLedgerMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }

        [Display(Name = "VType")]
        public string VType { get; set; }

        [Display(Name = "VNo")]
        public int VNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime VDate { get; set; }

        [Display(Name = "VDate")]
        public string VDateStr { get; set; }//d

        [Display(Name = "ItemId")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ItemDesc")]
        public string ItemDesc { get; set; }

        [Display(Name = "Qty")]
        public double Qty { get; set; }

        [Display(Name = "UnitId")]
        public int UnitId { get; set; }

        [Display(Name = "Rate")]
        public double Rate { get; set; }

        [Display(Name = "Amount")]
        public double Amount { get; set; }

        [Display(Name = "Discount")]
        public double Discount { get; set; }

        [Display(Name = "Total Discount")]
        public double TotalDiscount { get; set; }

        [Display(Name = "SGST%")]
        public double SgstPer { get; set; }

        [Display(Name = "SGST Amount")]
        public double SgstAmount { get; set; }

        [Display(Name = "CGST%")]
        public double CgstPer { get; set; }

        [Display(Name = "CGST Amount")]
        public double CgstAmount { get; set; }

        [Display(Name = "IGST%")]
        public double IgstPer { get; set; }

        [Display(Name = "IGST Amount")]
        public double IgstAmount { get; set; }

        [Display(Name = "Freight Rate")]
        public double FreightRate { get; set; }

        [Display(Name = "Freight Amount")]
        public double FreightAmount { get; set; }

        [Display(Name = "NetAmount")]
        public double NetAmount { get; set; }

        [Display(Name = "IndentLgrId")]
        public int IndentLgrId { get; set; }

        [Display(Name = "OrderLgrId")]
        public int OrderLgrId { get; set; }

        [Display(Name = "PurchaseNo")]
        public string PurchaseNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime PurchaseDate { get; set; }

        [Display(Name = "Purchase Date")]
        public string PurchaseDateStr { get; set; }//d

        public string Remarks { get; set; }

        [Display(Name = "ItemCode")]
        public string ItemCode { get; set; }//d

        [Display(Name = "Unit")]
        public string UnitName { get; set; }//d	

        [Display(Name = "VType")]
        public string VTypeName { get; set; }//d
    }
}