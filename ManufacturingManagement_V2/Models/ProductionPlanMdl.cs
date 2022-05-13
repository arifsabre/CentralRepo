using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ProductionPlanMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PlanRecId")]
        public int PlanRecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Month")]
        public int PPMonth { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Year")]
        public int PPYear { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ItemId")]
        public int ItemId { get; set; }

        [Display(Name = "ItemCode")]
        public string ItemCode { get; set; }//d

        [Display(Name = "ShortName")]
        public string ShortName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Prd. Qty")]
        public double PrdQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Insp. Qty")]
        public double InspQty { get; set; }

        [Display(Name = "UnitName")]
        public string UnitName { get; set; }//d

    }
}