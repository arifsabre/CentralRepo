using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class CalibrationMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ImteId")]
        public int ImteId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Calibration Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime CalibDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Certificate No")]
        public string CertificateNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Certified By")]
        public string CertifiedBy { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Next Calib. Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime NextCalibDate { get; set; }

        [Display(Name = "Is Tested")]
        public bool IsTested { get; set; }

        [Display(Name = "TestRecId")]
        public int TestRecId { get; set; }

        [Display(Name = "Id No")]
        public string IdNo { get; set; }//d

        [Display(Name = "IMTE Type")]
        public int ImteTypeId { get; set; }//d

        [Display(Name = "IMTE Type")]
        public string ImteTypeName { get; set; }//d

        [Display(Name = "Range")]
        public string ImteRange { get; set; }//d

        [Display(Name = "Location")]
        public string Location { get; set; }//d

        [Display(Name = "Status")]
        public bool IsInUse { get; set; }//d

    }
}