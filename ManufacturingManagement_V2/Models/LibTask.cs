using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Models
{
    public class LibTask
    {
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Task  Is  Required")]
        public string Task { get; set; }

        [Required(ErrorMessage = "Task_Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? TaskDate { get; set; }
              
       
      
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreatedOn { get; set; }

      
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? UpdatedOn { get; set; }

       // public int Status { get; set; }
        public string TaskStatus { get; set; }

        public int  UserId  { get; set; }
        public string UserName { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }

        public int TotalTask { get; set; }

        public List<LibTask> TaskList { get; set; }
        public List<LibTask> Tasktotal { get; set; }

        [Required(ErrorMessage = "Priority  Is  Required")]
        public int PId { get; set; }
        public string Priority { get; set; }

        [Required(ErrorMessage = "Status  Is  Required")]
        public int SId { get; set; }
        public string Status { get; set; }


        public LibTask()
        {
            this.CopStatus = new List<SelectListItem>();
            this.CopPrority = new List<SelectListItem>();

        }
       public List<SelectListItem> CopStatus { get; set; }
        public List<SelectListItem> CopPrority { get; set; }
        public List<LibTask> Item_List { get; set; }


        public string NoticeId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime NoticeDT { get; set; }
        public string NoticeMsg { get; set; }


    }
}