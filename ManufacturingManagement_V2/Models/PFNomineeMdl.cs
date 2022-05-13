﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class PFNomineeMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Display(Name = "NewEmpId")]
        public int NewEmpId { get; set; }

        [Display(Name = "Employee")]
        public string EmpId { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Member's Name")]
        public string MemberName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Age")]
        public int Age { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Relation")]
        public string Relation { get; set; }

        [Display(Name = "Address")]
        public string MAddress { get; set; }

        [Display(Name = "Share%")]
        public double SharePercent { get; set; }

        [Display(Name = "Guardian")]
        public string GuardianDetail { get; set; }

        [Display(Name = "NomineeFor")]
        public string NomineeFor { get; set; }

        [Display(Name = "NomineeForName")]
        public string NomineeForName { get; set; }//d

    }
}