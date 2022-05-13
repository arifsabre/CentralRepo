using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class UnitMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "UnitId")]
        public int Unit { get; set; }//not unitid

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Unit Name")]
        public string UnitName { get; set; }
    }
}