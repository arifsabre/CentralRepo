using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class RfqTenderMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RfqId")]
        public int RfqId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Reference No")]
        public string ReferenceNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Offer Type")]
        public string OfferType { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Offer Type")]
        public string OfferTypeName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RFQ Date")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}",
        //ApplyFormatInEditMode = true)]
        public string RfqDate { get; set; }

        [Display(Name = "Quotation No")]
        public string QuotationNo { get; set; }//d

        [Display(Name = "Quotation Date")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}",
        //ApplyFormatInEditMode = true)]
        public string QuotationDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Railway")]
        public int RailwayId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Railway")]
        public string RailwayName { get; set; }//d

        [Display(Name = "Established Product")]
        public string EstProduct { get; set; }

        [Display(Name = "Product Description")]
        public string ProductDesc { get; set; }

        [Display(Name = "Drg. Spec. Changed")]
        public string DrgSpecChanged { get; set; }

        [Display(Name = "Mode Of Disp.")]
        public string ModeOfDisp { get; set; }

        [Display(Name = "Inspection Authority")]
        public string InspAuthority { get; set; }

        [Display(Name = "Payment Terms")]
        public string TC1PmtTerms { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Items")]
        public string Items { get; set; }//d

        public string FlName { get; set; }//file upload
        public byte[] FileContent { get; set; }//file upload

        public List<RfqTenderDetailMdl> Ledgers { get; set; }

    }
}