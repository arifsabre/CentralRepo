using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AAA_TodayPrsentAbsentstatusMDI
    {
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalEmployee { get; set; }
        
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string FatherName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime AnnivDate { get; set; }
        public int UpcomingAniversary { get; set; }



        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Birthday { get; set; }
        public int birthdayafter { get; set; }
              

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime JoiningDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Retirementdate { get; set; }
        public int retiredindays { get; set; }
        
        
        
    }
}