using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class EmployeeOpeningMdl
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

        [Display(Name = "Employee")]
        public string EmpName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VType")]
        public string VType { get; set; }

        [Display(Name = "VType")]
        public string VTypeName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime VDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Days Worked")]
        public double DaysWorked { get; set; }

        //note: instead of bonus amt, used as 
        //adjusted pl by 14/05/2019
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Adjusted PL")]
        public double Earning { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Remaining PL")]
        public double RemainingPL { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Earned PL")]
        public double EarnedPL { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PL Encashment")]
        public double PLEncashment { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Encashment Amount")]
        public double EncAmount { get; set; }

    }
}