using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AmcDetailMdl
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AmcId { get; set; }
        public int ItemId { get; set; }
        public string ItemDesc { get; set; }

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

        [Display(Name = "Unit Rate")]
        public double UnitRate { get; set; }

        [Display(Name = "SGST %")]
        public double VatPer { get; set; }

        [Display(Name = "SGST Amount")]
        public double VatAmount { get; set; }

        [Display(Name = "CGST %")]
        public double SatPer { get; set; }

        [Display(Name = "CGST Amount")]
        public double SatAmount { get; set; }

        [Display(Name = "IGST %")]
        public double CstPer { get; set; }

        [Display(Name = "IGST Amount")]
        public double CstAmount { get; set; }

        [Display(Name = "Net Amount")]
        public double NetAmount { get; set; }

        public string Remarks { get; set; }

        public string HsnCode { get; set; }
        public string ItemFor { get; set; }
        public string ItemForName { get; set; }//d

    }
}