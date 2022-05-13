using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class C_Fitting_ItemMDI
    {

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SRN")]
        public int srno { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Prag SRN")]
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
        [Display(Name = "Consignee")]
        public int consigneeId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Consignee")]
        public string consigneename { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Invoice No")]
        public string invoiceno { get; set; }

        [Display(Name = "Invoice Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime invoicedate { get; set; }
        public string invoicedate1 { get; set; }

        [Display(Name = "Sold Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime solddate { get; set; }
        public string solddate1 { get; set; }

        //[Required(ErrorMessage = "Required!")]
        [Display(Name = "Coach No")]
        public  string coachno { get; set; }

        //[Required(ErrorMessage = "Required!")]
        [Display(Name = "Rake No As per Ordering Agency")]
        public string rakenoorderagency { get; set; }

       // [Required(ErrorMessage = "Required!")]
        [Display(Name = "Rake No As Per Railway")]
        public string rakenorailway { get; set; }

       // [Required(ErrorMessage = "Required!")]
        [Display(Name = "Rake No As Per User Railway")]
        public string rakenouserrailway { get; set; }

        //[Required(ErrorMessage = "Required!")]
        [Display(Name = "Nominated Shed of IR")]
        public string shedname { get; set; }

      
        [Display(Name = "Waranty UpTo")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime validity { get; set; }
        public string validity1 { get; set; }

        [Display(Name = "Installation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
         ApplyFormatInEditMode = true)]
        public DateTime fittingdate { get; set; }
        public string fittingdate1 { get; set; }



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

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "CreatedBy")]
        public string createdby { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "UpdatedBy")]
        public string updatedby { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "SaleId")]
        public int SaleRecId { get; set; }


       

        public List<C_Fitting_ItemMDI> Item_List { get; set; }

    }
}