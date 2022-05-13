using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class IMTETypeMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int IMTETypeId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "IMTE Type Name")]
        public string IMTETypeName { get; set; }
    }
}