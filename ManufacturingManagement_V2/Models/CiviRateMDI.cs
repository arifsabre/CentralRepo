using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class CiviRateMDI
    {
        public int RecordId { get; set; }
       
        [Required(ErrorMessage = " Chapter No Required!")]
        [Display(Name = "Chapter No ")]
        public string Category { get; set; }
        public int CategoryId { get; set; }

        [Required(ErrorMessage = " Chapter Name Required!")]
        [Display(Name = "Enter Chapter Name")]
        public string SubCategory { get; set; }
        public int SubCategoryId { get; set; }
        
        [Required(ErrorMessage = " Description Required!")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = " Unit Required!")]
        [Display(Name = "Unit")]
         public string Unit { get; set; }
        public int UnitId { get; set; }

        [Required(ErrorMessage = "Rate Required!")]
        [Display(Name = "Rate")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "Amount Required!")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        public int UserId { get; set; }
        public int UpdatedBy { get; set; }

        public string UpdatedOnString { get; set; }

        public string Remark { get; set; }

        public string CreatedOnString { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedOn { get; set; }

       [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}",ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }
        public List<CiviRateMDI> Item_List { get; set; }
        public List<CiviRateMDI> Item_List1 { get; set; }
        public List<CiviRateMDI> Item_List2 { get; set; }
    }
}