using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Models
{
    public class AAA_MedicalTest
    {
        public int MedicalId { get; set; }
        public int NewEmpId { get; set; }
        public int UserId { get; set; }
        public string EmpName { get; set; }
        public int Age { get; set; }
        public decimal Weight { get; set; }
        public string Hemoglobin { get; set; }
        public string Pulse { get; set; }
        public string Oxygen { get; set; }
        public string Sugar { get; set; }
        public string EmpId { get; set; }
        public string ReportFormat { get; set; }
        public int AgentId { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime TestDate { get; set; }

        [Display(Name = "BP Systolic")]
        public string Bp { get; set; }
        [Display(Name = "BP Diastolic")]
        public string Bplow { get; set; }
        public string BloodGroup { get; set; }
        public string Height { get; set; }
        public string EyeSight { get; set; }
        public string Allergies { get; set; }
        public string HealthCondition { get; set; }
        public string Remark { get; set; }
        public int UpdateStatus { get; set; }
       

        public List<AAA_MedicalTest> List { get; set; }


        public AAA_MedicalTest()
        {
            this.tbl_employee = new List<SelectListItem>();
            this.tbl_employee1 = new List<SelectListItem>();


        }
        public List<SelectListItem> tbl_employee { get; set; }
        public List<SelectListItem> tbl_employee1 { get; set; }

        public string ToPulse { get; set; }
        public int Compcode { get; set; }
        [Display(Name = "Compamy")]
        public string cmpname { get; set; }
        [Display(Name = "Grade")]
        public string Grade { get; set; }

        public string GradeName { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
       

        public int RecordId { get; set; }
        public int VacId { get; set; }
        public string VacType { get; set; }

        public bool Dose1  { get; set; }
        public bool Dose2 { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Dose1 Date  Required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Dose1Date { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Dose2Date { get; set; }

        [Required(ErrorMessage = "Id  Required")]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "FileName  Required")]
        [Display(Name = "FileName")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "Attached File")]
        [DataType(DataType.Upload)]
        [Display(Name = "Attached File")]
        public List<HttpPostedFileBase> files { get; set; }

        public byte[] FileContent { get; set; }
        public List<AAA_MedicalTest> Item_List { get; set; }

    }
}