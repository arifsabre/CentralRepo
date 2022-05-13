using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class TenderDispatchMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TenderId")]
        public int TenderId { get; set; }

        [Display(Name = "TenderNo")]
        public string TenderNo { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ItemId")]
        public int ItemId { get; set; }

        [Display(Name = "ItemCode")]
        public string ItemCode { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Qty")]
        public double Qty { get; set; }

        [Display(Name = "UnitName")]
        public string UnitName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DelvDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime DelvDate { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string DetailInfo { get; set; }

    }

    public class TenderItemsMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TenderId")]
        public int TenderId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ItemId")]
        public int ItemId { get; set; }

        [Display(Name = "ItemCode")]
        public string ItemCode { get; set; }//d

        [Display(Name = "ShortName")]
        public string ShortName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Qty")]
        public double Qty { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "OurQty")]
        public double OurQty { get; set; }

        [Display(Name = "UnitName")]
        public string UnitName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Rate")]
        public double Rate { get; set; }

        [Display(Name = "DispCount")]
        public int DispCount { get; set; }

    }

}