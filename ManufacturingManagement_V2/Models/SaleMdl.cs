using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class SaleMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SaleRecId")]
        public int SaleRecId { get; set; }

        public string CompCode { get; set; }
        public string FinYear { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VType")]
        public string VType { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VNo")]
        public int VNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        [Display(Name = "VDate")]
        public string VDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Cash/Credit")]
        public string CashCredit { get; set; }

        public int AcCode { get; set; }
        public string Party { get; set; }//d

        public string ConsigneeName { get; set; }//d
        public string ChInv { get; set; }
        public string InvSeries { get; set; }
        public string Form38No { get; set; }
        public string FormCNo { get; set; }
        public double SubTotal { get; set; }
        public double BEDAdvPer { get; set; }
        public double BEDAdvAmount { get; set; }
        public double EduCessPer { get; set; }
        public double EduCessAmount { get; set; }
        public double SheCessPer { get; set; }
        public double SheCessAmount { get; set; }
        public double ExciseDutyAmount { get; set; }
        public double VATAmount { get; set; }
        public double SATAmount { get; set; }
        public double CSTAmount { get; set; }
        public double EntryTaxAmount { get; set; }
        public double FreightAmount { get; set; }
        public double OtherCharges { get; set; }
        public double Discount { get; set; }
        public double NetAmount { get; set; }
        public string GRNo { get; set; }

        [Display(Name = "GR Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string GRDate { get; set; }
        public string TrpDetail { get; set; }
        public int POrderId { get; set; }
        public int ConsigneeId { get; set; }
        public string RemovalDateTime { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "MA Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string MADate { get; set; }
        public string MANo { get; set; }
        public string TarrifHeading { get; set; }
        public string PONumber { get; set; }

        [Display(Name = "PO Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string PODate { get; set; }
        public double POValue { get; set; }
        public string CertOption { get; set; }
        public int PayingAuthId { get; set; }

        public string PayingAuthName { get; set; }//d
        public string B1LetterNo { get; set; }
        public string B1LetterDate { get; set; }
        public string B1Agent { get; set; }
        public string B2LetterNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "B2LetterDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public string B2LetterDate { get; set; }
        public string B2Agent { get; set; }
        public string IssueDateTime { get; set; }
        public string RemovalTimeInWords { get; set; }
        public int TrfCompCode { get; set; }
        public string IntPONumber { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "IntPODate")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string IntPODate { get; set; }
        public string Remarks { get; set; }
        public string BillPODesc { get; set; }
        public string ElctRefNo { get; set; }
        public string ElctRefDate { get; set; }
        public double RevChgTaxAmount { get; set; }
        public double TaxableAmount { get; set; }
        public double NetReceivable { get; set; }
        public string TaxType { get; set; }
        public string TrpMode { get; set; }
        public string PackingDetail { get; set; }
        public string InvoiceMode { get; set; }
        public string InvNote { get; set; }
        public string RNoteInfo { get; set; }
        public double AdjAmount { get; set; }
        public bool IsUnloaded { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Unloading Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string UnloadingDate { get; set; }
        public bool IsRCReceived { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RCRecDate")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string RCRecDate { get; set; }
        public bool IsRNoteReceived { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RNote Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string RNoteDate { get; set; }
        public bool IsBillSubmitted { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Bill Submit Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string BillSubmitDate { get; set; }
        public string BillSubmitDateP2 { get; set; }
        public string RCInfo { get; set; }
        public int OtherChgAcCode { get; set; }
        public double OtherChgPer { get; set; }

        public int RetSaleRecId { get; set; }
        public string RetInvoiceNo { get; set; }//d

        public string VersionNumber { get; set; }
        public string SupTypeCode { get; set; }
        public string DocTypeCode { get; set; }
        public string PrecDocNo { get; set; }
        public string PrecDocDate { get; set; }
        public int SupAddressId { get; set; }
        public string AddressName { get; set; }//d-bySupAddressId

        public double DistanceKM { get; set; }

        public string BillNo { get; set; }//d

        public string InvReferenceNo { get; set; }
        public string AckNo { get; set; }
        public string SekNo { get; set; }

        [Display(Name = "Acknowlwdgement Date")]
        public string AckDate { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string ItemQty { get; set; }

        public int RailwayId { get; set; }//d

        public bool ExportOpt1 { get; set; }
        public bool ExportOpt2 { get; set; }

        public string ActionMode { get; set; }

        public List<SaleLedgerMdl> Ledgers { get; set; }

        public List<SaleMdl> ObjectList { get; set; }

    }

}