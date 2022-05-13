using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class DocSubCategoryMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SubCategoryId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Sub Category Name")]
        public string SubCategoryName { get; set; }
    }
}