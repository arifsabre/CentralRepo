using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class WorkListMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Task Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime RecDT { get; set; }

        [Display(Name = "Compliance Of")]
        public int DepId { get; set; }

        [Display(Name = "Compliance Of")]
        public string DepName { get; set; }//d

        //[Required(ErrorMessage = "Required!")]
        [Display(Name = "UserId")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Task Name")]
        public string TaskName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Description")]
        public string WorkDesc { get; set; }

        [Display(Name = "Status")]
        public bool IsCompleted { get; set; }

        [Display(Name = "CompDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime CompDT { get; set; }

        [Display(Name = "CompDate")]
        public string CompDTStr { get; set; }//d

        [Display(Name = "compcode")]
        public int CompCode { get; set; }

        [Display(Name = "Company")]
        public string CompName { get; set; }//d

        [Display(Name = "Company")]
        public string ShortName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Department")]
        public int TaskOpt { get; set; }

        [Display(Name = "Department")]
        public string TaskOptName { get; set; }//d

        [Display(Name = "Document")]
        public bool Doc { get; set; }

    }
}