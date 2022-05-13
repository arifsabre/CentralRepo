using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ComplaintMdl
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CompId")]
        public int CompId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Comp. Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime CompDT { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CompUser")]
        public int CompUser { get; set; }

        [Display(Name = "CompUserName")]
        public string CompUserName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Complaint")]
        public string CompMsg { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CompType")]
        public int CompType { get; set; }

        [Display(Name = "Complaint Type")]
        public string CompTypeName { get; set; }//d

        //detail
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Reply Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime ReplyDT { get; set; }

        [Display(Name = "ReplyDTStr")]
        public string ReplyDTStr { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ReplyUser")]
        public int ReplyUser { get; set; }

        [Display(Name = "ReplyUserName")]
        public string ReplyUserName { get; set; }//d

        [Display(Name = "ForwardedTo")]
        public int ForwardedTo { get; set; }

        [Display(Name = "ForwardedUserName")]
        public string ForwardedUserName { get; set; }//d

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CompStatus")]
        public string CompStatus { get; set; }

        [Display(Name = "Complaint Status")]
        public string CompStatusName { get; set; }//d

    }

}