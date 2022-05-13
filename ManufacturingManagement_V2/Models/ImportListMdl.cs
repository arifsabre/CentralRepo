using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ImportListMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ImpId")]
        public int ImpId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Import Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime ImpDate { get; set; }

        //[Required(ErrorMessage = "Required!")]
        [Display(Name = "ItemId")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ItemCode")]
        public string ItemCode { get; set; }//d

        [Display(Name = "Unit")]
        public string UnitName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Quantity")]
        public double Qty { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        //rpt added fields

        [Display(Name = "CurrentStock")]
        public double CurrentStock { get; set; }

        [Display(Name = "TenderQty")]
        public double TenderQty { get; set; }

        [Display(Name = "OpeningStock")]
        public double OpeningStock { get; set; }

        [Display(Name = "TotalStock")]
        public double TotalStock { get; set; }

        [Display(Name = "DelvQty")]
        public double DelvQty { get; set; }

        [Display(Name = "BalanceQty")]
        public double BalanceQty { get; set; }

    }
}