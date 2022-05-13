using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class RfqTenderDetailMdl
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RfqId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }

        [Display(Name = "Quantity")]
        public double Qty { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }

        [Display(Name = "Unit Name")]
        public int Unit { get; set; }//d

        [Display(Name = "Unit Name")]
        public string UnitName { get; set; }

        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }//d

        [Display(Name = "Short Name")]
        public string ShortName { get; set; }//d

        [Display(Name = "Group Name")]
        public string GroupName { get; set; }//d

    }
}