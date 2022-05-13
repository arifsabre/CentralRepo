using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class FeedbackMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime EntryDT { get; set; }

        [Display(Name = "Date")]
        public string EntryDTStr { get; set; }//d

        [Display(Name = "Sending User")]
        public int SendingUser { get; set; }

        [Display(Name = "Sending User")]
        public string SendingUserName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Suggestion")]
        public string Suggestion { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Status")]
        public string FBStatus { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Status")]
        public string FBStatusName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Reply Message")]
        public string ReplyMsg { get; set; }

        [Display(Name = "Reply User")]
        public int ReplyUser { get; set; }

        [Display(Name = "Reply User")]
        public string ReplyUserName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Reply Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime ReplyDT { get; set; }

        [Display(Name = "Reply Date")]
        public string ReplyDTStr { get; set; }//d

    }

}