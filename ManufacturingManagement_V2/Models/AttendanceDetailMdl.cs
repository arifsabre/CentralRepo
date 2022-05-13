using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AttendanceDetailMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime AttDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Shift")]
        public string AttShift { get; set; }

        [Display(Name = "NewEmpId")]
        public int NewEmpId { get; set; }

        [Display(Name = "EMP Code")]
        public string EmpId { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Attendance For")]
        public string AttValue { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Range(0, 24, ErrorMessage = "Invalid hours!")]
        [Display(Name = "Duration")]
        public double AttHours { get; set; }

        //--start--for batchwise
        [Display(Name = "Month")]
        public int AttMonth { get; set; }

        [Display(Name = "Year")]
        public int AttYear { get; set; }

        [Display(Name = "Day")]
        public int AttDay { get; set; }
        //--end--for batchwise

        #region additional fields

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

        [Display(Name = "Attendance For")]
        public string AttValueName { get; set; }

        #endregion

    }
}