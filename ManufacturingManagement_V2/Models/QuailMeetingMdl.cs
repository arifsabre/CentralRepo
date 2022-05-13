using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class QuailMeetingMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "QmId")]
        public int QmId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime QlbDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Time")]
        public string QlbTime { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Function")]
        public int FctnId { get; set; }

        [Display(Name = "Function")]
        public string FctnName { get; set; }//d

        [Display(Name = "Function Leader")]
        public string FnLeader { get; set; }//d

        [Display(Name = "Status")]
        public int QmStatus { get; set; }

    }

}