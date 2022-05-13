using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ETenderItemMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ItemRecId { get; set; }
        public int TenderId { get; set; }
        public int TDRecId { get; set; }
        public string SpcChkList { get; set; }
        public string SpcRemarks { get; set; }
        public string ItemDescOpt { get; set; }
        public string ItemDescOffered { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }//d
        public string ShortName { get; set; }//d
        public string ItemName { get; set; }
        public double FullTDQty { get; set; }
        public string UnitOfMeasurement { get; set; }
        public double BasicRatePerUnit { get; set; }
        public double UncDiscountPer { get; set; }
        public double BasicPkgPer { get; set; }
        public string EDType { get; set; }
        public double EDMaxPerApplicable { get; set; }
        public double CessOnEDPer { get; set; }
        public string OtherChgType { get; set; }
        public double OtherChgPerUnit { get; set; }
        public bool PriceVarReq { get; set; }
        public string PriceVarClause { get; set; }
        public string Remarks { get; set; }

        public string TenderInfo { get; set; }//d

        [System.Web.Mvc.AllowHtml]
        public string TendetDetailItemsHtml { get; set; }//d

        [System.Web.Mvc.AllowHtml]
        public string ETenderItemsHtml { get; set; }//d

        public List<TenderDetailMdl> TendetDetailItems { get; set; }//d

        public List<ETenderItemMdl> ETenderItems { get; set; }//d

    }

}