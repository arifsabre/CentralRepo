using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AttendanceBatchMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Year")]
        public int AttYear { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Month")]
        public int AttMonth { get; set; }

        [Display(Name = "Month")]
        public string MonthName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Shift")]
        public string AttShift { get; set; }

        [Display(Name = "Shift")]
        public string ShiftName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Day")]
        public int AttDay { get; set; }

        [Display(Name = "Unit")]
        public int JoiningUnit { get; set; }

        [Display(Name = "Unit")]
        public string UnitName { get; set; }

        public string Grade { get; set; }

        [Display(Name = "Grade")]
        public string GradeName { get; set; }

        public List<EmployeeAttendanceMdl> EmpAttendance { get; set; }

    }

    public class EmployeeAttendanceMdl
    {
        public string EmpId { get; set; }//d
        public string NewEmpId { get; set; }
        public string EmpName { get; set; }
        public string FatherName { get; set; }
        public string AttValue { get; set; }
        public string remainingCL { get; set; }
    }

}