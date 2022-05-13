using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AdvanceMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "NewEMPId")]
        public int NewEmpId { get; set; }

        [Display(Name = "Employee")]
        public string EmpId { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime AdvDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Advance")]
        public double AdvAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Received")]
        public double RecAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Installment Amt")]
        public double InstAmount { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        #region additional fields

        [Display(Name = "EMP Name")]
        public string EmpName { get; set; }

        [Display(Name = "Grade")]
        public string Grade { get; set; }

        [Display(Name = "Grade")]
        public string GradeName { get; set; }

        [Display(Name = "Joining Unit")]
        public int JoiningUnit { get; set; }

        [Display(Name = "Joining Unit")]
        public string JoiningUnitName { get; set; }

        [Display(Name = "Working Unit")]
        public int WorkingUnit { get; set; }

        [Display(Name = "Working Unit")]
        public string WorkingUnitName { get; set; }

        [Display(Name = "Category")]
        public string CategoryId { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        public string Designation { get; set; }

        #endregion

    }
}