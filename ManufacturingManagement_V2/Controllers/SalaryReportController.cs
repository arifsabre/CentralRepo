using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;

namespace ManufacturingManagement_V2.Controllers
{
    public class SalaryReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            DateTime dtx = DateTime.Now.AddMonths(-1);
            rptOption.AttYear = dtx.Year;
            rptOption.AttMonth = dtx.Month;
            rptOption.AttShift = "d";
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.GradeList = new SelectList(EmployeeBLL.Instance.getEmployeeGradeList(), "ObjectCode", "ObjectName");
            ViewBag.MonthList = new SelectList(mc.getMonthList(), "Value", "Text");
            ViewBag.ShiftList = new SelectList(mc.getShiftList(), "Value", "Text");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            ViewBag.AgencyList = new SelectList(EmployeeBLL.Instance.getAgencyList(), "ObjectId", "ObjectName");
            ViewBag.LocationList = new SelectList(EmployeeBLL.Instance.getLocationList(), "ObjectId", "ObjectName");
            rptOption.ReportDate = DateTime.Now;
            return View(rptOption);
        }

        private void setLoginInfo(CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc)
        {
            DataTable lginfo = mc.getCrptLoginInfo();
            CrystalDecisions.Shared.TableLogOnInfos crtableLogoninfos = new CrystalDecisions.Shared.TableLogOnInfos();
            CrystalDecisions.Shared.TableLogOnInfo crtableLogoninfo = new CrystalDecisions.Shared.TableLogOnInfo();
            CrystalDecisions.Shared.ConnectionInfo crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
            crConnectionInfo.ServerName = lginfo.Rows[0]["svrname"].ToString();
            crConnectionInfo.DatabaseName = lginfo.Rows[0]["dbname"].ToString();
            crConnectionInfo.UserID = lginfo.Rows[0]["userid"].ToString();
            crConnectionInfo.Password = lginfo.Rows[0]["passw"].ToString();
            CrystalDecisions.CrystalReports.Engine.Tables CrTables = rptDoc.Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
            {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }
        }

        #region salary register

        [HttpPost]
        public ActionResult SalaryRegister(rptOptionMdl rptOption)
        {
            //[100118]/F1
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getSalaryRegisterFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&attshift=" + rptOption.AttShift + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSalaryRegisterFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, attshift = rptOption.AttShift, compcode = rptOption.CompCode, grade = rptOption.Grade, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public ActionResult getSalaryRegisterFile(int attmonth, int attyear, string attshift, int compcode, string grade, string rptformat)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                //return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/SalaryRpt.rdlc");
            Reports.dsReport dsr = new Reports.dsReport();
            System.Data.DataTable dtr = new DataTable();
            //[100118]/F1/RDLC
            dtr = rptBLL.getSalaryReportData(attmonth, attyear, attshift, compcode, grade, "");
            dsr.Tables["tbl_salary"].Merge(dtr);
            //
            ReportParameter rp = new ReportParameter("prRptHead", "Salary Register  " + rptBLL.Message);
            localReport.SetParameters(new ReportParameter[] { rp });
            //
            ReportDataSource reportDataSource = new ReportDataSource("dsReport", dsr.Tables["tbl_salary"]);
            localReport.DataSources.Add(reportDataSource);
            //
            string filename = "SalaryReport." + mc.getNameByKey(mc.getReportFormats(), "format", rptformat, "ext");
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>'" + rptformat + "'</OutputFormat>" +
            "  <PageWidth>1080mm</PageWidth>" +
            "  <MarginTop>0.3in</MarginTop>" +
            "  <MarginLeft>0.5in</MarginLeft>" +
            "  <MarginRight>0.2in</MarginRight>" +
            "  <MarginBottom>0.3in</MarginBottom>" +
            "  <FileName>'" + filename + "'</FileName>" +
            "</DeviceInfo>";
            //
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                rptformat,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }

        #endregion

        #region salary sheet

        [HttpPost]
        public ActionResult SalarySheet(rptOptionMdl rptOption)
        {
            //[100118]/F2
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getSalarySheetFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&attshift=" + rptOption.AttShift + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSalarySheetFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, attshift = rptOption.AttShift, compcode = rptOption.CompCode, grade = rptOption.Grade, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public ActionResult getSalarySheetFile(int attmonth, int attyear, string attshift, int compcode, string grade, string rptformat)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            LocalReport localReport = new LocalReport();
            //[100118]/F2/RDLC
            localReport.ReportPath = Server.MapPath("~/Reports/SalarySheetRpt.rdlc");
            string rpth = "SALARY SHEET ";
            if (grade == "w")
            {
                localReport.ReportPath = Server.MapPath("~/Reports/WageSheetRpt.rdlc");
                rpth = "WAGES SHEET ";
            }
            Reports.dsReport dsr = new Reports.dsReport();
            System.Data.DataTable dtr = new DataTable();
            dtr = rptBLL.getSalaryReportData(attmonth, attyear, attshift, compcode, grade, "");
            dsr.Tables["tbl_salary"].Merge(dtr);

            string gradename = mc.getNameByKey(EmployeeBLL.Instance.getEmployeeGradeData(), "gradecode", grade, "gradename");
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            ReportParameter rp1 = new ReportParameter("prCmpName", compMdl.CmpName);
            ReportParameter rp2 = new ReportParameter("prCmpAddress", compMdl.Address1 + " " + compMdl.Address2 + " " + compMdl.Address3);
            ReportParameter rp3 = new ReportParameter("prRptHead", rpth + "FOR THE MONTH OF: " + monthname.ToUpper() + ", " + attyear.ToString()+", Grade: " + gradename);
            localReport.SetParameters(new ReportParameter[] { rp1,rp2,rp3 });
            //
            ReportDataSource reportDataSource = new ReportDataSource("dsReport", dsr.Tables["tbl_salary"]);
            localReport.DataSources.Add(reportDataSource);
            //
            string filename = "SalaryReport." + mc.getNameByKey(mc.getReportFormats(), "format", rptformat, "ext");
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>'" + rptformat + "'</OutputFormat>" +
            "  <PageWidth>356mm</PageWidth>" +
            "  <PageHeight>216mm</PageHeight>" +
            "  <MarginTop>0.3in</MarginTop>" +
            "  <MarginLeft>0.5in</MarginLeft>" +
            "  <MarginRight>0.2in</MarginRight>" +
            "  <MarginBottom>0.3in</MarginBottom>" +
            "  <FileName>'" + filename + "'</FileName>" +
            "</DeviceInfo>";
            //
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                rptformat,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }

        #endregion

        #region salary with bakndetail

        [HttpPost]
        public ActionResult SalaryWithBankDetail(rptOptionMdl rptOption)
        {
            //[100118]/F3, Not in Use
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getSalaryWithBankDetailFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&attshift=" + rptOption.AttShift + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                reportpms += "&bankopt=" + rptOption.Detailed + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSalaryWithBankDetailFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, attshift = rptOption.AttShift, compcode = rptOption.CompCode, grade = rptOption.Grade, rptformat = rptOption.ReportFormat, bankopt = rptOption.Detailed });
        }

        [HttpGet]
        public ActionResult getSalaryWithBankDetailFile(int attmonth, int attyear, string attshift, int compcode, string grade, string rptformat, bool bankopt = false)
        {
            //Not in Use
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            LocalReport localReport = new LocalReport();
            //[100118]/F3/RDLC
            localReport.ReportPath = Server.MapPath("~/Reports/SalaryWithBankDetailRpt.rdlc");
            string rpth = "SALARY ";
            if (grade.ToLower() == "w")
            {
                rpth = "WAGES ";
            }
            Reports.dsReport dsr = new Reports.dsReport();
            System.Data.DataTable dtr = new DataTable();
            dtr = rptBLL.getSalaryReportData(attmonth, attyear, attshift, compcode, grade, "salarybankdetail");
            string rfilter = "bankname = 'PUNJAB NATIONAL BANK'";
            string bank = "PNB";
            if (bankopt == true)
            {
                rfilter = "bankname = 'PUNJAB NATIONAL BANK'";
            }
            else
            {
                bank = "Non-PNB";
                rfilter = "bankname <> 'PUNJAB NATIONAL BANK'";
            }
            dtr.DefaultView.RowFilter = rfilter + " and mark = 0 and dayspaid > 0";//note:
            dsr.Tables["tbl_salary"].Merge(dtr.DefaultView.ToTable());
            //
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            ReportParameter rp1 = new ReportParameter("prCmpName", compMdl.CmpName);
            ReportParameter rp2 = new ReportParameter("prCmpAddress", compMdl.Address1 + " " + compMdl.Address2 + " " + compMdl.Address3);
            ReportParameter rp3 = new ReportParameter("prRptHead", rpth + "FOR THE MONTH OF: " + monthname.ToUpper() + ", " + attyear.ToString()+", Bank: " + bank);
            localReport.SetParameters(new ReportParameter[] { rp1, rp2, rp3 });
            //
            ReportDataSource reportDataSource = new ReportDataSource("dsReport", dsr.Tables["tbl_salary"]);
            localReport.DataSources.Add(reportDataSource);
            //
            string filename = "SalaryReport." + mc.getNameByKey(mc.getReportFormats(), "format", rptformat, "ext");
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>'" + rptformat + "'</OutputFormat>" +
            "  <PageWidth>210mm</PageWidth>" +
            "  <PageHeight>297mm</PageHeight>" +
            "  <MarginTop>0.3in</MarginTop>" +
            "  <MarginLeft>0.5in</MarginLeft>" +
            "  <MarginRight>0.2in</MarginRight>" +
            "  <MarginBottom>0.3in</MarginBottom>" +
            "  <FileName>'" + filename + "'</FileName>" +
            "</DeviceInfo>";
            //
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                rptformat,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }
        
        #endregion

        #region salary slip

        [HttpPost]
        public ActionResult SalarySlip(rptOptionMdl rptOption)
        {
            //[100118]/F4
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getSalarySlipFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&rptdate=" + mc.getStringByDateForReport(rptOption.ReportDate) + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSalarySlipFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, compcode = rptOption.CompCode, rptdate = rptOption.ReportDate });
        }

        [HttpGet]
        public FileResult getSalarySlipFile(int attmonth, int attyear, int compcode, DateTime rptdate)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }

            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SalaryRPT/"), "SalarySlipW.rpt"));
            setLoginInfo(rptDoc);
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section3"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "SALARY SLIP FOR THE MONTH OF " + monthname.ToUpper() + " - " + attyear.ToString();

            //dbp parameters   --usp_get_salary_slip_report
            rptDoc.SetParameterValue("@attmonth", attmonth);
            rptDoc.SetParameterValue("@attyear", attyear);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
            }
            return File(stream, "application/pdf");
        }

        [HttpGet]
        public ActionResult getSalarySlipFile_OLD(int attmonth, int attyear, string attshift, int compcode, string grade, string rptformat, DateTime rptdate)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (compcode == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Company not selected!</h1></a>");
            }
            if (grade.ToLower() != "w")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Salary slip is only for Workers grade!</h1></a>");
            }           
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            LocalReport localReport = new LocalReport();
            //[100118]/F4/RDLC
            localReport.ReportPath = Server.MapPath("~/Reports/SalarySlipRpt.rdlc");
            Reports.dsReport dsr = new Reports.dsReport();
            System.Data.DataTable dtr = new DataTable();
            dtr = rptBLL.getSalaryReportData(attmonth, attyear, attshift, compcode, grade, "salaryslip");
            dsr.Tables["tbl_salary"].Merge(dtr);
            //
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            ReportParameter rp1 = new ReportParameter("prCmpName", compMdl.CmpName);
            ReportParameter rp2 = new ReportParameter("prCmpAddress", compMdl.Address1+" "+compMdl.Address2+" "+compMdl.Address3);
            string rpthead = "SALARY SLIP FOR THE MONTH OF: " + monthname.ToUpper() + ", " + attyear.ToString();
            //if (compMdl.CompCode == 2 || compMdl.CompCode == 3 || compMdl.CompCode == 8)
            //{
            //    rpthead = "SALARY AS PER ENGINEERING WAGE BOARD, MONTH: " + monthname.ToUpper() + ", " + rptOption.AttYear.ToString();
            //}
            ReportParameter rp3 = new ReportParameter("prRptHead", rpthead);
            ReportParameter rp4 = new ReportParameter("prReportDate", rptdate.ToShortDateString());
            //ReportParameter rp3 = new ReportParameter("prRptHead", "SALARY SLIP FOR THE MONTH OF: " + monthname.ToUpper() + ", " + rptOption.AttYear.ToString());
            localReport.SetParameters(new ReportParameter[] { rp1,rp2,rp3,rp4 });
            //
            ReportDataSource reportDataSource = new ReportDataSource("dsReport", dsr.Tables["tbl_salary"]);
            localReport.DataSources.Add(reportDataSource); 
            //
            string filename = "SalaryReport." + mc.getNameByKey(mc.getReportFormats(), "format", rptformat, "ext");
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>'" + rptformat + "'</OutputFormat>" +
            "  <PageWidth>210mm</PageWidth>" +
            "  <PageHeight>297mm</PageHeight>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>0.5in</MarginRight>" +
            "  <FileName>'" + filename + "'</FileName>" +
            "</DeviceInfo>";
            //
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                rptformat,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }

        #endregion

        #region salary slip receipt

        [HttpPost]
        public ActionResult SalarySlipReceipt(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/GetSalarySlipReceiptHTML";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetSalarySlipReceiptHTML", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, compcode = rptOption.CompCode });
        }

        public ActionResult GetSalarySlipReceiptHTML(int attmonth, int attyear, int compcode)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (compcode == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Unit not selected!</h1></a>");
            }
            setViewData();

            //dataset from dbProcedures/HR_Reports_SP.sql
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetSalarySlipReceiptReportHtml(attmonth,attyear,compcode);

            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            sbHeader.Append("<div style='text-align:center;'>");//div main
            //company
            sbHeader.Append("<div style='font-size:12pt;'>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpName"].ToString());
            sbHeader.Append("</div>");
            //cmp address
            sbHeader.Append("<div style='font-size:10pt;'>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpFooter1"].ToString());
            sbHeader.Append("</div>");
            //repoprt name
            sbHeader.Append("<div style='font-size:11pt;background-color:lightgray;'>");
            sbHeader.Append("<b><u>Salary Slip Receipt</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString());
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            //report content
            double tlern = 0;
            double tlpf = 0;
            double tlesi = 0;
            double tladv = 0;
            double tltds = 0;
            double tlded = 0;
            double tlnet = 0;
            string str = "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;' rowspan='2'>SlNo</th>");
            sb.Append("<th style='width:15px;' rowspan='2'>EMP&nbsp;ID</th>");
            sb.Append("<th style='width:auto;' rowspan='2'>Name</th>");
            sb.Append("<th style='width:auto;' rowspan='2'>UAN/<br/>ESI&nbsp;No</th>");
            sb.Append("<th style='width:auto;' rowspan='2'>Paid<br/>Days</th>");
            sb.Append("<th style='width:15px;text-align:right;' rowspan='2'>Total<br/>Earning</th>");
            sb.Append("<th style='width:15px;text-align:center;' colspan='4'>Deductions</th>");//note
            sb.Append("<th style='width:15px;text-align:right;' rowspan='2'>Total<br/>Deduction</th>");
            sb.Append("<th style='width:15px;text-align:right;' rowspan='2'>Net<br/>Payable</th>");
            sb.Append("<th style='width:15px;' rowspan='2'>Signature&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</th>");
            sb.Append("</tr>");
            //
            sb.Append("<tr>");//subColumns-of-Deduction
            sb.Append("<th style='width:15px;text-align:right;'>PF</th>");
            sb.Append("<th style='width:15px;text-align:right;'>ESI</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Advance</th>");
            sb.Append("<th style='width:15px;text-align:right;'>TDS</th>");
            sb.Append("</tr>");
            //
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["EmpId"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["EmpName"].ToString() + "</td>");
                str = ds.Tables["tbl1"].Rows[i]["UAN"].ToString() + "<br/>" + ds.Tables["tbl1"].Rows[i]["esinumber"].ToString();
                sb.Append("<td>" + str + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["DaysPaid"].ToString() + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["TotalEarned"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["PFDeduction"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["ESIDeduction"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["AdvDeduction"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["TdsDeduction"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["TotalDeduction"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["NetPayable"].ToString())) + "</td>");
                sb.Append("<td>&nbsp;</td>");
                tlern += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["TotalEarned"].ToString());
                tlpf += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["PFDeduction"].ToString());
                tlesi += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["ESIDeduction"].ToString());
                tladv += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["AdvDeduction"].ToString());
                tltds += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["TdsDeduction"].ToString());
                tlded += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["TotalDeduction"].ToString());
                tlnet += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["NetPayable"].ToString());
                sb.Append("</tr>");
            }
            sb.Append("<tr class='tblrow'>");
            sb.Append("<td>&nbsp;</td>");
            sb.Append("<td>&nbsp;</td>");
            sb.Append("<td>&nbsp;</td>");
            sb.Append("<td>&nbsp;</td>");
            sb.Append("<td>Total</td>");
            sb.Append("<td align='right'>" + mc.getINRCFormat(tlern) + "</td>");
            sb.Append("<td align='right'>" + mc.getINRCFormat(tlpf) + "</td>");
            sb.Append("<td align='right'>" + mc.getINRCFormat(tlesi) + "</td>");
            sb.Append("<td align='right'>" + mc.getINRCFormat(tladv) + "</td>");
            sb.Append("<td align='right'>" + mc.getINRCFormat(tltds) + "</td>");
            sb.Append("<td align='right'>" + mc.getINRCFormat(tlded) + "</td>");
            sb.Append("<td align='right'>" + mc.getINRCFormat(tlnet) + "</td>");
            sb.Append("<td>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        #endregion

        #region form-12 agency

        [HttpPost]
        public ActionResult Form12AgencyReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getForm12AgencyReport";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&agencyid=" + rptOption.AgencyId + "";
                reportpms += "&locationid=" + rptOption.LocationId + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getForm12AgencyReport", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, grade = rptOption.Grade, agencyid = rptOption.AgencyId, locationid = rptOption.LocationId, compcode = rptOption.CompCode });
        }
        [HttpGet]
        public FileResult getForm12AgencyReport(int attmonth, int attyear, string grade, int agencyid, int locationid, int compcode)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SalaryRPT/"), "Form12Rpt_Agency.rpt"));
            setLoginInfo(rptDoc);

            //additional values
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtcmpname = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtcmpname"];
            txtcmpname.Text = compMdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpNameLeft = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpNameLeft"];
            txtCmpNameLeft.Text = compMdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress"];
            txtAddress.Text = compMdl.Footer1;
            CrystalDecisions.CrystalReports.Engine.TextObject txtmonth = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtmonth"];
            txtmonth.Text = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            CrystalDecisions.CrystalReports.Engine.TextObject txtyear = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtyear"];
            txtyear.Text = attyear.ToString();
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptInfo = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptInfo"];
            string agencyname = mc.getNameByKey(EmployeeBLL.Instance.getAgencyData(), "agencyid", agencyid.ToString(), "agencyname");
            string locationname = mc.getNameByKey(EmployeeBLL.Instance.getLocationData(), "locationid", locationid.ToString(), "locationname");
            txtRptInfo.Text = compMdl.ShortName + " -" + txtmonth.Text + ", " + txtyear.Text;
            if (agencyid > 0)
            {
                txtRptInfo.Text += "\r\nAgency: "+agencyname;
            }
            if (locationid > 0)
            {
                txtRptInfo.Text += "\r\nLocation: " + locationname;
            }
            //time text boxes
            CrystalDecisions.CrystalReports.Engine.TextObject txtC1R1 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                    rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC1R1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC1R2 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC1R2"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC1R3 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC1R3"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC1R4 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC1R4"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC2R1 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC2R1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC2R2 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC2R2"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC2R3 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC2R3"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC2R4 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC2R4"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC3R1 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC3R1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC3R2 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC3R2"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC3R3 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC3R3"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC3R4 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC3R4"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC4R1 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC4R1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC4R2 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC4R2"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC4R3 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC4R3"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC4R4 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC4R4"];           
            //
            txtC1R1.Text = "09:00 AM";
            txtC1R2.Text = "08:00 AM";
            txtC1R3.Text = "04:00 PM";
            txtC1R4.Text = "12:00 NIGHT";
            txtC2R1.Text = "01:30 PM";
            txtC2R2.Text = "12:00 NOON";
            txtC2R3.Text = "08:00 PM";
            txtC2R4.Text = "04:00 AM";
            txtC3R1.Text = "02:00 PM";
            txtC3R2.Text = "12:30 PM";
            txtC3R3.Text = "08:30 PM";
            txtC3R4.Text = "04:30 AM";
            txtC4R1.Text = "05:30 PM";
            txtC4R2.Text = "04:00 PM";
            txtC4R3.Text = "12:00 NIGHT";
            txtC4R4.Text = "08:00 AM";
            //

            //dbp parameters--hr_reports_sp/usp_get_form12_report_v2
            rptDoc.SetParameterValue("@attmonth", attmonth);
            rptDoc.SetParameterValue("@attyear", attyear);
            rptDoc.SetParameterValue("@grade", grade);
            rptDoc.SetParameterValue("@agencyid", agencyid);
            rptDoc.SetParameterValue("@locationid", locationid);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
            }
            return File(stream, "application/pdf");
        }

        public ActionResult getForm12AgencyReportHTML_1X(int attmonth, int attyear, string grade, int agencyid, int locationid, int compcode)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (compcode == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Unit not selected!</h1></a>");
            }
            setViewData();

            //dataset from dbProcedures/HR_Reports_SP.sql
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetForm12AgencyReportHtml(attmonth, attyear, grade, agencyid, locationid, compcode);

            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            sbHeader.Append("<div style='text-align:center;'>");//div main
            //company
            sbHeader.Append("<div style='font-size:12pt;'>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpName"].ToString());
            sbHeader.Append("</div>");
            //cmp address
            sbHeader.Append("<div style='font-size:10pt;'>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpFooter1"].ToString());
            sbHeader.Append("</div>");
            //repoprt name
            sbHeader.Append("<div style='font-size:11pt;background-color:lightgray;'>");
            sbHeader.Append("<b><u>Form 12 Agency</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString());
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            //report content
            //double tlern = 0;
            //double tlpf = 0;
            //double tlesi = 0;
            //double tladv = 0;
            //double tltds = 0;
            //double tlded = 0;
            //double tlnet = 0;
            string str = "";
            DateTime dtcmp = new DateTime(1900,1,1);
            DateTime lvdate = new DateTime(1900, 1, 1);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>EMP&nbsp;ID</th>");
            sb.Append("<th style='width:auto;'>EmpName</th>");
            sb.Append("<th style='width:auto;'>FatherName</th>");
            sb.Append("<th style='width:auto;'>Agency</th>");
            sb.Append("<th style='width:auto;'>Location</th>");
            sb.Append("<th style='width:15px;'>Category</th>");
            sb.Append("<th style='width:15px;text-align:center;'>1st</th>");
            sb.Append("<th style='width:15px;text-align:center;'>2nd</th>");
            sb.Append("<th style='width:15px;text-align:center;'>3rd</th>");
            sb.Append("<th style='width:15px;text-align:center;'>4rth</th>");
            sb.Append("<th style='width:15px;text-align:center;'>5th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>6th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>7th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>8th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>9th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>10th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>11th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>12th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>13th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>14th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>15th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>16th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>17th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>18th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>19th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>20th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>21st</th>");
            sb.Append("<th style='width:15px;text-align:center;'>22nd</th>");
            sb.Append("<th style='width:15px;text-align:center;'>23nd</th>");
            sb.Append("<th style='width:15px;text-align:center;'>24th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>25th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>26th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>27th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>28th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>29th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>30th</th>");
            sb.Append("<th style='width:15px;text-align:center;'>31st</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Total No. Of Days Worked</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Basic Rate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Basic Earning</th>");
            sb.Append("<th style='width:15px;text-align:right;'>VDA Rate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>VDA Earning</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Conv Rate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Conv Earning</th>");
            sb.Append("<th style='width:15px;text-align:right;'>CCA Rate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>CCA Earning</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Med. Allow. Rate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Med. Allow. Earning</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Dresswash Rate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Dresswash Earning</th>");
            sb.Append("<th style='width:15px;text-align:right;'>TotalAmount</th>");
            sb.Append("<th style='width:15px;text-align:right;'>On Acct of Provident Fund</th>");
            sb.Append("<th style='width:15px;text-align:right;'>On Acct of Insurance</th>");
            sb.Append("<th style='width:15px;text-align:right;'>On Acct of Advance</th>");
            sb.Append("<th style='width:15px;text-align:right;'>TDS/Misc Deduction</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Actual Wage Paid</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Deductable Weekoff Days</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Paid Leave (CL)</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Paid Leave (PL)</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Absent/Unpaid Days</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Week off days<br/>Allowed Leave</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Holiday<br/>Lockdown</th>");
            sb.Append("<th style='width:15px;text-align:left;'>Acknowledgement of <br/>Payment</th>");
            sb.Append("</tr>");
            //
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["EmpId"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["EmpName"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["FatherName"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["AgencyName"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["LocationName"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["CategoryName"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D01"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D02"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D03"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D04"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D05"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D06"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D07"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D08"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D09"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D10"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D11"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D12"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D13"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D14"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D15"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D16"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D17"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D18"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D19"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D20"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D21"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D22"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D23"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D24"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D25"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D26"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D27"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D28"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D29"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D30"].ToString() + "</td>");
                sb.Append("<td align='center'>" + ds.Tables["tbl1"].Rows[i]["D31"].ToString() + "</td>");
                sb.Append("<td align='right'>" + ds.Tables["tbl1"].Rows[i]["DaysWorked"].ToString() + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["BasicRate"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["BasicEarn"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["DARate"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["DAEarn"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["ConvRate"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["ConvEarn"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["CompAllowRate"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["CompAllowEarn"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["MedAllowRate"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["MedAllowEarn"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["DresswashRate"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["DresswashEarn"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["TotalEarned"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["PFDeduction"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["ESIDeduction"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["AdvDeduction"].ToString())) + "</td>");

                str = mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["TdsDeduction"].ToString())) + "<br/>";
                str += mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["MiscDeduction"].ToString()));
                sb.Append("<td align='right'>" + str + "</td>");

                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["NetPaid"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["DeductableWD"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["CasualLeave"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["PrivilegedLeave"].ToString())) + "</td>");
                
                str = ds.Tables["tbl1"].Rows[i]["ESILeave"].ToString()+"<br/>";
                str += ds.Tables["tbl1"].Rows[i]["LeaveWithoutPay"].ToString()+"<br/>";
                str += ds.Tables["tbl1"].Rows[i]["TAbsent"].ToString();
                sb.Append("<td align='right'>" + str + "</td>");

                str = ds.Tables["tbl1"].Rows[i]["WeekOffDay"].ToString() + "<br/>";
                str += ds.Tables["tbl1"].Rows[i]["AllowedLeave"].ToString();
                sb.Append("<td align='right'>" + str + "</td>");

                str = ds.Tables["tbl1"].Rows[i]["Holiday"].ToString() + "<br/>";
                str += ds.Tables["tbl1"].Rows[i]["Lockdown"].ToString();
                sb.Append("<td align='right'>" + str + "</td>");

                //acknow
                str = (i+1).ToString();
                lvdate = Convert.ToDateTime(ds.Tables["tbl1"].Rows[i]["leavingdate"].ToString());
                if ((lvdate.Day == 1 && lvdate.Month == 1 && lvdate.Year == 1900) ||
                    lvdate.Year.ToString() + lvdate.Month.ToString() != attyear.ToString() + attmonth.ToString())
                {
                    sb.Append("<td align='left'>" + str + "</td>");
                }
                else
                {
                    str += "&nbsp;&nbsp;Leaving Date: " + mc.getStringByDate(lvdate);
                    sb.Append("<td align='left'>" + str + "</td>");
                }
                //cl-acknow

                sb.Append("</tr>");
            }
            //sb.Append("<tr class='tblrow'>");
            //sb.Append("<td align='right'>" + mc.getINRCFormat(tlern) + "</td>");
            //sb.Append("<td align='right'>" + mc.getINRCFormat(tlpf) + "</td>");
            //sb.Append("<td align='right'>" + mc.getINRCFormat(tlesi) + "</td>");
            //sb.Append("<td align='right'>" + mc.getINRCFormat(tladv) + "</td>");
            //sb.Append("<td align='right'>" + mc.getINRCFormat(tltds) + "</td>");
            //sb.Append("<td align='right'>" + mc.getINRCFormat(tlded) + "</td>");
            //sb.Append("<td align='right'>" + mc.getINRCFormat(tlnet) + "</td>");
            //sb.Append("</tr>");
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        #endregion

        #region salary report for ho with neft download

        [HttpPost]
        public ActionResult SalaryNeftReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getSalaryNeftReport";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                reportpms += "&bankopt=" + rptOption.Detailed + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSalaryNeftReport", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, grade = rptOption.Grade, compcode = rptOption.CompCode, rptformat = rptOption.ReportFormat, bankopt = rptOption.Detailed });
        }

        public ActionResult getSalaryNeftReport(int attmonth, int attyear, string grade, int compcode, string rptformat, bool bankopt = true)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SalaryRPT/"), "SalaryNeftReport.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            string gradename = mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename");
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtFooter1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtFooter1"];
            txtCmpName.Text = compMdl.CmpName;
            txtFooter1.Text = compMdl.Footer1;
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Salary For " + monthname + " - " + attyear.ToString();
            if (grade.ToLower() == "d" || grade.ToLower() == "m" || grade.ToLower() == "s")
            {
                gradename = "Director, Manager and Staff";
            }
            else
            {
                gradename = mc.getNameByKey(EmployeeBLL.Instance.getEmployeeGradeData(), "gradecode", grade, "gradename");
            }
            txtRptHead.Text += ", Grade: " + gradename;
            int bOpt = 1;
            if (bankopt == true){
                txtRptHead.Text += ", Bank: PNB";
            }
            else {
                bOpt = 0;
                txtRptHead.Text += ", Bank: Non-PNB";
            }
            //dbp parameters   --usp_get_salary_neft_report
            rptDoc.SetParameterValue("@attmonth", attmonth);
            rptDoc.SetParameterValue("@attyear", attyear);
            rptDoc.SetParameterValue("@grade", grade);
            rptDoc.SetParameterValue("@bankopt", bOpt);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
            }
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "SalaryNEFT.xls");
            }
            return File(stream, "application/pdf");
        }

        #endregion

        #region salary report for head office

        [HttpPost]
        public ActionResult SalaryReportHO(rptOptionMdl rptOption)
        {
            //[100119]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getSalaryReportHO";
                string reportpms = "compcode=" + rptOption.CompCode + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&attmonth=" + rptOption.AttMonth + "";
                reportpms += "&attyear=" + rptOption.AttYear + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSalaryReportHO", new { compcode = rptOption.CompCode, grade = rptOption.Grade, attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public FileResult getSalaryReportHO(int compcode, string grade, int attmonth, int attyear, string rptformat)
        {
            //[100119]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "SalaryReportHO.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            string gradename = mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename");
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Salary For " + monthname + " - " + attyear.ToString();
            if (grade.ToLower() == "d" || grade.ToLower() == "m" || grade.ToLower() == "s")
            {
                gradename = "Director, Manager and Staff";
            }
            else
            {
                gradename = mc.getNameByKey(EmployeeBLL.Instance.getEmployeeGradeData(), "gradecode", grade, "gradename");
            }
            txtRptHead.Text += ", Grade: " + gradename;
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters   --usp_ho_salary_report
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@grade", grade);
            rptDoc.SetParameterValue("@attmonth", attmonth);
            rptDoc.SetParameterValue("@attyear", attyear);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "SalaryReportHO.xls");
            }
            return File(stream, "application/pdf");
        }

        #endregion

        #region form 12 report

        [HttpPost]
        public ActionResult Form12Report(rptOptionMdl rptOption)
        {
            //[100120]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getForm12ReportFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&attshift=" + rptOption.AttShift + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getForm12ReportFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, attshift = rptOption.AttShift, compcode = rptOption.CompCode, grade = rptOption.Grade, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public ActionResult getForm12ReportFile(int attmonth, int attyear, string attshift, int compcode, string grade, string rptformat)
        {
            //[100120]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            if (grade.ToLower() == "w")
            {
                //[100120]/F1/NCRPT
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Form12Rpt_Worker.rpt"));
            }
            else
            {
                //[100120]/F2/NCRPT
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Form12Rpt_Staff.rpt"));
            }
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            System.Data.DataTable dtratt = new DataTable();
            System.Data.DataTable dtrsalary = new DataTable();
            dtratt = rptBLL.getForm12ReportData(attmonth,attyear,attshift,compcode,grade);
            dsr.Tables["tbl_attendance"].Merge(dtratt);
            dtrsalary = rptBLL.getSalaryReportData(attmonth,attyear,attshift,compcode,grade,"");
            dsr.Tables["tbl_salary"].Merge(dtrsalary);
            //additional values
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtcmpname = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtcmpname"];
            txtcmpname.Text = compMdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpNameLeft = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpNameLeft"];
            txtCmpNameLeft.Text = compMdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress"];
            txtAddress.Text = compMdl.Footer1;
            CrystalDecisions.CrystalReports.Engine.TextObject txtmonth = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtmonth"];
            txtmonth.Text = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            CrystalDecisions.CrystalReports.Engine.TextObject txtyear = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtyear"];
            txtyear.Text = attyear.ToString();
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptInfo = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptInfo"];
            txtRptInfo.Text = compMdl.ShortName + " -" + txtmonth.Text + ", " + txtyear.Text;
            //time text boxes
            CrystalDecisions.CrystalReports.Engine.TextObject txtC1R1 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                    rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC1R1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC1R2 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC1R2"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC1R3 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC1R3"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC1R4 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC1R4"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC2R1 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC2R1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC2R2 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC2R2"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC2R3 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC2R3"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC2R4 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC2R4"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC3R1 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC3R1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC3R2 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC3R2"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC3R3 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC3R3"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC3R4 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC3R4"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC4R1 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC4R1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC4R2 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC4R2"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC4R3 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC4R3"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtC4R4 = (CrystalDecisions.CrystalReports.Engine.TextObject)
                rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtC4R4"];
            //
            txtC1R1.Text = "09:00 AM";
            txtC1R2.Text = "08:00 AM";
            txtC1R3.Text = "04:00 PM";
            txtC1R4.Text = "12:00 NIGHT";
            txtC2R1.Text = "01:30 PM";
            txtC2R2.Text = "12:00 NOON";
            txtC2R3.Text = "08:00 PM";
            txtC2R4.Text = "04:00 AM";
            txtC3R1.Text = "02:00 PM";
            txtC3R2.Text = "12:30 PM";
            txtC3R3.Text = "08:30 PM";
            txtC3R4.Text = "04:30 AM";
            txtC4R1.Text = "05:30 PM";
            txtC4R2.Text = "04:00 PM";
            txtC4R3.Text = "12:00 NIGHT";
            txtC4R4.Text = "08:00 AM";
            //
            rptDoc.SetDataSource(dsr);
            //rptDoc.Subreports[0].SetDataSource(dsr);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "Form12.xls");
            }
            return File(stream, "application/pdf");
        }

        #endregion

        #region incentive report

        [HttpPost]
        public ActionResult IncentiveReport(rptOptionMdl rptOption)
        {
            //[100101]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getIncentiveReportFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&attshift=" + rptOption.AttShift + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getIncentiveReportFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, attshift = rptOption.AttShift, compcode = rptOption.CompCode, rptformat = rptOption.ReportFormat, grade= rptOption.Grade });
        }

        [HttpGet]
        public ActionResult getIncentiveReportFile(int attmonth, int attyear, string attshift, int compcode, string rptformat, string grade)
        {
            //[100101]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "IncentiveReport.rpt"));
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            DataTable dtr = new DataTable();
            dtr = rptBLL.getIncentiveReportData(attmonth, attyear, attshift, compcode, grade);
            dsr.Tables["tbl_incentivedetail"].Merge(dtr);
            //additional values
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtcmpname = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtcmpname"];
            txtcmpname.Text = compMdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtaddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtaddress"];
            txtaddress.Text = compMdl.Address1 + compMdl.Address2 + compMdl.Address3;
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtrpthead"];
            txtrpthead.Text = rptBLL.Message;
            CrystalDecisions.CrystalReports.Engine.TextObject txttotaldays = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txttotaldays"];
            txttotaldays.Text = "Total Days = " + DateTime.DaysInMonth(attyear,attmonth);
            txttotaldays.Text += ", Total Working Hours in the Month @ 8 Hrs./Day = " + DateTime.DaysInMonth(attyear, attmonth) * 8;
            //
            rptDoc.SetDataSource(dsr);
            //rptDoc.Subreports[0].SetDataSource(dsr);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "IncentiveReport.xls");
            }
            return File(stream, "application/pdf");
        }

        #endregion

        #region esi return report

        [HttpPost]
        public ActionResult ESIReturnReport(rptOptionMdl rptOption)
        {
            //[100121]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getESIReturnReportFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getESIReturnReportFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, compcode = rptOption.CompCode, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public ActionResult getESIReturnReportFile(int attmonth, int attyear, int compcode, string rptformat)
        {
            //[100121]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ReturnESIRpt.rpt"));
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            DataTable dtr = new DataTable();
            dtr = rptBLL.getESIReturnData(attmonth, attyear, compcode);
            dsr.Tables["tbl_pfesireturn"].Merge(dtr);
            //additional values
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtcmpname = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtcmpname"];
            txtcmpname.Text = compMdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtaddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtaddress"];
            txtaddress.Text = compMdl.Address1 + compMdl.Address2 + compMdl.Address3;
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtrpttitle"];
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            txtrpttitle.Text = "ESI Return for " + monthname + " " + attyear.ToString();
            //
            rptDoc.SetDataSource(dsr);
            //rptDoc.Subreports[0].SetDataSource(dsr);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "ESIReturn.xls");
            }
            return File(stream, "application/pdf");
        }

        #endregion

        #region esi return report -for csv

        [HttpPost]
        public ActionResult ESIReturnReportCSV(rptOptionMdl rptOption)
        {
            //[100121]/CSV
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getESIReturnReportCSVFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getESIReturnReportCSVFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, compcode = rptOption.CompCode, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public ActionResult getESIReturnReportCSVFile(int attmonth, int attyear, int compcode, string rptformat)
        {
            //[100121]/CSV
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            DataTable dtr = new DataTable();
            dtr = rptBLL.getESIReturnData(attmonth, attyear, compcode);
            string reportmatter = "<table border='0' cellpadding='4' cellspacing='0' style='font-size:10pt;'>";
            for (int i = 0; i < dtr.Rows.Count; i++)
            {
                reportmatter += "<tr>";
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["esinumber"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["empname"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["dayspaid"].ToString().Split('.')[0] + "</td>";
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["esicalamt"].ToString().Split('.')[0] + "</td>";
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["esideduction"].ToString().Split('.')[0] + "</td>";
                reportmatter += "</tr>";
            }
            reportmatter += "</table>";
            return Content("<html><body style='font-family:verdana;font-size:10pt;'>" + reportmatter + "</body></html>");
        }

        #endregion

        #region pf return report

        [HttpPost]
        public ActionResult PFReturnReport(rptOptionMdl rptOption)
        {
            //[100122]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getPFReturnReportFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getPFReturnReportFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, compcode = rptOption.CompCode, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public ActionResult getPFReturnReportFile(int attmonth, int attyear, int compcode, string rptformat)
        {
            //[100122]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ReturnPFRpt.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["tbl_pfesireturn"].Merge(rptBLL.getPFReturnData(attmonth,attyear,compcode));
            //additional values
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtcmpname = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtcmpname"];
            txtcmpname.Text = compMdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtaddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtaddress"];
            txtaddress.Text = compMdl.Address1 + compMdl.Address2 + compMdl.Address3;
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtrpttitle"];
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            txtrpttitle.Text = "PF Return for " + monthname + " " + attyear.ToString();
            //
            rptDoc.SetDataSource(dsr);
            //rptDoc.Subreports[0].SetDataSource(dsr);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "PFReturn.xls");
            }
            return File(stream, "application/pdf");
        }

        #endregion 

        #region pf return report -for csv

        [HttpPost]
        public ActionResult PFReturnReportCSV(rptOptionMdl rptOption)
        {
            //[100122]/CSV
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getPFReturnReportCSVFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getPFReturnReportCSVFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, compcode = rptOption.CompCode, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public ActionResult getPFReturnReportCSVFile(int attmonth, int attyear, int compcode, string rptformat)
        {
            //[100122]/CSV
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            DataTable dtr = new DataTable();
            dtr = rptBLL.getPFReturnData(attmonth, attyear, compcode);
            string reportmatter = "<table border='0' cellpadding='4' cellspacing='0' style='font-size:10pt;'>";
            reportmatter += "<tr><td>";
            for (int i = 0; i < dtr.Rows.Count; i++)
            {
                reportmatter += dtr.Rows[i]["uan"].ToString() + "#~#";
                reportmatter += dtr.Rows[i]["empname"].ToString() + "#~#";
                reportmatter += dtr.Rows[i]["NetPaid"].ToString().Split('.')[0] + "#~#";
                reportmatter += dtr.Rows[i]["pfcalamt"].ToString().Split('.')[0] + "#~#";
                reportmatter += dtr.Rows[i]["pfcalamt"].ToString().Split('.')[0] + "#~#";
                reportmatter += dtr.Rows[i]["pfcalamt"].ToString().Split('.')[0] + "#~#";
                reportmatter += dtr.Rows[i]["pfdeduction"].ToString().Split('.')[0] + "#~#";
                reportmatter += dtr.Rows[i]["fpfamount"].ToString().Split('.')[0] + "#~#";
                reportmatter += dtr.Rows[i]["epfamount"].ToString().Split('.')[0] + "#~#";
                reportmatter += dtr.Rows[i]["ncpdays"].ToString().Split('.')[0] + "#~#";
                reportmatter += "0<br/>";
            }
            reportmatter += "</td></tr></table>";
            return Content("<html><body style='font-family:verdana;font-size:10pt;'>" + reportmatter + "</body></html>");
        }

        #endregion 

        #region pf return summary

        [HttpPost]
        public ActionResult PFReturnSummary(rptOptionMdl rptOption)
        {
            //[100123]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getPFReturnSummaryFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getPFReturnSummaryFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, compcode = rptOption.CompCode, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public ActionResult getPFReturnSummaryFile(int attmonth, int attyear, int compcode, string rptformat)
        {
            //[100123]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ReturnPFSummaryRpt.rpt"));
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            DataTable dtSummary = rptBLL.getPFSummaryData(attmonth,attyear,compcode);
            dsr.Tables["dtRptVariables"].Rows.Clear();
            System.Data.DataRow dr = dsr.Tables["dtRptVariables"].NewRow();
            dr["numcol1"] = dtSummary.Rows[0]["pfcount"].ToString();
            dr["numcol2"] = dtSummary.Rows[0]["nonpfcount"].ToString();
            dr["numcol3"] = dtSummary.Rows[0]["pfgrosswage"].ToString();
            dr["numcol4"] = dtSummary.Rows[0]["nonpfgrosswage"].ToString();
            dsr.Tables["dtRptVariables"].Rows.Add(dr);
            dsr.Tables["tbl_pfesireturn"].Merge(rptBLL.getNonPFReportData(attmonth,attyear,compcode));
            //additional values
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtcmpname = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtcmpname"];
            txtcmpname.Text = compMdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtaddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtaddress"];
            txtaddress.Text = compMdl.Address1 + compMdl.Address2 + compMdl.Address3;
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtrpttitle"];
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            txtrpttitle.Text = "PF Return Summary for " + monthname + " " + attyear.ToString();
            //
            rptDoc.SetDataSource(dsr);
            //rptDoc.Subreports[0].SetDataSource(dsr);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "PFReturnSummary.xls");
            }
            return File(stream, "application/pdf");
        }

        #endregion

        #region annual return salary

        [HttpGet]
        public ActionResult AnnualReturnSalary()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.AttYear = DateTime.Now.Year;
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult AnnualReturnSalary(rptOptionMdl rptOption)
        {
            //[100124]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getAnnualReturnSalaryFile";
                string reportpms = "attyear=" + rptOption.AttYear + "";
                reportpms += "&compcode=" + rptOption.JoiningUnit + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAnnualReturnSalaryFile", new { attyear = rptOption.AttYear, compcode = rptOption.JoiningUnit, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public ActionResult getAnnualReturnSalaryFile(int attyear, int compcode, string rptformat)
        {
            //[100124]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "SalaryAnnualReturnRpt.rpt"));
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["tbl_annualsalary"].Merge(rptBLL.getAnnualSalaryReturnData(attyear,compcode));
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptTitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptTitle"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress"];
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            txtRptTitle.Text = "Annual Return Report - " + attyear.ToString();
            txtCmpName.Text = compMdl.CmpName;
            txtAddress.Text = compMdl.Address1 + " " + compMdl.Address2 + " " + compMdl.Address3;
            //
            rptDoc.SetDataSource(dsr);
            //rptDoc.Subreports[0].SetDataSource(dsr);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "AnnualReturnSummary.xls");
            }
            return File(stream, "application/pdf");
        }

        #endregion

    }
}
