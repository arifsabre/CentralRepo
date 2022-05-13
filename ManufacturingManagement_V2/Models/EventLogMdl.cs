using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class EventLogMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }

        public int TblId { get; set; }

        public string PKVal { get; set; }

        [Display(Name = "Date")]
        public string EVDate { get; set; }

        [Display(Name = "Time")]
        public string EVTime { get; set; }

        [Display(Name = "Log Desc")]
        public string LogDesc { get; set; }

        [Display(Name = "User Name")]
        public string FullName { get; set; }

        [Display(Name = "TblName")]
        public string TblName { get; set; }

        [Display(Name = "PKField")]
        public string PKField { get; set; }
    }
}