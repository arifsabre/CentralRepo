using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AttendanceDetailBatchMdl
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

        public string Grade { get; set; }

        [Display(Name = "Grade")]
        public string GradeName { get; set; }

        public List<EmployeeAttendanceDetailMdl> EmpAttendance { get; set; }

    }

    public class EmployeeAttendanceDetailMdl
    {
        public string RecId { get; set; }
        public string EmpId { get; set; }//d
        public string NewEmpId { get; set; }
        public string EmpName { get; set; }
        public string FatherName { get; set; }
        public string AttValue { get; set; }
        public string AttHours { get; set; }
    }

}