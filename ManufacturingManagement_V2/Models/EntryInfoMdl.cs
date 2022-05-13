using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class EntryInfoMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TblId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "File Name")]
        public string TblName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PK Field")]
        public string PKField { get; set; }
    }
}