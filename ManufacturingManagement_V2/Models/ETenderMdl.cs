using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ETenderMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TenderId { get; set; }
        public bool toAdd { get; set; }//note
        public string TDocCost { get; set; }
        public string TdcmpDDNo { get; set; }
        public string TdcmpDDDate { get; set; }
        public string TdcmpExmp { get; set; }
        public double TDocCostAmt { get; set; }
        public string EMDCost { get; set; }
        public string EmdcDDNo { get; set; }
        public string EmdcDDDate { get; set; }
        public string EmdcExmp { get; set; }
        public double EmdCostAmt { get; set; }
        public string ElgCriteria { get; set; }
        public string ElgCrtRemarks { get; set; }
        public int TC1ValidFrom { get; set; }
        public string TC1FORDest { get; set; }
        public string TC1ModeOfDisp { get; set; }
        public string TC1Insp { get; set; }
        public string TC1DelvPeriod { get; set; }
        public string TC1PmtTerms { get; set; }
        public int TC2ValidFrom { get; set; }
        public string TC2FORDest { get; set; }
        public string TC2ModeOfDisp { get; set; }
        public string TC2Insp { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Delivery Period")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime TC2DelvPeriod { get; set; }
        public string TC2PmtTerms { get; set; }
        public string DelvSchedule { get; set; }
        public string FormDRequired { get; set; }
        public string PerfStmt { get; set; }
        public string DeviationReq { get; set; }
        public string AttachedDocs { get; set; }
        public string UploadedBy { get; set; }
        public string FormatFilledBy { get; set; }
        public string ApprovedBy { get; set; }
        public string Remarks { get; set; }

        public string TenderInfo { get; set; }//d
        public string RailwayName { get; set; }//d
        public string TenderNo { get; set; }//d
        public string TenderType { get; set; }//d
        public DateTime OpeningDate { get; set; }//d
        public string OpeningTime { get; set; }//d
        public string SerialNo { get; set; }//d

    }

}