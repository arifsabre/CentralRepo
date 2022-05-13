using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ManufacturingManagement_V2.Models
{
    public class rptOptionMdl
    {
        //general
        public string FromDate { get; set; }

        public string ToDate { get; set; }
        public string BioId { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }

        [Display(Name = "From Date1")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime DateFrom1 { get; set; }

        [Display(Name = "To Date1")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime DateTo1 { get; set; }

        [Display(Name = "Report Format")]
        public string ReportFormat { get; set; }

        public string RptHeader { get; set; }//d

        public string RptName { get; set; }

        public int ObjectId { get; set; }

        [Display(Name = "Report Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime ReportDate { get; set; }
        
        //sorting
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }

        //employee report filters
        public int NewEmpId { get; set; }
        public int CategoryId { get; set; }
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string Grade { get; set; }
        public int JoiningUnit { get; set; }
        public int WorkingUnit { get; set; }
        public string HodEmpId { get; set; }
        public string DepCode { get; set; }
        public string ServiceType { get; set; }
        public string EmpType { get; set; }//as all,existing,resigned
        public bool LockStatus { get; set; }

        //attendance report filters
        public string AttDate { get; set; }
        public int AttDay { get; set; }
        public int AttMonth { get; set; }
        public int AttYear { get; set; }
        public string AttShift { get; set; }

        //compliance report
        public int VehicleId { get; set; }
        [Display(Name = "Company")]
        public int CompCode { get; set; }
        [Display(Name = "Fin. Year")]
        public string FinYear { get; set; }
        public string CmpName { get; set; }//d
        public int CTypeId { get; set; }
        public string CTypeName { get; set; }//d
        public string RegNo { get; set; }//d

        //form 14-15 report
        public string FormNo { get; set; }

        //indent report
        public int VNoFrom { get; set; }
        public int VNoTo { get; set; }
        public string Status { get; set; }

        //bonus report
        public bool Detailed { get; set; }

        //pending order
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }

        //lic report
        public bool Above58 { get; set; }
        public double Amount { get; set; }

        //inventory
        [Display(Name = "Item Type")]
        public string ItemType { get; set; }
        [Display(Name = "Item Group")]
        public int GroupId { get; set; }

        [Display(Name = "Item Group")]
        public string GroupName { get; set; }//d

        [Display(Name = "Finished Item")]
        public int FgItemId { get; set; }

        [Display(Name = "Finished Item")]
        public string FgItemCode { get; set; }//d

        [Display(Name = "Raw Material")]
        public int RmItemId { get; set; }

        [Display(Name = "Raw Material")]
        public string RmItemCode { get; set; }//d

        //Mktg-V2 report
        public string Option { get; set; }
        public string Days { get; set; }
        public int RailwayId { get; set; }
        public int AgentId { get; set; }
        public string POType { get; set; }
        public string OrderStatus { get; set; }
        public string RailwayName { get; set; }
        public string fromSugar { get; set; }
        public bool FilterByDT { get; set; }

        public string InvoiceNo { get; set; }
        public int SaleRecId { get; set; }

        //invoice control
        public string RptFor { get; set; }
        public string RptOpt { get; set; }

        public List<EmpColumns> EmpColumns { get; set; }
        public string[] SelectedEmpColumns { get; set; }

        public int machineid { get; set; }
        public int scheduleid { get; set; }

        public int scheduleid1 { get; set; }
        public string HsnCode { get; set; }

        public string LastUpdated { get; set; }
        public List<rptOptionMdl> Item_List { get; set; }

        public int AgencyId { get; set; }
        public int LocationId { get; set; }

        public int ECode { get; set; }

    }

    public class EmpColumns
    {
        public string EmpCol { get; set; }
        public string EmpColName { get; set; }

    }

}