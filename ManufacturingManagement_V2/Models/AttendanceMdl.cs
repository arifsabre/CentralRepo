using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AttendanceMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AttendanceId")]
        public int AttendanceId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Month")]
        public int AttMonth { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Year")]
        public int AttYear { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Shift")]
        public string AttShift { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Day")]
        public int AttDay { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Attendance For")]
        public string AttValue { get; set; }

        [Display(Name = "NewEmpId")]
        public int NewEmpId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "EMP Code")]
        public string EmpId { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D01")]
        public string D01 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D02")]
        public string D02 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D03")]
        public string D03 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D04")]
        public string D04 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D05")]
        public string D05 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D06")]
        public string D06 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D07")]
        public string D07 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D08")]
        public string D08 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D09")]
        public string D09 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D10")]
        public string D10 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D11")]
        public string D11 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D12")]
        public string D12 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D13")]
        public string D13 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D14")]
        public string D14 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D15")]
        public string D15 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D16")]
        public string D16 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D17")]
        public string D17 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D18")]
        public string D18 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D19")]
        public string D19 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D20")]
        public string D20 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D21")]
        public string D21 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D22")]
        public string D22 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D23")]
        public string D23 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D24")]
        public string D24 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D25")]
        public string D25 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D26")]
        public string D26 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D27")]
        public string D27 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D28")]
        public string D28 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D29")]
        public string D29 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D30")]
        public string D30 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D31")]
        public string D31 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D0X")]
        public string D0X { get; set; }
        
        #region additional fields
        
        [Display(Name = "Month")]
        public string MonthName { get; set; }

        [Display(Name = "Shift")]
        public string ShiftName { get; set; }

        [Display(Name = "EMP Name")]
        public string EmpName { get; set; }

        [Display(Name = "Father's Name")]
        public string FatherName { get; set; }

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

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Allow Modify")]
        public string AllowModify { get; set; }//to be worked for bool

        [Display(Name = "Remaining CL")]
        public double RemainingCL { get; set; }

        #endregion

    }
}