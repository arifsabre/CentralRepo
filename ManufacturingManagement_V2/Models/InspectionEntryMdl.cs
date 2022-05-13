using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class InspectionEntryMdl
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
        [Display(Name = "Inspetion Qty")]
        public double InspQty { get; set; }

        [Display(Name = "UnitName")]
        public string UnitName { get; set; }//d

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

    }
}