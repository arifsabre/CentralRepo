using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class VoucherMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company")]
        public int CCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "FinYear")]
        public string FinYR { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VType")]
        public string VType { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Voucher No")]
        public string VNo { get; set; }

        [Display(Name = "PKValStr")]
        public string PKValStr { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Voucher Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string VDate { get; set; }

        public List<VoucherInfoMdl> Info { get; set; }

        //---------------------------------------
        //for payment(to vendors) & receipt(from pauthority)
        //if billAcCode=0 then no billos entry

        [Display(Name = "BillAcCode")]
        public int BillAcCode { get; set; }

        [Display(Name = "BillAcDesc")]//disp
        public string BillAcDesc { get; set; }

        [Display(Name = "VendorId")]
        public int VendorId { get; set; }

        [Display(Name = "VendorId")]
        public int PayingAuthId { get; set; }
        //---------------------------------------

        public List<BillOsMdl> BillOsInfo { get; set; }

        public bool editMode { get; set; }
        public bool setBillInfo { get; set; } 

    }

    public class VoucherInfoMdl
    {
        [Display(Name = "VoucherId")]
        public string VoucherId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Voucher No")]
        public string VNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AcCode")]
        public int AcCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "AcContra")]
        public int AcContra { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DrAmount")]
        public double DrAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CrAmount")]
        public double CrAmount { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DrCr")]
        public string DrCr { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DrCr")]
        public string DrCrName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Narration")]
        public string Narration { get; set; }

        [Display(Name = "AcDesc")]
        public string AcDesc { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "BDate")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string BDate { get; set; }

        [Display(Name = "Amount")]//d
        public double Amount { get; set; }

        [Display(Name = "dispBillNo")]//d
        public string dispBillNo { get; set; }

    }
}