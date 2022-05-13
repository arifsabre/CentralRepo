using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AAA_HolidayMDI
    {
       
        public int HolidayId { get; set; }

        [Display(Name = "Holiday Name:")]
        [Required(ErrorMessage = "Holiday Name is Required:")]
        public string HolidayName { get; set; }

        [Display(Name = "Holiday Date:")]
        [Required(ErrorMessage = "Date Is Required:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime HolidayDate { get; set; }


        [Display(Name = "Description:")]
        [Required(ErrorMessage = "Description Is Required:")]
        public string Description { get; set; }
       

        [Display(Name = "Adjustment Date:")]
        [Required(ErrorMessage = "Adjustment Is Required:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime AdjustmentDate { get; set; }
                
        public string Day_of_Week { get; set; }
        public List<AAA_HolidayMDI> HolidayList { get; set; }
       }
    

}