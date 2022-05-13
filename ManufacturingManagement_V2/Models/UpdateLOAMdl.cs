using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class UpdateLOAMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TenderId")]
        public int TenderId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TenderNo")]
        public string TenderNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ItemCode")]
        public string ItemCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ShortName")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ConsigneeName")]
        public string ConsigneeName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "OfferedQty")]
        public double OfferedQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "BasicRate")]
        public double BasicRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Tax%")]
        public double SaleTaxPer { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "LOA Qty")]
        public double LoaQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "LOA Rate")]
        public double LoaRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "LOA Number")]
        public string LoaNumber { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AalCo")]
        public string AalCo { get; set; }

        [Display(Name = "AAL/CO")]
        public string AalCoName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "LOA Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime LoaDate { get; set; }

        [Display(Name = "LoaDateStr")]
        public string LoaDateStr { get; set; }//d

        //
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "LOA Amt")]//for item-consg
        public double LoaAmt { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CalcLoaAmount")]//sum(for item-consg)
        public double CalcLoaAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SD/BG Amount")]//for Tender
        public double SdBgAmount { get; set; }

        [Display(Name = "Delv. Schedule")]//for tender
        public string DelvSchedule { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "LOA Delv. Schedule")]//for tender
        public string LoaDelvSchedule { get; set; }

        [Display(Name = "Case File No")]//for tender
        public string TCFileNo { get; set; }

    }
}