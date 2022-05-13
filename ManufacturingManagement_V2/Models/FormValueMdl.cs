using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class FormValueMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company")]
        public int CompCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Fin. Year")]
        public string FinYear { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "NetProfit")]
        public double NetProfit { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PrvYrBonusAmt")]
        public double PrvYrBonusAmt { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DepreciationAmt")]
        public double DepreciationAmt { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RateOfDevRebate")]
        public double RateOfDevRebate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CapitalAmtPer")]
        public double CapitalAmtPer { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CapitalAmount")]
        public double CapitalAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RevSurpAmtPer")]
        public double RevSurpAmtPer { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RevSurpAmount")]
        public double RevSurpAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SurpAllocPer")]
        public double SurpAllocPer { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "BonusPer")]
        public double BonusPer { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "BonusAmount")]
        public double BonusAmount { get; set; }

        [Display(Name = "CompanyName")]
        public string CompanyName { get; set; }//d

        [Display(Name = "FinYearToDate")]
        public DateTime FinYearToDate { get; set; }//d

        [Display(Name = "NoOfMales")]
        public int NoOfMales { get; set; }

        [Display(Name = "NoOfFemales")]
        public int NoOfFemales { get; set; }

        [Display(Name = "NoOfWDays")]
        public int NoOfWDays { get; set; }

        [Display(Name = "PayableBonusAmt")]
        public double PayableBonusAmt { get; set; }

        [Display(Name = "PaymentDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }

    }
}