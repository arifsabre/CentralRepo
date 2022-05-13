using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class EmailTrackMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime EmDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "From")]
        public string EmFrom { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string Railway { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Subject")]
        public string EmSubject { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "UserId")]
        public int UserId { get; set; }

        [Display(Name = "Assigned To")]
        public string UserName { get; set; }

        [Display(Name = "Status")]
        public string EmStatus { get; set; }

        [Display(Name = "Status")]
        public string EmStatusName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime RepDate { get; set; }

        public string Remarks { get; set; }
    }
}