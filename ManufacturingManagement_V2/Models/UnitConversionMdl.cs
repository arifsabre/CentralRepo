using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class UnitConversionMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Main Unit")]
        public int MainUnit { get; set; }

        [Display(Name = "Main Unit")]
        public string MainUnitName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Alternate Unit")]
        public int AltUnit { get; set; }

        [Display(Name = "Alternate Unit")]
        public string AltUnitName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Conversion Factor")]
        public double ConvFactor { get; set; }

    }
}

