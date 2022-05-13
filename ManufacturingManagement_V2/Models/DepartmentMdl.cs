using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class DepartmentMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public string DepCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Department")]
        public string Department { get; set; }
    }
}