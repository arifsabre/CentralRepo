using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ExperienceDetailMdl
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

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company Name")]
        public string FirmName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Date From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Date To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Profile")]
        public string JobDesc { get; set; }

    }
}