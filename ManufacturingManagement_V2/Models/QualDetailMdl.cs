using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class QualDetailMdl
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
        [Display(Name = "Qualification")]
        public int QualId { get; set; }

        [Display(Name = "Qualification")]
        public string Qualification { get; set; }//d
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Passing Year")]
        public int PassingYear { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Institute")]
        public string Institute { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Univ./Board")]
        public string UnivBoard { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Division %")]
        public double Division { get; set; }

    }
}