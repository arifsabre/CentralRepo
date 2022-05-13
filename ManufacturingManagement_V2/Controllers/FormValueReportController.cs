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
    public class FormValueReportController : Controller
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
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            rptOptionMdl rptOption = new rptOptionMdl();
            //rptOption.DateFrom = objCookie.getFromDate();
            //rptOption.DateTo = objCookie.getToDate();
            return View(rptOption);
        }

        #region form A report
        //
        [HttpPost]
        public ActionResult FormAReport(rptOptionMdl rptOption)
        {
            //[100097]/A
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "FormValueReport/getFormAReportFile";
                string reportpms = "ccode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getFormAReportFile", new { ccode = rptOption.CompCode, finyear = rptOption.FinYear });
        }

        public ActionResult getFormAReportFile(int ccode, string finyear)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //[100097]/A/NCRPT
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "FormARpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrptDocerFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            rptDoc.RecordSelectionFormula = "{vw_formvalue.compcode}=" + ccode + " and {vw_formvalue.finyear}='" + finyear + "'";
            //CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtCmpName"];
            //txtCmpName.Text = objCookie.getCmpName();
            //rptDoc.Subreports[0].SetDataSource(ds);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //
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
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion form A report
        //

        #region form B report
        //
        [HttpPost]
        public ActionResult FormBReport(rptOptionMdl rptOption)
        {
            //[100097]/B
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "FormValueReport/getFormBReportFile";
                string reportpms = "ccode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getFormBReportFile", new { ccode = rptOption.CompCode, finyear = rptOption.FinYear });
        }

        public ActionResult getFormBReportFile(int ccode, string finyear)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //[100097]/A/NCRPT
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "FormBRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrptDocerFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            rptDoc.RecordSelectionFormula = "{vw_formvalue.compcode}=" + ccode + " and {vw_formvalue.finyear}='" + finyear + "'";
            //CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtCmpName"];
            //txtCmpName.Text = objCookie.getCmpName();
            //rptDoc.Subreports[0].SetDataSource(ds);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //
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
        //

        #region bonus worksheet report

        [HttpPost]
        public ActionResult BonusWorkingsheetReport(rptOptionMdl rptOption)
        {
            //[100084]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "FormValueReport/getBonusWorkingsheetReportFile";
                string reportpms = "finyear=" + rptOption.FinYear + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getBonusWorkingsheetReportFile", new { finyear = rptOption.FinYear, compcode = rptOption.CompCode, grade = rptOption.Grade });
        }

        [HttpGet]
        public FileResult getBonusWorkingsheetReportFile(string finyear, int compcode, string grade)
        {
            //[100084]
            //--usp_get_bonus_worksheet_report
            if (mc.isValidToDisplayReport() == false)//note
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "BonusWorksheetRpt.rpt"));
            setLoginInfo(rptDoc);
            string gradename = mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename");
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptTitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptTitle"];
            txtRptTitle.Text = "Bonus Worksheet For: " + finyear.ToString() + ", Grade: " + gradename;
            //dbp parameters -- usp_get_bonus_worksheet_report
            rptDoc.SetParameterValue("@finyear", finyear);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@grade", grade);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //
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

        #endregion

        #region form C
        //

        [HttpPost]
        public ActionResult FormCReport(rptOptionMdl rptOption)
        {
            //[100098]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "FormValueReport/getBonusReportFormCFile";
                string reportpms = "ccode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getBonusReportFormCFile", new { ccode = rptOption.CompCode, finyear = rptOption.FinYear, grade = rptOption.Grade });
        }

        public ActionResult getBonusReportFormCFile(int ccode, string finyear,string grade)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            SalaryReportBLL rptBLL = new SalaryReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //[100098]/NCRPT
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "FormCRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["tbl_bonusrpt"].Merge(rptBLL.getBonusReportFormCData(ccode, finyear, grade));
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtFinYear = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtFinYear"];
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(ccode);
            string gradename = mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename");
            txtCmpName.Text = compMdl.CmpName;
            txtFinYear.Text = "Bonus paid to employees of the accounting year Ending on the: " + finyear;
            //
            rptDoc.SetDataSource(dsr);
            //rptDoc.Subreports[0].SetDataSource(dsr);
            //
            double wdys = rptBLL.getCompanyWorkingDaysFinYear(ccode,finyear);
            rptDoc.SetParameterValue("@CmpWDays", wdys);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecorptDocSelectionFormula = "";
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

        #region form-C with bank detail

        [HttpPost]
        public ActionResult FormCBankDetail(rptOptionMdl rptOption)
        {
            //[100099]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "FormValueReport/getFormCBankDetail";
                string reportpms = "grade=" + rptOption.Grade + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getFormCBankDetail", new { grade = rptOption.Grade, compcode = rptOption.CompCode, finyear = rptOption.FinYear, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public FileResult getFormCBankDetail(string grade, int compcode, string finyear, string rptformat)
        {
            //[100099]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "FormC_BankDetail.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string gradename = mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename");
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Financial Year " + finyear + ", Grade: " + gradename;
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters--  usp_get_bonus_report_form_c_with_bank_detail
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@finyear", finyear);
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
                return File(stream, "application/excel", "FormCBankDetail.xls");
            }
            return File(stream, "application/pdf");
        }

        #endregion

        #region form-C NEFT report

        [HttpPost]
        public ActionResult FormCNeftReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "FormValueReport/getFormCNeftReport";
                string reportpms = "grade=" + rptOption.Grade + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getFormCNeftReport", new { grade = rptOption.Grade, compcode = rptOption.CompCode, finyear = rptOption.FinYear, rptformat = rptOption.ReportFormat });
        }

        [HttpGet]
        public FileResult getFormCNeftReport(string grade, int compcode, string finyear, string rptformat)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/BonusRPT/"), "BonusNeftReport.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string gradename = mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename");
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtFooter1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtFooter1"];
            txtCmpName.Text = compMdl.CmpName;
            txtFooter1.Text = compMdl.Footer1;
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Financial Year " + finyear + ", Grade: " + gradename;
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters--  usp_get_bonus_report_form_c_with_bank_detail
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@finyear", finyear);
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
                return File(stream, "application/excel", "BonuNEFT.xls");
            }
            return File(stream, "application/pdf");
        }

        #endregion

        #region form D

        [HttpPost]
        public ActionResult FormDReport(rptOptionMdl rptOption)
        {
            //[100097]/D
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "FormValueReport/getFormDReport";
                string reportpms = "compcode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getFormDReport", new { compcode = rptOption.CompCode, finyear = rptOption.FinYear });
        }

        public ActionResult getFormDReport(int compcode, string finyear)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //[100097]/D
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "FormDRpt.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula = "{usp_get_lic_worksheet_report;.age} <= 58";
            //CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            //txtrpthead.Text = "LIC Worksheet as on Date: " + mc.getStringByDate(vdate);
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@finyear", finyear);
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
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
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
