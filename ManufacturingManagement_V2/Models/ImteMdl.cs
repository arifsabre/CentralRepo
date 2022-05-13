using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ImteMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ImteId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Id No")]
        public string IdNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "IMTE Type")]
        public int ImteTypeId { get; set; }

        [Display(Name = "IMTE Type")]
        public string ImteTypeName { get; set; }//d

        [Display(Name = "Range")]
        public string ImteRange { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Purchase Year")]
        public string PurchaseYear { get; set; }

        [Display(Name = "Least Count")]
        public string LeastCount { get; set; }

        [Display(Name = "Status")]
        public bool IsInUse { get; set; }

        public string InUseStatus { get; set; }

    }
}