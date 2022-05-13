using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class BankGuaranteeMdl
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TenderId")]
        public int TenderId { get; set; }

        [Display(Name = "TenderNo")]
        public string TenderNo { get; set; }//d

        [Display(Name = "OpeningDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime OpeningDate { get; set; }//d

        [Display(Name = "AAL/CO")]
        public string AalCo { get; set; }//d

        [Display(Name = "LOA Number")]
        public string LoaNumber { get; set; }//d

        [Display(Name = "LOA Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime LoaDate { get; set; }//d

        [Display(Name = "Railway")]
        public string Railway { get; set; }//d

        [Display(Name = "PODetail")]
        public string PODetail { get; set; }//d

        [Display(Name = "LOADetail")]
        public string LoaDetail { get; set; }//d

        [Display(Name = "BGNumber")]
        public string BGNumber { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SD/BG Amount")]
        public double SdBgAmount { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "BGDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime BGDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Validity")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime Validity { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Extended Validity")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime ExtValidity { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "StatusId")]
        public int StatusId { get; set; }

        [Display(Name = "BG Status")]
        public string StatusName { get; set; }//d

        [Display(Name = "Delv. Schedule")]
        public string DelvSchedule { get; set; }//d

        [Display(Name = "LOA Delv. Schedule")]
        public string LoaDelvSchedule { get; set; }//d

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "BG Type")]
        public string BGType { get; set; }

        [Display(Name = "BG Type")]
        public string BGTypeName { get; set; }//d

        [Display(Name = "History Available")]
        public bool HistoryAvailable { get; set; }//d

        [Display(Name = "Railway")]
        public string RailwayName { get; set; }//d

        public int editTenderId { get; set; }
        public bool editMode { get; set; }

        public bool sendToHistory { get; set; }//c

    }
}