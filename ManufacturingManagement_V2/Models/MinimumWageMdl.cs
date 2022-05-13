using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class MinimumWageMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Year")]
        public int AttYear { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Month")]
        public int AttMonth { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Skilled")]
        public double Skilled { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Semi Skilled")]
        public double SemiSkilled { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Un Skilled")]
        public double UnSkilled { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Bonus %")]
        public double BonusPer { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company")]
        public int CompCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company")]
        public string CmpName { get; set; }//d

        [Display(Name = "Set To All")]
        public bool SetToAll { get; set; }
    }
}