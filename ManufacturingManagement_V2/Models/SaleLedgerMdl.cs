using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class SaleLedgerMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LRecId { get; set; }
        public int SaleRecId { get; set; }
        public int ItemRecId { get; set; }
        public string NoOfPckg { get; set; }
        public double Qty { get; set; }
        public double DelvQty { get; set; }
        public double Rate { get; set; }
        public double UnitRate { get; set; }
        public double Amount { get; set; }
        public double ExciseDutyPer { get; set; }
        public double ExciseDutyAmount { get; set; }
        public double VATPer { get; set; }
        public double VATAmount { get; set; }
        public double SATPer { get; set; }
        public double SATAmount { get; set; }
        public double CSTPer { get; set; }
        public double CSTAmount { get; set; }
        public double EntryTaxPer { get; set; }
        public double EntryTaxAmount { get; set; }
        public double FreightRate { get; set; }
        public double FreightAmount { get; set; }
        public double NetAmount { get; set; }
        public string Remarks { get; set; }
        public string InspCertificate { get; set; }
        public string DispatchMemo { get; set; }
        public bool ExciseDutyInc { get; set; }
        public bool SaleTaxInc { get; set; }
        public int LgrItemId { get; set; }
        public string ItemDesc { get; set; }
        public string ItemCode { get; set; }
        public double Discount { get; set; }
        public double TaxableAmount { get; set; }
        public bool IsInclGst { get; set; }
        public string UnitName { get; set; }
        public string HSNCode { get; set; }//d
        public int Unit { get; set; }//d

        //additional for porder detail

        public int ConsigneeId { get; set; }//d
        public string ConsigneeName { get; set; }//d
        public double OrdQty { get; set; }//d
        public double DspQty { get; set; }//d
        public double RemQty { get; set; }//d
        public double HsQty { get; set; }//d
        public string DelvDateStr { get; set; }//d
        public int ItemSlNo { get; set; }//d

    }
}