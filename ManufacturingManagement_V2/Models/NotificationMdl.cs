using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class NotificationMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "NoticeId")]
        public int NoticeId { get; set; }

        [Display(Name = "NoticeNo")]
        public int NoticeNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Notice Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime NoticeDT { get; set; }

        [Display(Name = "Sending User")]
        public int SendingUser { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Message")]
        public string NoticeMsg { get; set; }

        [Display(Name = "Message For")]
        public string MsgType { get; set; }

        [Display(Name = "To Receipient")]
        public int ToUser { get; set; }

        [Display(Name = "Message For")]
        public string MsgTypeName { get; set; }//d

        [Display(Name = "Message From")]
        public string SendingUserName { get; set; }//d

        [Display(Name = "To Receipient")]
        public string ToUserName { get; set; }//d

        public List<NotificationUserMdl> NotificationUsers { get; set; }

    }

    public class NotificationUserMdl
    {
        public string RecId { get; set; }

        [Display(Name = "Receiving User")]
        public string ReceivingUser { get; set; }

        [Display(Name = "Message From")]
        public string SendingUserName { get; set; }//d

        [Display(Name = "Receiving User")]
        public string ReceivingUsername { get; set; }//d

        [Display(Name = "NoticeNo")]
        public int NoticeNo { get; set; }//d

        [Display(Name = "Notice Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime NoticeDT { get; set; }

        [Display(Name = "Notification")]
        public string NoticeMsg { get; set; }

        [Display(Name = "Status")]
        public string IsAttended { get; set; }

        [Display(Name = "Attended AT")]
        public string AttendedAT { get; set; }

        [Display(Name = "Reply Msg")]
        public string ReplyMsg { get; set; }

        public bool setUser { get; set; }//1-set,0-reset
    }

}