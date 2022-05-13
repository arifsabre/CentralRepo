using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class QuailBookMdl
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Display(Name = "QmId")]
        public int QmId { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime QlbDate { get; set; }//d

        [Display(Name = "QlbTime")]
        public string QlbTime { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Function")]
        public int FctnId { get; set; }//d

        [Display(Name = "Function")]
        public string FctnName { get; set; }//d

        [Display(Name = "PrvQmId")]
        public int PrvQmId { get; set; }//d

        [Display(Name = "Prv.Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime PrvDate { get; set; }//d

        [Display(Name = "Function Leader")]
        public string FnLeader { get; set; }//d

        [Display(Name = "Status")]
        public int QmStatus { get; set; }//d

        public bool AddNew { get; set; }

        [Display(Name = "NextQmId")]
        public int NextQmId { get; set; }//d

        [Display(Name = "NextDateTime")]
        public string NextDateTime { get; set; }//d

        public List<QuailBookItemMdl> Items { get; set; }

        public List<QuailAttendanceMdl> Attendance { get; set; }

    }

    public class QuailBookItemMdl
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SlNo")]
        public string DispSNo { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "MainSNo")]
        public int MainSNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SubSNo")]
        public int SubSNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Item")]
        public string QlbItem { get; set; }

        [Display(Name = "ItemType")]
        public string ItemType { get; set; }

        [Display(Name = "Priority")]
        public int QlbPriority { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "UserName")]
        public int QlbUserId { get; set; }

        [Display(Name = "UserName")]
        public string QlbUserName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CompTime")]
        public string CompTime { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

    }

    public class QuailAttendanceMdl
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "UserId")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "MemberName")]
        public string UserName { get; set; }

    }

}