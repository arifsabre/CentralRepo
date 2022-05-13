using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class PurchaseMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PurchaseId")]
        public int PurchaseId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VType")]
        public string VType { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VNo")]
        public int VNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VDate")]
        public string VDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Cash/Credit")]
        public string CashCredit { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Vendor")]
        public int VendorId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SubTotal")]
        public double SubTotal { get; set; }

        [Display(Name = "Discount")]
        public double Discount { get; set; }

        [Display(Name = "SGST Amount")]
        public double SgstAmount { get; set; }

        [Display(Name = "CGST Amount")]
        public double CgstAmount { get; set; }

        [Display(Name = "IGST Amount")]
        public double IgstAmount { get; set; }

        [Display(Name = "Freight Amount")]
        public double FreightAmount { get; set; }

        [Display(Name = "Other Charges")]
        public double OtherCharges { get; set; }

        [Display(Name = "Round Off")]
        public double RoundOffAmt { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "NetAmount")]
        public double NetAmount { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Indent Nos")]
        public string IndentIds { get; set; }

        [Display(Name = "Order Nos")]
        public string OrderIds { get; set; }

        [Display(Name = "Vendor")]
        public string VendorName { get; set; }//d

        [Display(Name = "VType")]
        public string VTypeName { get; set; }//d

        public List<StockLedgerMdl> Ledgers { get; set; }
    }
}