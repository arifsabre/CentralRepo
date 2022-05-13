using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AlertsMdl
    {
        [System.Web.Mvc.AllowHtml]
        [Display(Name = "AlertDetail")]
        public string AlertDetail { get; set; }

        public bool isBirthdayAlert { get; set; }
        public string BirthdayAnnivMsg { get; set; }

        public List<AlertListInfoMdl> AlertList { get; set; }

    }

    public class AlertListInfoMdl
    {
        public string alertfor { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string result { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string srcurl { get; set; }
    }
}