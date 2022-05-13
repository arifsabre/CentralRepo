using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class PaymentReceiptMdl
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Display(Name = "PayingAuthId")]
        public int PayingAuthId { get; set; }

        public List<BillOsMdl> BillOS { get; set; }

        public List<VoucherMdl> Voucher { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string DetailInfo { get; set; }

        //Payment Receipt ERP_V1

        [Required(ErrorMessage = "Required!")]
        public int SaleRecId { get; set; }

        public string BillNo { get; set; }//d
        public string PONumber { get; set; }//d
        public string PayingAuthName { get; set; }//d
        public double BillAmount { get; set; }//d

        [Display(Name = "Bill P1%")]
        public double BillPerP1 { get; set; }

        [Display(Name = "Bill Amount P1")]
        public double BillAmountP1 { get; set; }

        [Display(Name = "Bill Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime BillDate { get; set; }//d=vdate

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Neft Detail P1")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime NeftDetailP1 { get; set; }

        [Display(Name = "Receipt Amount P1")]
        public double RecAmountP1 { get; set; }

        [Display(Name = "Excess Amount P1")]
        public double ExcessAmountP1 { get; set; }

        [Display(Name = "Short Amount P1")]
        public double ShortAmountP1 { get; set; }

        [Display(Name = "Short Amount Reason P1")]
        public string ShortAmtReasonP1 { get; set; }

        [Display(Name = "Recoverable Amount P1")]
        public double RecoverableAmtP1 { get; set; }

        [Display(Name = "R Note Qty")]
        public double RNoteQty { get; set; }

        [Display(Name = "Bill P2%")]
        public double BillPerP2 { get; set; }

        [Display(Name = "Bill Amount P2")]
        public double BillAmountP2 { get; set; }

        [Display(Name = "Neft Detail P2")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime NeftDetailP2 { get; set; }

        [Display(Name = "Receipt Amount P2")]
        public double RecAmountP2 { get; set; }

        [Display(Name = "Excess Amount P2")]
        public double ExcessAmountP2 { get; set; }

        [Display(Name = "Short Amount P2")]
        public double ShortAmountP2 { get; set; }

        [Display(Name = "Short Amount Reason P2")]
        public string ShortAmtReasonP2 { get; set; }

        [Display(Name = "Recoverable Amount P2")]
        public double RecoverableAmtP2 { get; set; }

        [Display(Name = "LD Charge")]
        public double LDCharge { get; set; }

        [Display(Name = "ED Deduction")]
        public double EDDeduction { get; set; }

        [Display(Name = "VAT Difference")]
        public double VatDiff { get; set; }

        [Display(Name = "Rate Difference")]
        public double RateDiff { get; set; }

        [Display(Name = "Rejection")]
        public double Rejection { get; set; }

        [Display(Name = "Warranty Claim")]
        public double WtClaim { get; set; }

        [Display(Name = "Calculation Mistake")]
        public double CalMist { get; set; }

        [Display(Name = "Bank Charge")]
        public double BankCharge { get; set; }

        [Display(Name = "Miscellaneous")]
        public double Miscellaneous { get; set; }

        [Display(Name = "VNO P1")]
        public string VNoP1 { get; set; }

        [Display(Name = "VNO P2")]
        public string VNoP2 { get; set; }

        [Display(Name = "Security Amount")]
        public double SecurityAmount { get; set; }

        [Display(Name = "SD Received Amount")]
        public double SDReceivedAmount { get; set; }

        [Display(Name = "SD Remarks")]
        public string SDRemarks { get; set; }

        [Display(Name = "TDS Deduction SGST")]
        public double TdsDedSgst { get; set; }

        [Display(Name = "TDS Deduction IGST")]
        public double TdsDedIgst { get; set; }

        [Display(Name = "TCS Deduction")]
        public double TcsDeduction { get; set; }
        //derived from sale

        public string Remarks { get; set; }//d 

        [Display(Name = "R Note Info")]
        public string RNoteInfo { get; set; }//d

        [Display(Name = "Bill 1 Letter Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime B1LetterDate { get; set; }//d

        [Display(Name = "Bill 2 Letter Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime B2LetterDate { get; set; }//d

    }

}

