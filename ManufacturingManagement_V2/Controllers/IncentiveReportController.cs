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
    public class IncentiveReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private CompanyBLL compBLL = new CompanyBLL();

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
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            ViewBag.GradeList = new SelectList(EmployeeBLL.Instance.getEmployeeGradeList(), "ObjectCode", "ObjectName");
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.AttYear = DateTime.Now.Year;
            return View(rptOption);
        }

        #region incentive report Quarterly

        [HttpPost]
        public ActionResult IncentiveQuarterly(rptOptionMdl rptOption)
        {
            //[100102]
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
                string reporturl = "IncentiveReport/getIncentiveQuarterlyReportFile";
                string reportpms = "&compcode=" + rptOption.CompCode + "";
                reportpms += "&qtr=" + rptOption.AttMonth + "";
                reportpms += "&attyear=" + rptOption.AttYear + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                reportpms += "&detailed=" + rptOption.Detailed + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getIncentiveQuarterlyReportFile", new { compcode = rptOption.CompCode, qtr = rptOption.AttMonth, attyear = rptOption.AttYear, rptformat = rptOption.ReportFormat, detailed=rptOption.Detailed, grade = rptOption.Grade });
        }

        public ActionResult getIncentiveQuarterlyReportFile(int compcode, int qtr, int attyear, string rptformat = "pdf", bool detailed=false, string grade="w")
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            if (detailed == true)
            {
                //[100102]/F2
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "IncentiveReportBankDetail.rpt"));
            }
            else
            {
                //[100102]/F1
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "IncentiveReportQuarterly.rpt"));
            }
            setLoginInfo(rptDoc);
            
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Year: " + attyear.ToString()+", Qtr: " + qtr.ToString();
            string gradename = "";
            if (grade.ToLower() == "d" || grade.ToLower() == "m" || grade.ToLower() == "s")
            {
                gradename = "Director, Manager and Staff";
            }
            else
            {
                gradename = mc.getNameByKey(EmployeeBLL.Instance.getEmployeeGradeData(), "gradecode", grade, "gradename");
            }
            txtRptHead.Text += ", Grade: " + gradename;

            //dbp parameters --usp_get_quarterly_incentive_report
            rptDoc.SetParameterValue("@qtr", qtr);
            rptDoc.SetParameterValue("@attyear", attyear);
            rptDoc.SetParameterValue("@joiningunit", compcode);
            rptDoc.SetParameterValue("@grade", grade);
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

        #region incentive report Hourly

        public ActionResult IndexHourly()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.MonthList = new SelectList(mc.getMonthList(), "Value", "Text");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            ViewBag.GradeList = new SelectList(EmployeeBLL.Instance.getEmployeeGradeList(), "ObjectCode", "ObjectName");
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.AttYear = DateTime.Now.Year;
            DateTime dtx = DateTime.Now.AddMonths(-1);
            rptOption.AttMonth = dtx.Month;
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult IncentiveHourly(rptOptionMdl rptOption)
        {
            //[100153]
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
                string reporturl = "IncentiveReport/getIncentiveHourlyReportFile";
                string reportpms = "&compcode=" + rptOption.CompCode + "";
                reportpms += "&month=" + rptOption.AttMonth + "";
                reportpms += "&attyear=" + rptOption.AttYear + "";
                reportpms += "&atthrs=" + rptOption.Option + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getIncentiveHourlyReportFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, compcode = rptOption.CompCode, atthrs = rptOption.Option, rptformat = rptOption.ReportFormat, grade=rptOption.Grade });
        }

        public ActionResult getIncentiveHourlyReportFile(int attmonth, int attyear, int compcode, double atthrs ,string rptformat = "pdf", string grade="w")
        {
            //[100153]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "IncentiveReportHourly.rpt"));
            setLoginInfo(rptDoc);

            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Month: " + monthname + " - " + attyear.ToString() + ", Hours >= " + atthrs.ToString();
            string gradename = "";
            if (grade.ToLower() == "d" || grade.ToLower() == "m" || grade.ToLower() == "s")
            {
                gradename = "Director, Manager and Staff";
            }
            else
            {
                gradename = mc.getNameByKey(EmployeeBLL.Instance.getEmployeeGradeData(), "gradecode", grade, "gradename");
            }
            txtRptHead.Text += ", Grade: " + gradename;

            //dbp parameters --usp_get_incentive_report_hourly
            rptDoc.SetParameterValue("@attmonth", attmonth);
            rptDoc.SetParameterValue("@attyear", attyear);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@atthrs", atthrs);
            rptDoc.SetParameterValue("@grade", grade);
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
                return File(stream, "application/excel", "HourlyIncentive.xls");
            }
            return File(stream, "application/pdf");

        }

        #endregion

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

    }
}
