using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ProductionPlanDPDetailMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Display(Name = "PO Number")]
        public string PONumber { get; set; }

        [Display(Name = "PO Date")]
        public string PODate { get; set; }

        [Display(Name = "CaseFileNo")]
        public string CaseFileNo { get; set; }

        [Display(Name = "PendingQty")]
        public double PendingQty { get; set; }

        [Display(Name = "DelvDate")]
        public string DelvDate { get; set; }

        [Display(Name = "Railway")]
        public string RailwayName { get; set; }

        [Display(Name = "Consignee")]
        public string ConsigneeName { get; set; }

    }
}