using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class PromotionDetailMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Display(Name = "NewEmpId")]
        public int NewEmpId { get; set; }

        [Display(Name = "EMP Code")]
        public string EmpId { get; set; }//d

        [Display(Name = "EmpName")]
        public string EmpName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PromotionDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime PromotionDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CategoryId")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DesigId")]
        public int DesigId { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }//d

        [Display(Name = "Designation")]
        public string Designation { get; set; }//d

    }
}