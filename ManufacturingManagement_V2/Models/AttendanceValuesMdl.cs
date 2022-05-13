using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AttendanceValuesMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AttCode")]
        public string AttCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AttName")]
        public string AttName { get; set; }

    }
}