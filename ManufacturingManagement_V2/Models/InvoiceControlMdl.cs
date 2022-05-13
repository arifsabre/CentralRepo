using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class InvoiceControlMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SaleRecId")]
        public int SaleRecId { get; set; }

        [Display(Name = "Invoice No")]
        public string BillNo { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Invoice Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime VDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Is Unloaded")]
        public bool IsUnloaded { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Unloading Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime UnloadingDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Is RC Received")]
        public bool IsRCReceived { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RC Rec Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime RCRecDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Is R Note Received")]
        public bool IsRNoteReceived { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "R Note Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime RNoteDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Is Bill Submitted P1")]
        public bool IsBillSubmitted { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Bill Submit Date P1")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime BillSubmitDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Is Bill Submitted P2")]
        public bool IsBillSubmittedP2 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Bill Submit Date P2")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime BillSubmitDateP2 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RC Info")]
        public string RCInfo { get; set; }

        [Display(Name = "Bill % P1")]
        public double BillPerP1 { get; set; }//d

        [Display(Name = "Bill % P2")]
        public double BillPerP2 { get; set; }//d

        [Display(Name = "GR No")]
        public string GrNo { get; set; }//d

        public string FlName { get; set; }//file upload
        public byte[] FileContent { get; set; }//file upload

    }
}