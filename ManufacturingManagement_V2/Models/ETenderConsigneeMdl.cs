using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ETenderConsigneeMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }
        public int ItemRecId { get; set; }
        public int TenderId { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        public int ConsigneeId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string ConsigneeName { get; set; }//d
        public double TenderQty { get; set; }
        public double OfferedQty { get; set; }
        public string SaleTaxType { get; set; }
        public double SaleTaxPer { get; set; }
        public double Freight { get; set; }
        public double BasicRate { get; set; }
        public double UnitRate { get; set; }
        public double TotalAmount { get; set; }
        public double LoaQty { get; set; }
        public double LoaRate { get; set; }
        public double LoaAmt { get; set; }

        public int RailwayId { get; set; }//d
        public string TenderInfo { get; set; }//d

        [System.Web.Mvc.AllowHtml]
        public string ETendetItemsHtml { get; set; }//d

        [System.Web.Mvc.AllowHtml]
        public string ETenderConsigneeHtml { get; set; }//d

        public List<ETenderItemMdl> ETenderItems { get; set; }//d

        public List<ETenderConsigneeMdl> ETenderConsigneeItems { get; set; }//d

    }

}