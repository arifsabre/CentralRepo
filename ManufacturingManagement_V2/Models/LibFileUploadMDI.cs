using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Models
{
    public class LibFileUploadMDI
    {
      

        [Required(ErrorMessage = "Location is Required:")]
        public int compcode { get; set; }

        public string cmpname { get; set; }
       
        [Required(ErrorMessage = "Category is Required:")]
        public string LibCategory { get; set; }
      
        [Required(ErrorMessage = "Category is Required:")]
        public int LibCategoryId { get; set; }
       
        [Required(ErrorMessage = "Sub Category is Required:")]
        public string LibSubCategory { get; set; }

        [Required(ErrorMessage = "SubCategory is Required:")]
        public int LibSubCategoryId { get; set; }

        [Required(ErrorMessage = "EmpName is  Required:")]
        public int NewEmpId { get; set; }

        [Required(ErrorMessage = "EmpName is  Required:")]
        public string EmpName { get; set; }

        public string username { get; set; }

        [Required(ErrorMessage = "UserName is Required:")]
        public int userid { get; set; }
        public string FullName { get; set; }

       
       
        [Required(ErrorMessage = "Upload Path is Required:")]
        public string UploadPath { get; set; }

        public HttpPostedFile ImageFile { get; set; }

        public LibFileUploadMDI()
        {
            this.LibraryCategory = new List<SelectListItem>();
            this.LibrarySubCategory = new List<SelectListItem>();

        }

        public List<SelectListItem> LibraryCategory { get; set; }
        public List<SelectListItem> LibrarySubCategory { get; set; }
        public List<LibFileUploadMDI> Item_List { get; set; }
        public List<LibFileUploadMDI> Item_List1 { get; set; }
        public List<LibTask> ItemList { get; set; }


        [Required(ErrorMessage = "Select File")]
        [DataType(DataType.Upload)]
        [Display(Name = "Select File")]
        public List<HttpPostedFileBase> files { get; set; }

        public int Id { get; set; }
        public int RecordId { get; set; }
        [Required(ErrorMessage = "Select File")]
        public string FileName { get; set; }
     


        public byte[] FileContent { get; set; }
      
        public int OwnerId { get; set; }

        [Required(ErrorMessage = "DoccumentName Required")]
        public string DoccumentName { get; set; }

        [Required(ErrorMessage = "DoccumentName Required")]
        public string DuccumentName { get; set; }
        public string DepCode { get; set; }
        public string Department { get; set; }

        public int ReqId { get; set; }
        public string DownloadReason { get; set; }
        public string Approved { get; set; }
        public DateTime RequestOn { get; set; }
        public string RequestedBy { get; set; }
        public string ShortName { get; set; }


        [Required(ErrorMessage = "Type is  Required:")]
        [Display(Name = "Select Type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Description is  Required:")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Location is  Required:")]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Keywords is  Required:")]
        [Display(Name = "Keywords")]
        public string Keyss { get; set; }

        [Required(ErrorMessage = "Owne rName is  Required:")]
        [Display(Name = "Owner Name")]
        public string OwnerName { get; set; }

        [Required(ErrorMessage = "Status is  Required:")]
        [Display(Name = "Status")]
        public string Status { get; set; }


        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }

        public string UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }

        public int IssueTo { get; set; }
        public string IssueToName { get; set; }

        [Required(ErrorMessage = "IssueDate is  Required:")]
        public string IssueDate { get; set; }

        [Required(ErrorMessage = "ReturnDate is  Required:")]
        public string ReturnDate { get; set; }

        public string ReturnBy { get; set; }
        public string ReturnTo { get; set; }

        public string ReturnStatus { get; set; }

        [Required(ErrorMessage = "ConfirmationDate is  Required:")]
        public DateTime ConfirmationDate { get; set; }
        public string Type1 { get; set; }



    }
}