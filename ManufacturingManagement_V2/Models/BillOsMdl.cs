using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class BillOsMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public string RecId { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "compcode")]
        public int CCode { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "finyear")]
        public string FinYr { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VType")]
        public string VType { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VNo")]
        public string VNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VDate")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string VDate { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AcCode")]
        public int AcCode { get; set; }

        [Display(Name = "AcDesc")]
        public string AcDesc { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "BillType")]
        public string BillType { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "BillNo")]
        public string BillNo { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "BillAmount")]
        public double BillAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "BillDate")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string BillDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DueDate")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string DueDate { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DrCr")]
        public string DrCr { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DrAmount")]
        public double DrAmount { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CrAmount")]
        public double CrAmount { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Narration")]
        public string Narration { get; set; }

        //
        [Display(Name = "Received")]
        public double Received { get; set; }

        [Display(Name = "Balance")]
        public double Balance { get; set; }
        //
    
    }
}