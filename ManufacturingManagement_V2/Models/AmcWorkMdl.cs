using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AmcWorkMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AmcId")]
        public int AmcId { get; set; }

        [Display(Name = "Challan No")]
        public int ChallanNo { get; set; }

        public string ChallanNoStr { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "POrderId")]
        public int POrderId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PO Number")]
        public string PONumber { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ConsigneId")]
        public int ConsigneeId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Consignee")]
        public string ConsigneeName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Challan Date")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}",
        //ApplyFormatInEditMode = true)]
        public string VDate { get; set; }

        [Display(Name = "Transport Mode")]
        public string TrpMode { get; set; }

        [Display(Name = "Transport Detail")]
        public string TrpDetail { get; set; }

        [Display(Name = "Through")]
        public string Through { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "No of Packages")]
        public string NoOfPckg { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Notes")]
        public string InvNote { get; set; }//d

        [Display(Name = "Sub Total")]
        public double SubTotal { get; set; }

        [Display(Name = "SGST Amount")]
        public double VatAmount { get; set; }

        [Display(Name = "CGST Amount")]
        public double SatAmount { get; set; }

        [Display(Name = "IGST Amount")]
        public double CstAmount { get; set; }

        [Display(Name = "Net Amount")]
        public double NetAmount { get; set; }

        [Display(Name = "Rev Chg. Amount")]
        public double RevChgTaxAmount { get; set; }

        [Display(Name = "Items")]
        public string Items { get; set; }//d

        public string FlName { get; set; }//file upload
        public byte[] FileContent { get; set; }//file upload

        public List<AmcDetailMdl> Ledgers { get; set; }

    }
}