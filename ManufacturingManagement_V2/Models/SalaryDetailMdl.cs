using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class SalaryDetailMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Display(Name = "NewEMPId")]
        public int NewEmpId { get; set; }

        [Display(Name = "EMP Code")]
        public string EmpId { get; set; }//d

        [Display(Name = "EmpName")]
        public string EmpName { get; set; }//d
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "IncDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime IncDate { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Basic Rate")]
        public double BasicRate { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DA")]
        public double DA { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Conv Allowance")]
        public double ConvAllowance { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "HRA")]
        public double HRA { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Medical Allowance")]
        public double MedicalAllowance { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Comp Allowance")]
        public double CompAllowance { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DW Allowance")]
        public double DWAllowance { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Special Pay")]
        public double SpecialPay { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Others")]
        public double Others { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Gross Salary")]
        public double GrossSalary { get; set; }
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Old Gross")]
        public double OldGross { get; set; }

        [Display(Name = "Increment Amount")]
        public double IncAmount { get; set; }

        [Display(Name = "ESI Applicable")]
        public bool EsiApplicable { get; set; }

        [Display(Name = "PF Deductable")]
        public bool PFDeductable { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TDS Deduction")]
        public double TdsDeduction { get; set; }

    }
}