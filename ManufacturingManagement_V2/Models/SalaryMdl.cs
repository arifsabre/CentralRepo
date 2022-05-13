using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class SalaryMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AttendanceId")]
        public int AttendanceId { get; set; }

        [Display(Name = "NewEmpId")]
        public int NewEmpId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "EMP Code")]
        public string EmpId { get; set; }//d

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
        [Display(Name = "BasicRate")]
        public double BasicRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "BasicEarn")]
        public double BasicEarn { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DARate")]
        public double DARate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DAEarn")]
        public double DAEarn { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ConvRate")]
        public double ConvRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ConvEarn")]
        public double ConvEarn { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "HraRate")]
        public double HraRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "HraEarn")]
        public double HraEarn { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "MedAllowRate")]
        public double MedAllowRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "MedAllowEarn")]
        public double MedAllowEarn { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CompAllowRate")]
        public double CompAllowRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CompAllowEarn")]
        public double CompAllowEarn { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DressWashRate")]
        public double DressWashRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DressWashEarn")]
        public double DressWashEarn { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SpecialPayRate")]
        public double SpecialPayRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SpecialPayEarn")]
        public double SpecialPayEarn { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "OthersRate")]
        public double OthersRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "OthersPayEarn")]
        public double OthersPayEarn { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "GrossSalary")]
        public double GrossSalary { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "IncHours")]
        public double IncHours { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TotalEarned")]
        public double TotalEarned { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PFRate")]
        public double PFRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PFDeduction")]
        public double PFDeduction { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "FPF")]
        public double FpfAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "EPF")]
        public double EpfAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ESIRate")]
        public double ESIRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ESIDeduction")]
        public double ESIDeduction { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AdvDeduction")]
        public double AdvDeduction { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "FineDeduction")]
        public double FineDeduction { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ShortLeaveRate")]
        public double ShortLeaveRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ShortLeaveDed")]
        public double ShortLeaveDed { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "LateAttRate")]
        public double LateAttRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "LateAttDed")]
        public double LateAttDed { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TDSRate")]
        public double TDSRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TDSDeduction")]
        public double TDSDeduction { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TotalDeduction")]
        public double TotalDeduction { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "IsPaymentAck")]
        public bool IsPaymentAck { get; set; }

        //used in processing to keep empid
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "IsProcessed")]
        public bool IsProcessed { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "NetPaid")]
        public double NetPaid { get; set; }

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

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "IncAmount")]
        public double IncAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "IncEarn")]
        public double IncEarn { get; set; }

        #endregion

    }
}