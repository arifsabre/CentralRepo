using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class BomMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Finished Item")]
        public int FgItemId { get; set; }

        [Required(ErrorMessage = "Required!")]//note
        [Display(Name = "FgItemCode")]
        public string FgItemCode { get; set; }//d

        [Display(Name = "FgItemName")]
        public string FgItemName { get; set; }//d

        [Display(Name = "Item Type")]
        public string ItemTypeDesc { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Raw Material")]
        public int RmItemId { get; set; }

        [Required(ErrorMessage = "Required!")]//note
        [Display(Name = "RmItemCode")]
        public string RmItemCode { get; set; }//d

        [Display(Name = "RmItemName")]
        public string RmItemName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "R.M. Qty")]
        public double RmQty { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Waste Qty")]
        public double WasteQty { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Revision No")]
        public int RevNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime RevDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Revision Update")]
        public bool RevisionUpdate { get; set; }

        //d
        [Display(Name = "F.I. Unit")]
        public int FgUnit { get; set; }

        [Display(Name = "F.I. Unit")]
        public string FgUnitName { get; set; }

        [Display(Name = "R.M. Unit")]
        public int RmUnit { get; set; }

        [Display(Name = "R.M. Unit")]
        public string RmUnitName { get; set; }
        //end-d

        public List<BomMdl> BomList { get; set; }
    }
}