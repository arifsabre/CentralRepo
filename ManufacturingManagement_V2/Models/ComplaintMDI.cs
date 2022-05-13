using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Models
{
    public class ComplaintMDI
    {
        public int Reference { get; set; }

        [Required(ErrorMessage = "Location is Required:")]
        public int Compcode { get; set; }

        public string cmpname { get; set; }

       
        [Required(ErrorMessage = "Description is Required:")]
        public string Description { get; set; }

      
       
        public string Category { get; set; }

        [Required(ErrorMessage = "Category is Required:")]
        public int CatId { get; set; }

        [Required(ErrorMessage = "Priority is Required:")]
        public int PID { get; set; }
        public string Priority { get; set; }

        public string Status { get; set; }

        [Required(ErrorMessage = "Status is Required:")]
        public int SID { get; set; }

        [Required(ErrorMessage = "Assin To is Required:")]
        public int NewEmpId { get; set; }
        public string EmpName { get; set; }

        public string username { get; set; }
        public  int userid { get; set; }

        //[DataType(DataType.Date)]
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedOn { get; set; }

       // [DataType(DataType.Date)]
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime UpdatedOn { get; set; }
        public DateTime UpdatedOn2 { get; set; }
        public DateTime UpdatedOn3 { get; set; }
        public DateTime UpdatedOn4 { get; set; }
        public DateTime ClosedDate { get; set; }

        public int SmsStatus { get; set; }
        public int EmailStatus { get; set; }
        public string Reark { get; set; }
        public string UpdatedBy { get; set; }

        public int hodid { get; set; }
        public string hodname { get; set; }
        public string Subject { get; set; }
        public string createdby { get; set; }
        public string ApprovedBy { get; set; }
        public string HODApproved { get; set; }
        public string FinalApproved { get; set; }

        



        public ComplaintMDI()
        {
            this.CopCategory = new List<SelectListItem>();
            this.CopStatus = new List<SelectListItem>();
            this.tbl_employee = new List<SelectListItem>();
            this.tbl_company = new List<SelectListItem>();
            this.CopPrority = new List<SelectListItem>();

        }
        public List<SelectListItem> CopCategory { get; set; } 
        public List<SelectListItem> CopStatus { get; set; }
        public List<SelectListItem> tbl_employee { get; set; }
        public List<SelectListItem> tbl_company { get; set; }
        public List<SelectListItem> CopPrority { get; set; }
        public List<ComplaintMDI> Item_List { get; set; }

        

    }
}