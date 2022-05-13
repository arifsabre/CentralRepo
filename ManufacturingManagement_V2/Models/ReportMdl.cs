using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ReportMdl
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        //public int SaleRecId { get; set; }

        public string ReportHeader { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string ReportContent { get; set; }

        public string RunDate { get; set; }

        public string ReportUser { get; set; }

    }

}