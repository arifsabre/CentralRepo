using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ModifyAdviceMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int POrderId { get; set; }
        public int TenderId { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "MA SlNo")]
        public int RecSlNo { get; set; }
        public int editRecSlNo { get; set; }//note

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "MA Number")]
        public string ModifyAdvNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "MA For")]
        public string MAFor { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string MADetailHtml { get; set; }//d
        public int RailwayId { get; set; }//d
        public string POInfo { get; set; }//d

    }
}