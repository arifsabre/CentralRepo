using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class QuailCalendarMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Month")]
        public int Mth { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Year")]
        public int Yr { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Week")]
        public int WKN { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Monday")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime Monday { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Tuesday")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime Tuesday { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Wednesday")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime Wednesday { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Thursday")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime Thursday { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Friday")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime Friday { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Saturday")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime Saturday { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Sunday")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime Sunday { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "MondayInfo")]
        [System.Web.Mvc.AllowHtml]
        public string MondayInfo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TuesdayInfo")]
        [System.Web.Mvc.AllowHtml]
        public string TuesdayInfo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "WednesdayInfo")]
        [System.Web.Mvc.AllowHtml]
        public string WednesdayInfo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ThursdayInfo")]
        [System.Web.Mvc.AllowHtml]
        public string ThursdayInfo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "FridayInfo")]
        [System.Web.Mvc.AllowHtml]
        public string FridayInfo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SaturdayInfo")]
        [System.Web.Mvc.AllowHtml]
        public string SaturdayInfo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SundayInfo")]
        [System.Web.Mvc.AllowHtml]
        public string SundayInfo { get; set; }

        [Display(Name = "MondayQmId")]
        public int MondayQmId { get; set; }

        [Display(Name = "TuesdayQmId")]
        public int TuesdayQmId { get; set; }

        [Display(Name = "WednesdayQmId")]
        public int WednesdayQmId { get; set; }

        [Display(Name = "ThursdayQmId")]
        public int ThursdayQmId { get; set; }

        [Display(Name = "FridayQmId")]
        public int FridayQmId { get; set; }

        [Display(Name = "SaturdayQmId")]
        public int SaturdayQmId { get; set; }

        [Display(Name = "SundayQmId")]
        public int SundayQmId { get; set; }

    }

}