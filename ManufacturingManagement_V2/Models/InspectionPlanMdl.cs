using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class InspectionPlanMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PRecId")]
        public int PRecId { get; set; }

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
        [Display(Name = "IP Qty")]
        public double IPQty { get; set; }

        [Display(Name = "UnitName")]
        public string UnitName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M2 Qty")]
        public double M2IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M3 Qty")]
        public double M3IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M4 Qty")]
        public double M4IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M5 Qty")]
        public double M5IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M6 Qty")]
        public double M6IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M7 Qty")]
        public double M7IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M8 Qty")]
        public double M8IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M9 Qty")]
        public double M9IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M10 Qty")]
        public double M10IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M11 Qty")]
        public double M11IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M12 Qty")]
        public double M12IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M13 Qty")]
        public double M13IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M14 Qty")]
        public double M14IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "M15 Qty")]
        public double M15IPQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Target Qty")]
        public double TrgQty { get; set; }//d

    }
}