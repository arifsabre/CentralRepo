using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class SetDeductionAmountMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Display(Name = "As on date")]
        public string VDate { get; set; }

        public List<DeductionListMdl> DeductionList { get; set; }

    }

    public class DeductionListMdl
    {
        public string EmpId { get; set; }//d
        public string NewEmpId { get; set; }
        public string EmpName { get; set; }
        public double Balance { get; set; }
        public double InstAmount { get; set; }
        public double DeductionAmt { get; set; }
        public string Remarks { get; set; }
    }

}