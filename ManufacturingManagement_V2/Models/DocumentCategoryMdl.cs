using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class DocumentCategoryMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CategoryId")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CategoryName")]
        public string CategoryName { get; set; }

    }
}