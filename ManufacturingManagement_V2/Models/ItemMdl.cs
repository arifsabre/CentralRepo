using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ItemMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }

        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Sale Rate")]
        public double SaleRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Purchase Rate")]
        public double PurchaseRate { get; set; }

        public string Specification { get; set; }

        public string Remarks { get; set; }

        [Required(ErrorMessage = "Required!")]
        public int Unit { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Item Type")]
        public string ItemType { get; set; }

        [Display(Name = "HSN Code")]
        public string HSNCode { get; set; }

        public int OldItemId { get; set; }

        [Display(Name = "Group")]
        public int GroupId { get; set; }

        [Display(Name = "Unit")]
        public string UnitName { get; set; }//d	

        [Display(Name = "VType")]
        public string VTypeName { get; set; }//d

        [Display(Name = "Group")]
        public string GroupName { get; set; }//d

        [Display(Name = "UpdateUnit")]
        public bool updateUnit { get; set; }//d

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Display(Name = "MSB Qty")]
        public double MsbQty { get; set; }

        [Display(Name = "Lot Size")]
        public double LotSize { get; set; }

        [Display(Name = "Warranty Days")]
        public int WarrantyDays { get; set; }

    }
}