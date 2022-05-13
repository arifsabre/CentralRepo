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
    public class SalaryReportArrearController : Controller
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
            //rptOption.AttYear = DateTime.Now.Year;
            //rptOption.AttMonth = DateTime.Now.Month;
            //rptOption.AttShift = "d";
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            //ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            //ViewBag.MonthList = new SelectList(mc.getMonthList(), "Value", "Text");
            //ViewBag.ShiftList = new SelectList(mc.getShiftList(), "Value", "Text");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
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

        #region form 12 report Arrear

        [HttpPost]
        public ActionResult Form12ReportArrear(rptOptionMdl rptOption)
        {
            //[ArrearReport_SP]/P1/P2
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

            //note: static preparation
            //for arrearid = 1 -VDA
            rptOption.AttMonth = 8;
            rptOption.AttYear = 2018;
            rptOption.Grade = "w";
            rptOption.AttShift = "d";
            //

            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getForm12ArrearReportFile";
                string reportpms = "attmonth=" + rptOption.AttMonth.ToString() + "";
                reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
                reportpms += "&attshift=" + rptOption.AttShift + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getForm12ArrearReportFile", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, attshift = rptOption.AttShift, compcode = rptOption.CompCode, grade = rptOption.Grade, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public ActionResult getForm12ArrearReportFile(int attmonth, int attyear, string attshift, int compcode, string grade, string rptformat)
        {
            //[ArrearReport_SP]/P1/P2/NCRPT
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            SalaryReportArrearBLL rptBLL = new SalaryReportArrearBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            if (grade.ToLower() == "w")
            {
                //copied from Form12Rpt_Worker_Arrear [no change made in copied crpt]
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Form12Rpt_Worker_Arrear.rpt"));
            }
            else
            {
                //rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Form12Rpt_Staff.rpt"));
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Not Applicable!</h1></a>");
            }
            //
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            System.Data.DataTable dtratt = new DataTable();
            System.Data.DataTable dtrsalary = new DataTable();
            dtratt = rptBLL.getForm12ArrearReportData(attmonth, attyear, attshift, compcode, grade);
            dsr.Tables["tbl_attendance"].Merge(dtratt);
            dtrsalary = rptBLL.getArrearReportData(attmonth, attyear, attshift, compcode, grade, "");
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
            txtRptInfo.Text = "Arrear for VDA Increment: " + compMdl.ShortName + " -" + txtmonth.Text + ", " + txtyear.Text;
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
            if (compMdl.CompCode == 6 || compMdl.CompCode == 7)//Polymers(Mfg) or PR-U2
            {
                txtC1R1.Text = "06:00 AM";
                txtC1R2.Text = "";
                txtC1R3.Text = "";
                txtC1R4.Text = "";
                txtC2R1.Text = "10:00 AM";
                txtC2R2.Text = "";
                txtC2R3.Text = "";
                txtC2R4.Text = "";
                txtC3R1.Text = "10:30 AM";
                txtC3R2.Text = "";
                txtC3R3.Text = "";
                txtC3R4.Text = "";
                txtC4R1.Text = "02:30 PM";
                txtC4R2.Text = "";
                txtC4R3.Text = "";
                txtC4R4.Text = "";
            }
            else
            {
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
            }
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

    }
}
