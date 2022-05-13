using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ManufacturingManagement_V2.Models
{
    public class AAA_ITAssestCell_MDI
    {
        public int Tran_Id { get; set; }

        [Required(ErrorMessage = " Required")]
        public string Item_Type { get; set; }

        [Required(ErrorMessage = "Scrap_Item")]
        public string Scrap_Item { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Item_Name { get; set; }

        [Required(ErrorMessage = "Required?")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Required")]
        public int compcode { get; set; }
       [Required(ErrorMessage = " Required")]
        public string cmpname { get; set; }
        public string ShortName { get; set; }

        [Required(ErrorMessage = " Required")]
        public int Issue_Qty { get; set; }

        public string DepCode { get; set; }
        [Required(ErrorMessage = " Required")]
        public string Department { get; set; }

        [Required(ErrorMessage = " Required")]
        public string Remark { get; set; }

        [Required(ErrorMessage = "Issue To Required")]
        public string Issue_To { get; set; }
        public int NewEmpId { get; set; }
        [Required(ErrorMessage = "EmpName Required")]
        public string EmpName { get; set; }

        public int Supplier_Id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Supplier_Name { get; set; }

        [Required(ErrorMessage = "Comment Required")]
        public string Comment { get; set; }
        public string Invoice_No { get; set; }
        public int Plus { get; set; }
        public int Minus { get; set; }
        public string Scrap_Comment { get; set; }
        public string Approoved { get; set; }
        public string Approval_Comment { get; set; }

        [Required(ErrorMessage = " Required")]
        public int Scrap_Qty { get; set; }

        public int Scrap1 { get; set; }
        public int Scrap2 { get; set; }
        public int Scrap3 { get; set; }
        public string Location { get; set; }
        public int StockMaster { get; set; }
        
        public AAA_ITAssestCell_MDI()
        {
            this.AAA_Item_Type = new List<SelectListItem>();
            this.AAA_ItemMaster_ITCell = new List<SelectListItem>();
            this.AAA_Item_StockITCell = new List<SelectListItem>();

        }
       public List<SelectListItem> AAA_Item_Type { get; set; }
       public List<SelectListItem> AAA_ItemMaster_ITCell { get; set; }
       public List<SelectListItem> AAA_Item_StockITCell { get; set; }
       public int Item_Type_Id { get; set; }
       public int Item_Id { get; set; }
       public string Serial_No { get; set; }

        public List<AAA_ITAssestCell_MDI> SubCatList{ get; set; }


        [Required(ErrorMessage = "Purchaage_Date")]
        [DataType(DataType.Date)]
  
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Purchaage_Date { get; set; }

       
        [Required(ErrorMessage = "Warranty_Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Warranty_Date { get; set; }



        public int Issue_Id { get; set; }
        [Required(ErrorMessage = "Issue_Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Issue_Date { get; set; }

        [Required(ErrorMessage = "Scrap_Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Scrap_Date { get; set; }
        public List<AAA_ITAssestCell_MDI> Item_List { get; set; }
           
        [DataType(DataType.Upload)]
        [Display(Name = "Select File")]
        public List<HttpPostedFileBase> files { get; set; }

        public int Id { get; set; }
        public int RecordId { get; set; }
        [Required(ErrorMessage = "FileName Required")]
        public String FileName { get; set; }

        [Required(ErrorMessage = "FileContent Required")]
        public byte[] FileContent { get; set; }

        [Required(ErrorMessage = "Order No Required")]
        public string OrderNo { get; set; }

        [Required(ErrorMessage = "Item Quantity Required")]
        public int ItemQty { get; set; }

        [Required(ErrorMessage = "Item Description Required")]
        public string ItemDescription { get; set; }

        public string BillNo { get; set; }
       
        public int Userid { get; set; }
        public bool Status { get; set; }


        

        }
    }
