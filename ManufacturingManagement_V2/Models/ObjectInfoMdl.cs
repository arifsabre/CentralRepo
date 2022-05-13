using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ObjectInfoMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Object Id")]
        public int ObjectId { get; set; }

        [Display(Name = "Group Id")]
        public int EntryId { get; set; }

        [Display(Name = "Group Name")]
        public string EntryName { get; set; }//d

        [Display(Name = "Report SlNo")]
        public int SlNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Report Name")]
        public string RptName { get; set; }

        [Display(Name = "Pattern As")]
        public string PatternAs { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SP Name")]
        public string SPName { get; set; }

        [Display(Name = "Report Document Name")]
        public string RptDocName { get; set; }

        [Display(Name = "BLL Method")]
        public string BllMethod { get; set; }

        [Display(Name = "Location Menu")]
        public string LocationMenu { get; set; }

        [Display(Name = "Report Link")]
        [System.Web.Mvc.AllowHtml]
        public string RptLink { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        public List<ObjectInfoMdl> ObjectInfoList { get; set; }

    }
}