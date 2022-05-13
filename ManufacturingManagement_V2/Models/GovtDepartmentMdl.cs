using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class GovtDepartmentMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DepId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Department")]
        public string DepName { get; set; }
    }
}