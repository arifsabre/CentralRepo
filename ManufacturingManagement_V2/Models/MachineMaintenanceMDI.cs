using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Models
{
    public class MachineMaintenanceMDI
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Company")]
        [Display(Name = "Company")]
        public int compcode { get; set; }

        [Required(ErrorMessage = "Machine Required")]
        [Display(Name = "Machine Name")]
        public int machineid { get; set; }

        public string machinename { get; set; }

        [Required(ErrorMessage = "Check Point")]
        [Display(Name = "Check Point")]
        public int checkpointid { get; set; }


        [Required(ErrorMessage = "Schedule")]
        [Display(Name = "Schedule")]
        public int scheduleid { get; set; }
        public int scheduleid1 { get; set; }

        [Required(ErrorMessage = "Assignee")]
        [Display(Name = "Assign By")]
        public string assignee { get; set; }

        [Required(ErrorMessage = "Responsibility ?")]
        [Display(Name = "Responsibility")]
        public int responsibility { get; set; }

        [Required(ErrorMessage = "Responsibility ?")]
        public string responsibility1 { get; set; }

        [Required(ErrorMessage = "CheckingBy ?")]
        public string CheckingBy { get; set; }


        [Required(ErrorMessage = "Checking Responsibilty")]
        [Display(Name = "Checking Responsibility")]
        public int checkingresponsibilty { get; set; }
        public string checkingresponsibilty1 { get; set; }

        [Required(ErrorMessage = "Detailofworkdone")]
        [Display(Name = "Detail of workdone")]
        public string detailofworkdone { get; set; }

        [Required(ErrorMessage = "Condition")]
        [Display(Name = "Condition")]
        public string condition { get; set; }

        [Required(ErrorMessage = " Enter Maintenance Date?")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Enter Start Date")]
        public string duedate { get; set; }

        [Required(ErrorMessage = "User Name")]
        [Display(Name = "User Name")]
        public string username { get; set; }

        [Required(ErrorMessage = "userid")]
        [Display(Name = "userid")]
        public int userid { get; set; }
        public string ExternalMan { get; set; }
        
        public bool Active { get; set; }
        public string FullName { get; set; }

        [Required(ErrorMessage = "Select Checking By ?")]
        [Display(Name = "NewEmpId")]
        public int NewEmpId { get; set; }

        [Required(ErrorMessage = "Select Checking By ?")]
        [Display(Name = "EmpName")]
        public string EmpName { get; set; }

        [Required(ErrorMessage = "Company")]
        [Display(Name = "Company")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "FullCompany")]
        [Display(Name = "FullCompany")]
        public string cmpname { get; set; }


   

        [Required(ErrorMessage = "Check Point")]
        [Display(Name = "Check Point")]
        public string checkdetail { get; set; }

        [Required(ErrorMessage = "Schedule")]
        [Display(Name = "Schedule")]
        public string schedulename { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
      
        [Required(ErrorMessage = "UpTime:")]
        public string UpTime { get; set; }
       
        [Required(ErrorMessage = "BreakTime:")]
        public string BreakTime { get; set; } 
        public int Doc { get; set; }
        
       public int breakid { get; set; }

        [Required(ErrorMessage = "Please Enter BreakDown Date?")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Break Date")]
        public string breakdate { get; set; }

        [Required(ErrorMessage = "Up-Date?")]
        [DataType(DataType.DateTime)]
      //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Up-Date")]
        public DateTime? upagain { get; set; }


        [Required(ErrorMessage = "Please Enter MachineUp Date?")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public string MachineUp_Date { get; set; }




        [Required(ErrorMessage = "NextDueDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "NextDueDate")]
        public string NextDueDate { get; set; }
       
      
      
        [Required(ErrorMessage = "Break Reason")]
        [Display(Name = "Break Reason")]
        public string breakreason { get; set; }




        public int NotUpdatedSinceDays { get; set; }
       

        [Required(ErrorMessage = "FileId  Required")]
        [Display(Name = "FileId")]
        public int FileId { get; set; }

        [Required(ErrorMessage = "FileName  Required")]
        [Display(Name = "FileName")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "Attached File")]
        [DataType(DataType.Upload)]
        [Display(Name = "Attached File")]
        public List<HttpPostedFileBase> files { get; set; }

        public byte[] FileContent { get; set; }
        public List<MachineMaintenanceMDI> Item_List { get; set; }
        public List<MachineMaintenanceMDI> Item_List1 { get; set; }
        public List<MachineMaintenanceMDI> Item_List2 { get; set; }
        public List<MachineMaintenanceMDI> Item_List3 { get; set; }
        public List<MachineMaintenanceMDI> HistoryItem_List { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DueDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime DueDate1 { get; set; }
    }
   
}

	
	