using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ManufacturingManagement_V2.Models
{
    //
    public class clsEnum{}
    //
    public enum LoginType
    {
        Admin = 0,
        User = 1,
        HOD = 2,
        Director = 3
    }
    //
    public enum Entry
    {
        //-------------------------------------------
        /*--Note: clsmyClass/getMainMenueList()
         
        updated by tbl_entrygroup and tbl_entrydetail as:-
        insert into tbl_entrydetail(groupid, entryid, entryname) values('','','')
        insert into tbl_entrygroup(groupid, groupname) values('','')
        
        --collection:
        select char(13)+'///'+char(13)+'/// '+
        cast(EntryId as varchar(10)) + char(13)+'///'+char(13)+ 
        entryname+' = '+cast(EntryId as varchar(10))+','
        from tbl_entrydetail where <GroupId=92>

        */
        //-------------------------------------------
        //mm+sm pattern
        //[Report Group by object Info = 92]

        //Report Group = 92
        /// <summary>
        /// 9201
        /// </summary>
        Account_Report = 9201,

        /// <summary>
        /// 9202
        /// </summary>
        Admin_Report = 9202,

        /// <summary>
        /// 9203
        /// </summary>
        Agent_Report = 9203,

        /// <summary>
        /// 9204
        /// </summary>
        Attendance_Report = 9204,

        /// <summary>
        /// 9205
        /// </summary>
        Bank_Guarantee_Report = 9205,

        /// <summary>
        /// 9206
        /// </summary>
        Billing_Report = 9206,

        /// <summary>
        /// 9207
        /// </summary>
        Biometric_Report = 9207,

        /// <summary>
        /// 9208
        /// </summary>
        Bonus_Report = 9208,

        /// <summary>
        /// 9209
        /// </summary>
        Calibration_Report = 9209,

        /// <summary>
        /// 9210 Not in Use
        /// </summary>
        Compliance_Report = 9210,

        /// <summary>
        /// 9211
        /// </summary>
        Dispatch_Report = 9211,

        /// <summary>
        /// 9212
        /// </summary>
        Employee_Advance_Report = 9212,

        /// <summary>
        /// 9213
        /// </summary>
        Employee_Alert_Report = 9213,

        /// <summary>
        /// 9214
        /// </summary>
        Employee_Report = 9214,

        /// <summary>
        /// 9215
        /// </summary>
        GST_Report = 9215,

        /// <summary>
        /// 9216
        /// </summary>
        Incentive_Report = 9216,

        /// <summary>
        /// 9217
        /// </summary>
        Inventory_Report = 9217,

        /// <summary>
        /// 9218
        /// </summary>
        Jobwork_Report = 9218,

        /// <summary>
        /// 9219
        /// </summary>
        LIC_Report = 9219,

        /// <summary>
        /// 9220
        /// </summary>
        Marketing_PO_Report = 9220,

        /// <summary>
        /// 9221 Not in Use
        /// </summary>
        Master_Report = 9221,

        /// <summary>
        /// 9222
        /// </summary>
        Order_Planning_Report = 9222,

        /// <summary>
        /// 9223
        /// </summary>
        Payment_Receipt_Report = 9223,

        /// <summary>
        /// 9224
        /// </summary>
        Production_Report = 9224,

        /// <summary>
        /// 9225
        /// </summary>
        Salary_Report = 9225,

        /// <summary>
        /// 9226
        /// </summary>
        Sales_Analysis_Report = 9226,

        /// <summary>
        /// 9227
        /// </summary>
        Sales_Report = 9227,

        /// <summary>
        /// 9228
        /// </summary>
        Stores_PO_Report = 9228,

        /// <summary>
        /// 9229
        /// </summary>
        Tender_Report = 9229,

        /// <summary>
        /// 9230
        /// </summary>
        Inspection_Report = 9230,

        //master = 10
        Railway_Master = 1001,
        Party_Master = 1005,
        Paying_Authority_Master = 1010,
        Registration_Master = 1015,
        Item_Master = 1020,
        Consignee_Master = 1025,
        Company_Address = 1030,
        Masters_Updation_for_DI = 1040,
        Item_Group_Master = 1045,
        City_Master = 1050,
        Govt_Department_Master = 1055,
        Agent_Master = 1060,
        //tender = 11
        Tender_Entry = 1101,
        Tender_Item = 1105,
        //Tender_Checksheet_Report = 1125,
        //E_Tender_Report = 1130,
        NIT_List_Upload = 1135,
        NIT_List_Processing = 1140,
        NIT_Process_Updation = 1145,
        NIT_List_Delete = 1150,
        Tender_Dispatch = 1155,
        Email_Track_Entry = 1160,
        Email_Track_Reply = 1165,
        //purchase order = 12
        Purchase_Order_Entry = 1201,
        //Order_Execution_Report = 1215,
        //Purchase_Order_Report = 1220,
        Verify_PO = 1225,
        Update_BG = 1230,
        //BG_Report = 1235,
        //sales = 13
        Draft_Proforma_Invoice = 1301,
        IC_DM_Updation = 1305,
        Sale_Invoice_Main = 1310,
        Scrap_Transfer = 1315,
        Unloading_Mark = 1320,
        //Sale_Invoice_Report = 1325,
        SaleEntryVNoChange_Admin = 1330,
        Internal_PO_Sale_Invoice = 1335,
        Sale_Bill_Entry = 1345,
        Sale_Return_Entry = 1350,
        EInvoice_Updation = 1355,
        AgentWorkAssignment = 1356,
        //Receipt = 14
        Receipt_Entry = 1401,
        Bill_Receipt_Entry = 1425,
        //Document Files = 15,
        Upload_Docs_Item = 1501,
        Upload_Docs_Tender = 1505,
        Upload_Docs_Pvt_Party = 1510,
        Upload_Docs_StdSpecs = 1515,
        Upload_Dispach_Document = 1520,
        Upload_Invoice_Document = 1522,
        Dispatch_Document_Report = 1525,
        Document_Type_Master = 1530,
        Document_Category_Master = 1535,
        Download_Docs_Item = 1540,
        Download_Docs_Tender = 1545,
        Download_Docs_Pvt_Party = 1550,
        Download_Docs_StdSpecs = 1555,
        Download_Dispach_Document = 1560,
        Download_Invoice_Document = 1565,
        Upload_Company_Document = 1570,
        Download_Company_Document = 1575,
        //collaboration = 16
        Collaboration_File_Upload = 1601,
        Collaboration_File_Download = 1605,
        Collaboration_File_Update = 1610,
        Collaboration_Project_Entry = 1615,
        Collaboration_Category_Master = 1620,
        //calibration = 17
        Calibration_Entry = 1701,
        WipToRg1_Entry = 1702,
        //Calibration_View = 1705,
        Imte_Type_Master = 1720,
        Imte_Master = 1725,
        //reports = 18
        Item_Master_Report = 1801,
        Consignee_Master_Report = 1805,
        Paying_Authority_Report = 1810,
        Party_Master_View = 1815,
        Registration_Report = 1820,
        //Item_Performance_Report = 1825,
        //dashboard = 19
        Management_Dashboard = 1901,
        Collaboration_Dashboard = 1905,
        Marketing_HOD_Dashboard = 1910,
        Marketing_User_Dashboard = 1915,
        Quality_Control_Dashboard = 1920,
        Quality_Production_Dashboard = 1925,
        //SaleSummaryDsbReport = 1930,
        //AgentwiseReports = 1935,
        //TenderDsbRpt = 1940,
        //PurchaseOrderDsbRpt = 1945,
        //DispatchUnloadingDsbRpt = 1950,
        //BillingReceiptDsbRpt = 1955,
        //DispatchPlanDsbRpt = 1960,
        HR_Dashboard = 1965,
        Store_Dashboard = 1970,
        Marketing_Menu = 1975,
        ITCELL_Dashboard = 1976,
        CRM_Dashboard = 1978,
        //options/admin = 20
        User_Admin = 2001,
        User_Permission = 2005,
        Collab_Permission = 2010,
        Collab_Permission_ByProject = 2015,
        Event_Log = 2020,
        User_Log_Report = 2025,
        Company_Master = 2030,
        QuailBook_Entry = 2035,
        //QuailBook_Report = 2040,
        Account_Group = 2045,
        Account_Master = 2050,
        Voucher_Entry = 2055,
        //Account_Reports = 2060,
        //store dept = 21
        //StoreItemEntry = 2101,
        VendorEntry = 2105,
        OrderEntry_Store = 2110,
        //OrderReport_Store = 2115,
        StoreEntry = 2120,
        PurchaseEntry_Store = 2125,
        ProductionEntry = 2130,
        PurchaseReport_Store = 2135,
        //ProductionReport = 2140,
        IndentEntry = 2145,
        IndentReport = 2150,
        IndentApprovalHOD = 2155,
        IndentExecution = 2160,
        IndentApprovalAdmin = 2165,
        JobworkEntry = 2170,
        //JobworkReport = 2175,
        ItemReceiptEntry = 2180,
        UploadTallyData = 2190,
        AMC_Entry = 2146,
        AMC_Report = 2147,
        InspectionEntry = 2148,
        //HR dept = 22
        EmployeeMaster = 2201,
        AttendanceEntry = 2205,
        AttendanceDetailEntry = 2210,
        AdvanceEntry = 2215,
        SalaryEntry = 2220,
        //EmployeeReport = 2225,
        //AttendanceReport = 2230,
        //AttendanceDetailReport = 2235,
        //AdvanceReport = 2240,
        //SalaryReport = 2245,
        DirectorsInfo = 2250,
        UploadEmployeeDocument = 2255,
        DownloadEmployeeDocument = 2260,
        BackdateAttendanceUpdation = 2265,
        SalaryDetailUpdation = 2270,
        PromotionDetailUpdation = 2275,
        EmployeeOpeningEntry = 2280,
        MinimumWage = 2285,
        FormValue_Entry = 2290,
        //Form_Report = 2295,
        DirectoryUpdation = 2296,
        BirthdayAnnivAlert = 2297,

        //compliances = 24 [from worklist]
        //Compliance_New = 2401, //add another dep here
        Compliance_HO = 2405,
        Compliance_HR = 2410,
        //Compliance_ERP = 2415, //not erp, add another dep instead

        //complaints = 25 
        Complaint_Entry = 2501,
        Complaint_Reply_ERP = 2505,
        Complaint_Reply_HW = 2510,
        Complaint_Reply_Others = 2515,
        Complaint_Report = 2520,

        //others-1 permissions set 1 = 90
        Dispatch_Alert = 9001,
        Tender_Alert = 9005,
        L1_L2_Qty_Updation = 9010,
        L1_L2_Qty_View = 9015,
        LPR_LQR_View = 9020,
        Dashboard_Alerts = 9025,
        BOM_Definition = 9030,
        Apply_Item_Group = 9035,
        Import_List = 9040,
        StatisticalSummary_Report = 9045,
        Circular_Entry = 9050,
        Circular_View = 9055,

        //others-2 permissions set 2 = 91
        Notification_Entry = 9101,
        Notification_Report = 9105,
        Correspondence_Entry = 9110,
        Correspondence_Report = 9115,
        Feedback_Entry = 9130,
        Feedback_Report = 9135,
        DakRegister_Entry = 9140,
        DakRegister_Report = 9145,
        Biometric_Data = 9150,

        //new
        Civil = 9155,
        MasterFileUpload = 9160,
        Coplaint = 9165,
        CoplaintAssign = 9166,
        Library = 9170,
        LibraryDownload = 9171,
        MasterFileDownload = 9175,
        MasterFile = 9176,
        MedicalTest = 9180,
        PowerConsuption = 9182,
        ItAssest = 9185,
        MachineMaintenance = 9186,
        CivilIndentHOD = 9156,
        CivilIndentFinal = 9157,
        Training = 9158,
        CRM_Entry = 9188,
        CRM_Assign = 9189,
        Memo = 9190
    }
    //
    public enum dbTables
    {
        /*
        updated by tbl_entryinfo
        insert into tbl_entryinfo(tblid,tblname,pkfield) 
        values('<tblid>','<tblname>','<pkfield>')
        */

        tbl_users = 1,
        tbl_company = 2,
        tbl_state = 3,
        tbl_city = 4,
        tbl_area = 5,
        tbl_division = 6,
        tbl_zone = 7,
        tbl_territory = 8,
        tbl_acmast = 9,
        tbl_purchase = 10,
        tbl_voucher = 11,
        tbl_dailywork = 12,
        tbl_category = 13,
        tbl_purchaseorder = 14,
        tbl_sale = 15,
        tbl_tender = 16,
        tbl_registration = 17,
        tbl_etender = 18,
        tbl_receipt = 19,
        tbl_calibration = 20,
        tbl_collabproject = 21,
        tbl_govtdepartment = 22,
        tbl_vendor = 23,
        tbl_order = 24,
        tbl_employee = 25,
        tbl_attendance = 26,
        tbl_attendancedetail = 27,
        tbl_advance = 28,
        tbl_salary = 29,
        tbl_railway = 30,
        tbl_consignee = 31,
        tbl_item = 32,
        tbl_payingauthority = 33,
        tbl_stock = 34,
        tbl_productionentry = 35,
        tbl_salarydetail = 36,
        tbl_promotiondetail = 37,
        tbl_employeeopening = 38,
        tbl_porderdetail = 39,
        tbl_ledger = 40,
        tbl_vehicle = 41,
        tbl_compliancetype = 42,
        tbl_compliance = 43,
        tbl_vehicletype = 44,
        tbl_travel = 45,
        tbl_nitlist = 46,
        tbl_minwage = 47,
        tbl_indent = 48,
        tbl_itemgroup = 49,
        tbl_vendoraddress = 50,
        tbl_ordertype = 51,
        tbl_notification = 52,
        tbl_correspondence = 53,
        tbl_complaint = 54,
        tbl_formvalue = 55,
        tbl_jobworkissue = 56,
        tbl_jobworkreceipt = 57,
        tbl_stockledger = 58,
        tbl_citym = 59,
        tbl_feedback = 60,
        tbl_salebill = 61,
        tbl_billreceipt = 62,
        tbl_companyaddress = 63,
        tbl_dakregister = 64,
        tbl_indentledger = 65,
        tbl_worklist = 66,
        tbl_importlist = 67,
        tbl_tallydrr = 68,
        tbl_unitconversion = 69,
        tbl_productionplan =70,
        tbl_tenderdispatch = 71,
        tbl_quailbook = 72,
        tbl_quailfunction = 73,
        tbl_quailmeeting = 74,
        tbl_functionmember = 75,
        tbl_bankguarantee = 76,
        tbl_party = 77,
        AAA_InvoiceSave_Detail_ITCell = 78,
        tbl_emailtrack = 79,
        tbl_documentsubcategory = 80,
        tbl_masterdocumentname = 81,
        tbl_imtetype = 82,
        tbl_imte = 83,
        tbl_bghistory = 84,
        LibraryRecordInsert = 85,
        ContractorIndent = 86,
        MasterLibraryFileUpload = 87,
        AAA_MedicalTest = 88,
        PowerConsuptionMonitor = 89,
        Coplaint = 90,
        tbl_agent = 91,
        tbl_rfqtender = 92,
        MachineMaintenance = 93,
        Letter = 94,
        Training_Insert = 95,
        tbl_circular = 96,
        Trainning_Group = 97,
        c_ItemFitting = 98,
        c_complaint = 99,
        c_Shed_Traning = 100,
        tbl_tenderdetail = 101,
        tbl_bom = 102,
        AAA_EmpMedicalTest_Vaccine = 103,
        tbl_amc = 104,
        Memo = 105,
        tbl_inspectionentry = 106,
        tbl_agentworkassign = 107,
        tbl_modifyadvlist = 108
    }
    //
    public enum permissionType
    {
        /// <summary>
        /// Permission to add record
        /// </summary>
        Add = 1,
        /// <summary>
        /// Permission to edit record
        /// </summary>
        Edit = 2,
        /// <summary>
        /// Permission to delete record
        /// </summary>
        Delete = 3
    }
    //
    public enum collabPermissionType
    {
        /// <summary>
        /// Permission to view project
        /// </summary>
        View = 1,
        /// <summary>
        /// Permission to upload project
        /// </summary>
        Upload = 2,
        /// <summary>
        /// Permission to download project
        /// </summary>
        Download = 3,
        /// <summary>
        /// Permission to delete file
        /// </summary>
        Delete = 4
    }
    //
    #region account enums

    public enum drCrType
    {
        Dr = 1,
        Cr = 2
    }
    //
    /// <summary>
    /// Account posting direction
    /// </summary>
    public enum postDirection
    {
        Forward = 0,
        Reverse = 1
    }
    //
    /// <summary>
    /// For account record type from rectype field of tbl_acmast
    /// acoount-a,group-g
    /// </summary>
    public enum recType
    {
        /// <summary>
        /// =a
        /// </summary>
        Account = 'a',
        /// <summary>
        /// =g
        /// </summary>
        Group = 'g'
    }
    /// <summary>
    /// For Cash Type from cash field of tbl_acmast
    /// c,b,p,r
    /// </summary>
    public enum cashType
    {
        /// <summary>
        /// =x
        /// </summary>
        All = 'x',
        /// <summary>
        /// =c
        /// </summary>
        Cash = 'c',
        /// <summary>
        /// =b
        /// </summary>
        Bank = 'b',
        /// <summary>
        /// =p
        /// </summary>
        Payment = 'p',
        /// <summary>
        /// =r
        /// </summary>
        Receipt = 'r'
    }
    //
    /// <summary>
    /// For Account Type from actype field of tbl_acmast
    /// a,l,p
    /// </summary>
    public enum acType
    {
        /// <summary>
        /// =a
        /// </summary>
        Asset = 'a',
        /// <summary>
        /// =l
        /// </summary>
        Libility = 'l',
        /// <summary>
        /// =p
        /// </summary>
        Payment = 'p'
    }
    //
    /// <summary>
    /// Account display option
    /// </summary>
    public enum actOption
    {
        RecType = 0,
        CashType = 1,
        Groupwise = 2
    }
    //
    /// <summary>
    /// Fixed Accounts
    /// </summary>
    public enum fAccount
    {
        All = 0,
        /// <summary>
        /// accode = 7
        /// </summary>
        CurrentLiabilities = 7,
        /// <summary>
        /// accode = 8
        /// </summary>
        SundryCreditors = 8,
        /// <summary>
        /// accode = 16
        /// </summary>
        SundryDebtors = 16,
        /// <summary>
        /// accode = 54
        /// </summary>
        SaleAccount = 54,
        /// <summary>
        /// accode = 55
        /// </summary>
        PurchaseAccount = 55,
        /// <summary>
        /// accode = 18
        /// </summary>
        Bank = 18,
        /// <summary>
        /// accode = 50
        /// </summary>
        CashInHand = 50,
        /// <summary>
        /// accode = 22
        /// </summary>
        SalesRevenue = 22,
        /// <summary>
        /// accode = 24
        /// </summary>
        DirectExpenses = 24,
        /// <summary>
        /// accode = 57
        /// </summary>
        CounterSale = 57,
        /// <summary>
        /// accode = 29
        /// </summary>
        CommissionAccruel = 29,
        /// <summary>
        /// accode = 30
        /// </summary>
        Broker = 30,
        /// <summary>
        /// accode = 31
        /// </summary>
        SGST = 31,
        /// <summary>
        /// accode = 32
        /// </summary>
        CGST = 32,
        /// <summary>
        /// accode = 34
        /// </summary>
        IGST = 34,
        /// <summary>
        /// accode = 35
        /// </summary>
        ServiceTax = 35,
        /// <summary>
        /// accode = 52
        /// </summary>
        FreightAccount = 52,
        /// <summary>
        /// accode = 58
        /// </summary>
        Employee = 58
    }
    //
    public enum reportName
    {
        /// <summary>
        /// To display/print
        /// day book report
        /// </summary>
        DayBook = 1,
        /// <summary>
        /// To display/print
        /// cash book report 
        /// </summary>
        CashBook = 2,
        /// <summary>
        /// To display/print
        /// bank book report
        /// </summary>
        BankBook = 3,
        /// <summary>
        /// To display/print
        /// balance sheet report
        /// </summary>
        BalanceSheet = 4,
        /// <summary>
        /// To display/print
        /// profit and loss report
        /// </summary>
        ProfitNdLoss = 5
    }
    //
    /// <summary>
    /// Profit and Loss phase options
    /// </summary>
    public enum pnlPhase
    {
        Phase1 = 1,
        Phase2 = 2
    }
    //
    public enum dsbDept
    {
        Front = 1,
        Main = 2,
        Admin = 3,
        HR = 4,
        Stores = 5,
        Production = 6,
        Compliance = 7,
        Indent = 8,
        Marketing = 9,
        QuailBook = 10,
        Accounts = 11,
        Quality = 12,
        Home = 13,
        ITCELL = 14,
        Civil = 15,
        DocumentControl = 16,
        Electrical = 17
    }
    //
    public enum userOption
    {
        QmId = 1
    }
    /// <summary>
    /// Bill outstanding type
    /// </summary>
    public enum billOSType
    {
        /// <summary>
        /// Receipt = r
        /// </summary>
        Receipt = 1,
        /// <summary>
        /// Payment = p
        /// </summary>
        Payment = 2
    }
    //
    #endregion
    //
}