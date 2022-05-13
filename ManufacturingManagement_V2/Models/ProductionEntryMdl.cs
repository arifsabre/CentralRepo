using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ProductionEntryMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PlanRecId")]
        public int PlanRecId { get; set; }

        [Display(Name = "Month-Year")]
        public string MonthYear { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Entry Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime EntryDate { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ItemId")]
        public int ItemId { get; set; }//d

        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }//d

        [Display(Name = "ShortName")]
        public string ShortName { get; set; }//d
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Production Qty")]
        public double PrdQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Rejected Qty")]
        public double RejQty { get; set; }

        [Display(Name = "Confirming Qty")]
        public double ConfQty { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "To RG1")]
        public double ToRG1 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "To Jobwork")]
        public double ToJobwork { get; set; }

        [Display(Name = "WIP Qty")]
        public double WIP { get; set; }//d
        [Display(Name = "UnitName")]
        public string UnitName { get; set; }//d

        [Display(Name = "Rejection Reason")]
        public string RejReason { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

    }
}