using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class PowerConsuptionMDI
    {
        public int Id { get; set; }

        [Display(Name = "Select Company")]
        [Required(ErrorMessage = "Company Name is Required:")]
        public int? compcode { get; set; }
        public string cmpname { get; set; }
        public string ShortName { get; set; }

        [Display(Name = "Select Reading Date")]
        [Required(ErrorMessage = "Reading is Required:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public string ReadingDate { get; set; }

        public string ReadingDateDisp { get; set; }

        [Display(Name = "Enter Reading Time")]
        [Required(ErrorMessage = "Reading Time is Required:")]
       // [DataType(DataType.Time)]
       // [DisplayFormat(DataFormatString = "{hh:mm:ss tt")]
        public string ReadingTime { get; set; }

        [Display(Name = "MeterReading in Kwh")]
        [Required(ErrorMessage = "MeterReadingKwh is Required:")]
        [DisplayFormat(DataFormatString = "{0:0.####}", ApplyFormatInEditMode = true)]
        public decimal MeterReadingKwh { get; set; }

        [Display(Name = "MeterReadingKvah in Kvah")]
        [Required(ErrorMessage = "MeterReadingKvah is Required:")]
        [DisplayFormat(DataFormatString = "{0:0.####}", ApplyFormatInEditMode = true)]
        public decimal MeterReadingKvah { get; set; }

        [Display(Name = "Daily Consuption in  Kwh ")]
        [Required(ErrorMessage = "Daily Consuption is Required:")]
        [DisplayFormat(DataFormatString = "{0:0.####}", ApplyFormatInEditMode = true)]
        public decimal DailyConKwh { get; set; }

        [Display(Name = "Daily Consuption in  Kvah ")]
        [Required(ErrorMessage = "Daily Consuption is Required:")]
        [DisplayFormat(DataFormatString = "{0:0.####}", ApplyFormatInEditMode = true)]
        public decimal DailyConKvah { get; set; }

        [Display(Name = "MTD Consuption in  Kwh ")]
        [Required(ErrorMessage = "MTD Consuption is Required:")]
        [DisplayFormat(DataFormatString = "{0:0.####}", ApplyFormatInEditMode = true)]
        public decimal MTDConKwh { get; set; }

        [Display(Name = "MTD Consuption in  Kvah ")]
        [Required(ErrorMessage = "MTD Consuption is Required:")]
        [DisplayFormat(DataFormatString = "{0:0.####}", ApplyFormatInEditMode = true)]
        public decimal MTDConKvah { get; set; }

        [Display(Name = "PFDaily")]
        [Required(ErrorMessage = "PFDaily is Required:")]
        [DisplayFormat(DataFormatString = "{0:0.####}", ApplyFormatInEditMode = true)]
        public decimal PFDaily { get; set; }

        [Display(Name = "MTDPFDaily")]
        [Required(ErrorMessage = "MTDPFDaily is Required:")]
        [DisplayFormat(DataFormatString = "{0:0.####}", ApplyFormatInEditMode = true)]
        public decimal MTDPFDaily { get; set; }

        [Display(Name = "MeterPFDaily")]
        [Required(ErrorMessage = "MeterPFDaily is Required:")]
       [DisplayFormat(DataFormatString = "{0:0.###}", ApplyFormatInEditMode = true)]
        public decimal MeterPFDaily { get; set; }

        [Display(Name = "Remark")]
        [Required(ErrorMessage = "Remark is Required:")]
        public string Remark { get; set; }

       
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }

        public int? UserId { get; set; }
        public string username { get; set; }
        public DateTime? DeletedOn { get; set; }

        public string UpdatedBy { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:0.###}", ApplyFormatInEditMode = true)]
        public decimal Demand { get; set; }

        public List<PowerConsuptionMDI> Item_List { get; set; }
























    }
}