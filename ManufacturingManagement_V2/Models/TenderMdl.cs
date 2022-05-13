using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class TenderMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TenderId { get; set; }
        
        [Display(Name = "Tender No")]
        public string TenderNo { get; set; }

        [Display(Name = "Serial No")]
        public string SerialNo { get; set; }

        [Display(Name = "Tender Type")]
        public string TenderType { get; set; }

        [Display(Name = "Receiving Date")]
        public string RecDate { get; set; }

        [Display(Name = "PO Type")]
        public string POType { get; set; }

        [Display(Name = "Opening Date")]
        public string OpeningDate { get; set; }

        [Display(Name = "Opening Time")]
        public string OpeningTime { get; set; }
        public string Remarks { get; set; }

        [Display(Name = "Quotation No")]
        public string QuotationNo { get; set; }

        [Display(Name = "Quotation Date")]
        public string QuotationDate { get; set; }

        [Display(Name = "Quotation Date")]
        public string QuotationDateStr { get; set; }//d

        public int RailwayId { get; set; }

        [Display(Name = "Railway")]
        public string RailwayName { get; set; }//d

        [Display(Name = "Established Product")]
        public string EstProduct { get; set; }

        [Display(Name = "Product Description")]
        public string ProductDesc { get; set; }

        [Display(Name = "Drawing/Spec. Changed")]
        public string DrgSpecChanged { get; set; }

        [Display(Name = "Delivery Schedule")]
        public string DelvSchedule { get; set; }

        [Display(Name = "Mode of Dispatch")]
        public string ModeOfDisp { get; set; }

        [Display(Name = "Inspecting Authority")]
        public string InspAuthority { get; set; }

        [Display(Name = "Clarification Required")]
        public string Clarification { get; set; }
        public string TenderStatus { get; set; }

        [Display(Name = "Tender Status")]
        public string StatusDesc { get; set; }//d

        [Display(Name = "Payment Terms")]
        public string TC1PmtTerms { get; set; }
        public string AalCo { get; set; }
        public string AalCoName { get; set; }//d
        public string LoaNumber { get; set; }
        public string LoaDateStr { get; set; }//d
        public double SdBgAmount { get; set; }
        public Boolean IsReTender { get; set; }
        public string LoaDelvSchedule { get; set; }
        public string TCFileNo { get; set; }
        public string ItemsQty { get; set; }//d
        public string PONumber { get; set; }//d
        public string PODateStr { get; set; }//d
        public string Step1 { get; set; }//d
        public string Step2 { get; set; }//d
        public string Step3 { get; set; }//d
        public string LoaInfo { get; set; }//d
        public List<TenderDetailMdl> Ledgers { get; set; }

    }

}