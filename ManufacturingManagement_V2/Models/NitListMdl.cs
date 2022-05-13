using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class NitListMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TenderNo")]
        public string TenderNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "EntryDT ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime EntryDT { get; set; }

        [Display(Name = "TenderTitle")]
        public string TenderTitle { get; set; }

        [Display(Name = "DeptRailway")]
        public string DeptRailway { get; set; }

        [Display(Name = "ItemDesc")]
        public string ItemDesc { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Task Name")]
        public string TaskName { get; set; }

        [System.Web.Mvc.AllowHtml]
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TenderStatus")]
        public string TenderStatus { get; set; }

        [Display(Name = "UploadingDT")]
        public string UploadingDT { get; set; }

        [Display(Name = "ClosingDT")]
        public string ClosingDT { get; set; }

        [Display(Name = "DueDays")]
        public string DueDays { get; set; }

        [Display(Name = "PrcToCS")]
        public string PrcToCS { get; set; }

        public bool isPrcToCS { get; set; }//note

        [Display(Name = "NotOurItem")]
        public string NotOurItem { get; set; }

        public bool isNotOurItem { get; set; }//note

        [Display(Name = "AlertGenerated")]
        public string AlertGenerated { get; set; }
        public bool isAlertGenerated { get; set; }//note

        [Display(Name = "ProcessedBy")]
        public string ProcessedBy { get; set; }
        
        [Display(Name = "AlertBy")]
        public string AlertBy { get; set; }

        [Display(Name = "ProcessDT")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime ProcessDT { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "CompCode")]
        public string CompCode { get; set; }

        [Display(Name = "ShortName")]
        public string ShortName { get; set; }//d

        [Display(Name = "ProcessedByName")]
        public string ProcessedByName { get; set; }//d

        [Display(Name = "DispUser")]
        public string DispUser { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [System.Web.Mvc.AllowHtml]
        [Display(Name = "PageContent")]
        public string PageContent { get; set; }

        public List<NitListMdl> TenderList { get; set; }

    }
}