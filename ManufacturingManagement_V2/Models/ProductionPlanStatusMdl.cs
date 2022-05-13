using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ProductionPlanStatusMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

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
        public string ItemCode { get; set; }

        [Display(Name = "ShortName")]
        public string ShortName { get; set; }

        [Display(Name = "PrdQty")]
        public double PrdQty { get; set; }

        [Display(Name = "InspQty")]
        public double InspQty { get; set; }

        [Display(Name = "ProducedQty")]
        public double ProducedQty { get; set; }

        [Display(Name = "InspectedQty")]
        public double InspectedQty { get; set; }

        [Display(Name = "PrvWipQty")]
        public double PrvWipQty { get; set; }

        [Display(Name = "PrvPrdQty")]
        public double PrvPrdQty { get; set; }

        [Display(Name = "PrvInspQty")]
        public double PrvInspQty { get; set; }

        [Display(Name = "PrvProducedQty")]
        public double PrvProducedQty { get; set; }

        [Display(Name = "PrvInspectedQty")]
        public double PrvInspectedQty { get; set; }

        [Display(Name = "M1DPQty")]
        public double M1DPQty { get; set; }

        [Display(Name = "M2DPQty")]
        public double M2DPQty { get; set; }

        [Display(Name = "M3DPQty")]
        public double M3DPQty { get; set; }

        [Display(Name = "M4DPQty")]
        public double M4DPQty { get; set; }

        [Display(Name = "M5DPQty")]
        public double M5DPQty { get; set; }

        [Display(Name = "M6DPQty")]
        public double M6DPQty { get; set; }

        [Display(Name = "M7DPQty")]
        public double M7DPQty { get; set; }

        [Display(Name = "M8DPQty")]
        public double M8DPQty { get; set; }

        [Display(Name = "M9DPQty")]
        public double M9DPQty { get; set; }

        [Display(Name = "M10DPQty")]
        public double M10DPQty { get; set; }

        [Display(Name = "M11DPQty")]
        public double M11DPQty { get; set; }

        [Display(Name = "M12DPQty")]
        public double M12DPQty { get; set; }

        [Display(Name = "M13DPQty")]
        public double M13DPQty { get; set; }

        [Display(Name = "M14DPQty")]
        public double M14DPQty { get; set; }

        [Display(Name = "M15DPQty")]
        public double M15DPQty { get; set; }

        [Display(Name = "UnitName")]
        public string UnitName { get; set; }

        //d
        [Display(Name = "PrvMonth")]
        public string PrvMonth { get; set; }

        [Display(Name = "Month1")]
        public string Month1 { get; set; }

        [Display(Name = "Month2")]
        public string Month2 { get; set; }

        [Display(Name = "Month3")]
        public string Month3 { get; set; }
        [Display(Name = "Month4")]
        public string Month4 { get; set; }

        [Display(Name = "Month5")]
        public string Month5 { get; set; }

        [Display(Name = "Month6")]
        public string Month6 { get; set; }

        [Display(Name = "Month7")]
        public string Month7 { get; set; }

        [Display(Name = "Month8")]
        public string Month8 { get; set; }

        [Display(Name = "Month9")]
        public string Month9 { get; set; }

        [Display(Name = "Month10")]
        public string Month10 { get; set; }

        [Display(Name = "Month11")]
        public string Month11 { get; set; }

        [Display(Name = "Month12")]
        public string Month12 { get; set; }

        [Display(Name = "Month13")]
        public string Month13 { get; set; }

        [Display(Name = "Month14")]
        public string Month14 { get; set; }

        [Display(Name = "Month15")]
        public string Month15 { get; set; }

        public bool IsAutoPlanned { get; set; }

    }
}