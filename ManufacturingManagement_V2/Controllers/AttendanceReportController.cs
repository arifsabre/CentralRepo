using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;
using System.Collections;

namespace ManufacturingManagement_V2.Controllers
{
    public class AttendanceReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        AttendanceBLL attBLL = new AttendanceBLL();
        //
        // GET: /AttendanceReport/

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.AttYear = DateTime.Now.Year;
            rptOption.AttMonth = DateTime.Now.Month;
            rptOption.AttShift = "d";
            rptOption.AttDay = DateTime.Now.Day;
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            ViewBag.MonthList = new SelectList(mc.getMonthList(), "Value", "Text");
            ViewBag.ShiftList = new SelectList(mc.getShiftList(), "Value", "Text");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            return View(rptOption);
        }

        public ActionResult Index1()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.AttYear = DateTime.Now.Year;
            rptOption.AttMonth = DateTime.Now.Month;
            rptOption.AttShift = "d";
            rptOption.AttDay = DateTime.Now.Day;
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            ViewBag.MonthList = new SelectList(mc.getMonthList(), "Value", "Text");
            ViewBag.ShiftList = new SelectList(mc.getShiftList(), "Value", "Text");
            //ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            return View(rptOption);
        }

        #region attendance report

        [HttpPost]
        public ActionResult AttendanceReport(rptOptionMdl rptOption)
        {
            //[100066]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Attendance_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Attendance_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "AttendanceReport/getAttendanceReportFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&shift=" + rptOption.AttShift + "";
                reportpms += "&compcode=" + rptOption.JoiningUnit.ToString() + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAttendanceReportFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, shift = rptOption.AttShift, compcode = rptOption.JoiningUnit, grade = rptOption.Grade, rptformat = rptOption.ReportFormat });
        }

        public ActionResult getAttendanceReportFile(int attmonth, int attyear, string shift, int compcode, string grade, string rptformat)
        {
            //[100066]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            AttendanceReportBLL rptBLL = new AttendanceReportBLL();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/AttendanceRpt.rdlc");//[100066]/RDLC
            Reports.dsReport dsr = new Reports.dsReport();
            System.Data.DataTable dtr = rptBLL.getAttendanceReportData(attmonth, attyear, shift, compcode, grade);
            string rpthead = rptBLL.Message;
            dsr.Tables["tbl_attendance"].Merge(dtr);
            //
            ReportParameter rp = new ReportParameter("prRptHead", "Attendance Report  " + rpthead);
            localReport.SetParameters(new ReportParameter[] { rp });
            //
            ReportDataSource reportDataSource = new ReportDataSource("dsReport", dsr.Tables["tbl_attendance"]);
            localReport.DataSources.Add(reportDataSource);
            //
            string filename = "AttendanceReport." + mc.getNameByKey(mc.getReportFormats(), "format", rptformat, "ext");
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>'" + rptformat + "'</OutputFormat>" +
            "  <PageWidth>620mm</PageWidth>" +
            "  <PageHeight>210mm</PageHeight>" +
            "  <MarginTop>0.3in</MarginTop>" +
            "  <MarginLeft>0.75in</MarginLeft>" +
            "  <MarginRight>0.3in</MarginRight>" +
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

        #region attendance history HTML

        public ActionResult getAttendanceHistoryHTML(int attendanceid)
        {
            //[100067]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Attendance_Report, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            attBLL = new AttendanceBLL();
            DataSet ds = new DataSet();
            ds = attBLL.getAttendanceHistoryData(attendanceid);
            if(ds.Tables.Count==0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>No record(s) found!</h1></a>");
            }
            if(ds.Tables[0].Rows.Count==0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>No record(s) found!</h1></a>");
            }
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", ds.Tables[0].Rows[0]["attmonth"].ToString(), "monthname");
            string rpthead = "For: " + monthname + "-" + ds.Tables[0].Rows[0]["attyear"].ToString() + ", " + ds.Tables[0].Rows[0]["empname"].ToString() + " ["+ds.Tables[0].Rows[0]["empid"].ToString() + "], Att. Id: " + ds.Tables[0].Rows[0]["attendanceid"].ToString();
            ArrayList arlSkip = new ArrayList();
            arlSkip.Add("recid");
            arlSkip.Add("newempid");
            arlSkip.Add("empid");
            arlSkip.Add("empname");
            arlSkip.Add("attmonth");
            arlSkip.Add("attyear");
            arlSkip.Add("attshift");
            arlSkip.Add("attendanceid");
            arlSkip.Add("modifyuser");
            //rptheader
            string reportheader = "<table border='0' cellpadding='4' cellspacing='0' style='font-size:10pt;' align='center'>";
            reportheader += "<tr>";//line-1
            reportheader += "<td align='left'>";
            string logopath = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
            logopath += "Images/prag-logo.png";
            reportheader += "<img src=" + logopath + " width='60%' height='55%'>";
            reportheader += "</td>";
            reportheader += "<td align='left'>";
            reportheader += "<span style='font-family:Verdana;font-size:14pt;color:black;font-weight:bold;'>PRAG GROUP: Attendance History</span>";
            reportheader += "</td>";
            reportheader += "</tr>";
            reportheader += "<tr>";//line-2
            reportheader += "<td colspan='2' align='center'>";
            reportheader += "<span style='font-family:Verdana;font-size:12pt;color:black;font-weight:bold;'>" + rpthead + "</span>";
            reportheader += "</td>";
            reportheader += "</tr>";
            reportheader += "</table>";//rptheader
            //rptmatter
            string reportmatter = "<table border='1' cellpadding='4' cellspacing='0' style='font-size:10pt;' align='center'>";
            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>S.No.</b></td>";
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                if (arlSkip.Contains(ds.Tables[0].Columns[i].ToString().ToLower()) == false)
                {
                    reportmatter += "<td valign='top'><b>" + ds.Tables[0].Columns[i].ToString() + "<br/></b></td>";
                }
            }
            reportmatter += "</tr>";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                reportmatter += "<tr>";
                reportmatter += "<td valign='top'>" + (i + 1).ToString() + "</td>";//sno
                for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                {
                    if (arlSkip.Contains(ds.Tables[0].Columns[j].ToString().ToLower()) == false)
                    {
                        reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i][j].ToString() + "</td>";
                    }
                }
                reportmatter += "</tr>";
            }
            //totals
            reportmatter += "<tr>";
            reportmatter += "<td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>";//cols 0,1,2
            string n = "";
            int l = 0;
            for (int i = 3; i < 34; i++)//cols D01 To 31
            {
                l = (i - 2).ToString().Length;
                n = l < 2 ? "0" + (i-2).ToString() : (i-2).ToString();
                string cl = "D" + n;
                //
                ArrayList arlX = new ArrayList();
                for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
                {
                    if (arlX.Contains(ds.Tables[0].Rows[k][cl].ToString()) == false)
                    {
                        arlX.Add(ds.Tables[0].Rows[k][cl].ToString());
                    }
                }
                //
                if (arlX.Count > 1)
                {
                    reportmatter += "<td bgcolor='yellow'>" + arlX.Count.ToString() + "</td>";
                }
                else
                {
                    reportmatter += "<td>" + arlX.Count.ToString() + "</td>";
                }
            }
            //cols 34-(+21) to 55
            for (int i = 34; i < 55; i++)
            {
                reportmatter += "<td>&nbsp;</td>";
            }
            reportmatter += "</tr>";//totals
            reportmatter += "</table>";//rptmatter
            //rptfooter
            string reportfooter = "<table border='0' cellpadding='4' cellspacing='0' style='font-size:8pt;' align='center'>";
            reportfooter += "<tr>";
            reportfooter += "<td align='left'>";
            reportfooter += "<span style='font-family:Verdana;color:black'>---End of Report---Run Date: " + mc.getDateTimeString(DateTime.Now) + " [User: " + objCookie.getUserName() + "]</span>";
            reportfooter += "</td>";
            reportfooter += "<td align='right'>";
            reportfooter += "<a href='javascript:window.print();'>Print</a>";
            reportfooter += "</td>";
            reportfooter += "<td align='right'>";
            reportfooter += "<a href='javascript:window.close();'>Close</a>";
            reportfooter += "</td>";
            reportfooter += "</tr>";
            reportheader += "</table>";//rfooter
            return Content("<html><body style='font-family:verdana;font-size:10pt;'>" + reportheader + "<br/>" + reportmatter + "<br/>" + reportfooter + "</body></html>");
        }

        #endregion

        #region absent summary report

        [HttpPost]
        public ActionResult AbsentSummaryReport(rptOptionMdl rptOption)
        {
            //[100069]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Attendance_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Attendance_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "AttendanceReport/getAbsentSummaryReportFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&compcode=" + rptOption.JoiningUnit.ToString() + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAbsentSummaryReportFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, compcode = rptOption.JoiningUnit, grade = rptOption.Grade });
        }

        public ActionResult getAbsentSummaryReportFile(int attmonth, int attyear, int compcode, string grade)
        {
            //[100069]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            if (compcode == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Company not selected!</h1></a>");
            }
            if (grade == null)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Grade not selected!</h1></a>");
            }
            //
            AttendanceReportBLL rptBLL = new AttendanceReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AbsentSummaryRpt.rpt"));
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["tbl_absentletter"].Merge(rptBLL.getAbsentSummaryData(attmonth,attyear,compcode,grade));
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptTitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptTitle"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress"];
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            string gradename = mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename");
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            if (compMdl.CompCode == 6)
            {
                compMdl.CmpName = "PRAG POLYMERS";
            }
            txtRptTitle.Text = "Absent Summary Report - " + monthname + ", " + attyear.ToString() + ".  ["+gradename+"]";
            txtCmpName.Text = compMdl.CmpName;
            txtAddress.Text = compMdl.Address1 + " " + compMdl.Address2 + " " + compMdl.Address3;
            //
            rptDoc.SetDataSource(dsr);
            //rd.Subreports[0].SetDataSource(dsr);
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
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region daily attendance report

        [HttpPost]
        public ActionResult DailyAttendanceReport(rptOptionMdl rptOption)
        {
            //[100068]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Attendance_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Attendance_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "AttendanceReport/getDailyAttendanceReportFile";
                string reportpms = "attday=" + rptOption.AttDay.ToString() + "";
                reportpms += "&attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&shift=" + rptOption.AttShift + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&compcode=" + rptOption.JoiningUnit.ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getDailyAttendanceReportFile", new { attday = rptOption.AttDay, attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, shift = rptOption.AttShift, grade = rptOption.Grade, compcode = rptOption.JoiningUnit });
        }

        public ActionResult getDailyAttendanceReportFile(int attday, int attmonth, int attyear, string shift, string grade, int compcode)
        {
            //[100068]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            if (grade == null)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Grade not selected!</h1></a>");
            }
            if (compcode == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Company not selected!</h1></a>");
            }
            //
            AttendanceReportBLL rptBLL = new AttendanceReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "DailyAttendanceRptA5R.rpt"));//A4
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["tbl_dailyattendancerpt"].Merge(rptBLL.getDailyAbsentReportData(attday,attmonth,attyear,shift,grade,compcode));
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptTitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptTitle"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress"];
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            string gradename = mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename");
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            if (compMdl.CompCode == 6)
            {
                compMdl.CmpName = "PRAG POLYMERS";
            }
            txtRptTitle.Text = "Date: " + attday.ToString() + "-" + monthname + ", " + attyear.ToString()+".  ["+gradename+"]";
            txtCmpName.Text = compMdl.CmpName;
            txtAddress.Text = compMdl.Address1 + " " + compMdl.Address2 + " " + compMdl.Address3;
            //
            rptDoc.SetDataSource(dsr);
            //rd.Subreports[0].SetDataSource(dsr);
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
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region yearly attendance report

        public ActionResult YearlyAttendanceView()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.AttYear = DateTime.Now.Year;
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");

            var listItems = mc.getReportFormatList();
            System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
            li.Value = "DirectXL";
            li.Text = "Direct Excel";
            listItems.Add(li);
            ViewBag.ReportFormatList = new SelectList(listItems, "Value", "Text");

            return View(rptOption);
        }

        [HttpPost]
        public ActionResult DisplayYearlyAttendanceReport(rptOptionMdl rptOption)
        {
            //[100175]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Attendance_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Attendance_Report, permissionType.Edit);
            string methodname = "getYearlyAttendanceReport";
            if (rptOption.ReportFormat == "DirectXL")
            {
                methodname = "getYearlyAttendanceReportCSV";
            }
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "AttendanceReport/" + methodname;
                string reportpms = "attyear=" + rptOption.AttYear + "";
                reportpms += "&ccode=" + rptOption.CompCode + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction(methodname, new { attyear = rptOption.AttYear, ccode = rptOption.CompCode, rptformat = rptOption.ReportFormat });
        }

        public ActionResult getYearlyAttendanceReport(int attyear, int ccode, string rptformat)
        {
            //[100175]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/AttendanceReport/YearlyAttendanceReport.rdlc");
            AttendanceBLL rptBLL = new AttendanceBLL();
            DataSet ds = new DataSet();
            ds = rptBLL.getYearlyAttendanceData(attyear, ccode);
            Reports.dsReport dsr = new Reports.dsReport();
            //
            dsr.Tables["dtRDLC1"].Rows.Clear();
            DataRow dr = dsr.Tables["dtRDLC1"].NewRow();
            //DateTime opdt = new DateTime();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                dr = dsr.Tables["dtRDLC1"].NewRow();
                //opdt = Convert.ToDateTime(ds.Tables[0].Rows[i]["OpeningDate"].ToString());
                dr["Col_1"] = ds.Tables[0].Rows[i]["NewEmpId"].ToString();
                dr["Col_2"] = ds.Tables[0].Rows[i]["EmpId"].ToString();
                dr["Col_3"] = ds.Tables[0].Rows[i]["EmpName"].ToString();
                dr["Col_4"] = ds.Tables[0].Rows[i]["GradeName"].ToString();//mc.getStringByDateDDMMYYYY(opdt);
                dr["Col_5"] = ds.Tables[0].Rows[i]["January"].ToString();
                dr["Col_6"] = ds.Tables[0].Rows[i]["February"].ToString();
                dr["Col_7"] = ds.Tables[0].Rows[i]["March"].ToString();
                dr["Col_8"] = ds.Tables[0].Rows[i]["April"].ToString();
                dr["Col_9"] = ds.Tables[0].Rows[i]["May"].ToString();
                dr["Col_10"] = ds.Tables[0].Rows[i]["June"].ToString();
                dr["Col_11"] = ds.Tables[0].Rows[i]["July"].ToString();
                dr["Col_12"] = ds.Tables[0].Rows[i]["August"].ToString();
                dr["Col_13"] = ds.Tables[0].Rows[i]["September"].ToString();
                dr["Col_14"] = ds.Tables[0].Rows[i]["October"].ToString();
                dr["Col_15"] = ds.Tables[0].Rows[i]["November"].ToString();
                dr["Col_16"] = ds.Tables[0].Rows[i]["December"].ToString();
                dr["Col_17"] = ds.Tables[0].Rows[i]["Total"].ToString();
                dsr.Tables["dtRDLC1"].Rows.Add(dr);
            }
            //
            compBLL = new CompanyBLL();
            CompanyMdl compMdl = compBLL.searchObject(ccode);
            ReportParameter rp1 = new ReportParameter("prCmpName", compMdl.CmpName);
            ReportParameter rp2 = new ReportParameter("prCmpAddress", compMdl.Footer1);
            ReportParameter rp3 = new ReportParameter("prReportName", "Yearly Attendance Report");
            string rptheader = "Attendance Report : " + attyear.ToString();
            ReportParameter rp4 = new ReportParameter("prRptHead", rptheader);
            localReport.SetParameters(new ReportParameter[] { rp1, rp2, rp3, rp4 });
            //
            ReportDataSource reportDataSource = new ReportDataSource("dsReport", dsr.Tables["dtRDLC1"]);
            localReport.DataSources.Add(reportDataSource);
            //
            string filename = "AttendanceReport." + mc.getNameByKey(mc.getReportFormats(), "format", rptformat, "ext");
            string mimeType;
            string encoding;
            string fileNameExtension;
            //page A4 = 297mm X 210mm -- checked dynamically by db-prc
            //string pageWidth = ds.Tables[0].Rows[0]["PW"].ToString();
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>'" + rptformat + "'</OutputFormat>" +
            "  <PageWidth>390mm</PageWidth>" +
            "  <PageHeight>210mm</PageHeight>" +
            "  <MarginTop>0.25in</MarginTop>" +
            "  <MarginLeft>0.45in</MarginLeft>" +
            "  <MarginRight>0.2in</MarginRight>" +
            "  <MarginBottom>0.2in</MarginBottom>" +
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

        public ActionResult getYearlyAttendanceReportCSV(int attyear, int ccode)
        {
            //[100175] --Direct to Excel
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            string rptname = "Yearly Attendance Report: " + attyear.ToString();
            AttendanceBLL rptBLL = new AttendanceBLL();
            DataSet ds = new DataSet();
            ds = rptBLL.getYearlyAttendanceData(attyear, ccode);
            //
            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = ds.Tables[0];
            grid.DataBind();
            //
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + rptname + ".xls");
            Response.ContentType = "application/ms-excel";
            //
            Response.Charset = "";
            //
            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            //
            compBLL = new CompanyBLL();
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(ccode);
            //
            //company name
            htw.AddAttribute("CustomAttribute", "CustomAttributeValue");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "16pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "bold");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.TextAlign, "center");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine(compMdl.CmpName);
            htw.WriteBreak();
            //footer1
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "12pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "bold");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.TextAlign, "center");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine(compMdl.Footer1);
            htw.WriteBreak();
            //report name
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "14pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "bold");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.TextAlign, "center");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine(rptname);
            htw.WriteBreak();
            //
            grid.RenderControl(htw);
            //
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "10pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "regular");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine("---End of Report---[100175], Sorted on: Grade Name + Employee Index Number, Run Date: " + mc.getDateTimeString(DateTime.Now) + ".");
            htw.WriteBreak();
            //
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            //
            return View();
            //
        }

        #endregion

    }
}
