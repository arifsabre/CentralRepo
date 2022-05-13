using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class OrderLedgerMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }
        public int OrderId { get; set; }
        public int SlNo { get; set; }
        public int ItemId { get; set; }
        public string ItemDesc { get; set; }

        [Display(Name = "Quantity")]
        public double OrdQty { get; set; }

        [Display(Name = "DspQty")]
        public double DspQty { get; set; }

        [Display(Name = "UnitId")]
        public int UnitId { get; set; }

        [Display(Name = "Unit Name")]
        public string UnitName { get; set; }//d

        public double Rate { get; set; }
        public double Amount { get; set; }
        public string Remarks { get; set; }

        [Display(Name = "Item Name")]
        public string ItemName { get; set; }//d

        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }//d

        [Display(Name = "IndentId")]
        public int IndentId { get; set; }//d

        [Display(Name = "IndentLgrId")]
        public int IndentLgrId { get; set; }//d

    }
}