using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class C_ComplaintMDI
    {
        public int srno { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Reference_No")]
        public int referenceno { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Prag SRN")]
        public string pragsrno { get; set; }
        public string pragno { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company")]
        public int compcode { get; set; }

        [Display(Name = "Company")]
        public string cmpname { get; set; }
        public string shortname { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Item Name")]
        public int itemid { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Item Name")]
        public string itemname { get; set; }

        
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Assign_To")]
        public int newempid { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Assign_To")]
        public string EmpName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Customer Name")]
        public int customerid { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Customer Name")]
        public string customername { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Description")]
        public string detail { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Recieved By")]
        public string recievedby { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Priority")]
        public string priority { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Status")]
        public string status { get; set; }


        [Display(Name = "Complaint Recieved Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime dateofcomplaintrecieved { get; set; }
        public string dateofcomplaintrecieved1 { get; set; }

        [Display(Name = "Attend Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime attendeddate { get; set; }
        public string attendeddate1 { get; set; }

        [Display(Name = "Closed Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime dateofclosed { get; set; }
        public string dateofclosed1 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Orservation")]
        public string observation { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Rake No As per Ordering Agency")]
        public string rakenoorderagency { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Root Cause")]
        public string @rootcause { get; set; }

         [Required(ErrorMessage = "Required!")]
        [Display(Name = "Action Taken")]
        public string actiontaken { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Item Replaced")]
        public string detailofitemreplaced { get; set; }

        [Display(Name = "Complainer Mobile No")]
        [Required(ErrorMessage = "Please Enter Valid Mobile Number")]
        // [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Please Enter Valid Mobile Number!")]
        public string mobile1 { get; set; }

        [Display(Name = "Engineer Mobile No")]
        [Required(ErrorMessage = "Please Enter Valid Mobile Number!")]
        // [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Please Enter Valid Mobile Number!")]
        public string mobile2 { get; set; }
    


        [Display(Name = "Created On")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
         ApplyFormatInEditMode = true)]
        public DateTime createdon { get; set; }
        public string createdon1 { get; set; }

        [Display(Name = "Updated On")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
         ApplyFormatInEditMode = true)]
        public DateTime updatedon { get; set; }
        public string updatedon1 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "userid")]
        public int userid { get; set; }
        public int userid1 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CreatedBy")]
        public string createdby { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "UpdatedBy")]
        public string updatedby { get; set; }


        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Shed Id")]
        public int ShedId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Shed Name")]
        public string ShedName { get; set; }

        [Display(Name = "Trainning from")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Trainning To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Tranner Name")]
        public string Tranner { get; set; }

        public List<C_ComplaintMDI> Item_List { get; set; }
    }
}