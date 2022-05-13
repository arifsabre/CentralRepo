using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class VoucherDetailMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VType")]
        public string VType { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VNo")]
        public string VNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime VDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AcCode")]
        public int AcCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AcContra")]
        public int AcContra { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Amount")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DrCr")]
        public string DrCr { get; set; }

    }
}