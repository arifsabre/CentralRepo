using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class TenderDetailMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }
        public int TenderId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }//d
        public string ShortName { get; set; }//d
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public double Rate { get; set; }
        public string L1L2 { get; set; }
        public double OurQty { get; set; }
        public int Unit { get; set; }//d
        public string UnitName { get; set; }
    }
}