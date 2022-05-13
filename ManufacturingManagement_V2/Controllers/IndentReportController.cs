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
    public class IndentReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
        }

        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname",objCookie.getCompCode());
            //ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            rptOptionMdl rptOption = new rptOptionMdl();
            //rptOption.AttYear = DateTime.Now.Year;
            //rptOption.DateFrom = DateTime.Now;
            //rptOption.DateTo = DateTime.Now;
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
        //

        #region indent issue slip

        [HttpGet]
        public ActionResult IndentIssueSlip(int stkrecid = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (stkrecid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>No Item(s) Issued!</h1></a>");
            }
            bool viewper = mc.getPermission(Entry.IndentReport, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.IndentReport, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "IndentReport/getIndentIssueSlipReport";
                string reportpms = "stkrecid=" + stkrecid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getIndentIssueSlipReport", new { stkrecid = stkrecid });
        }

        public ActionResult getIndentIssueSlipReport(int stkrecid = 0)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CompanyMdl cmpmdl = new CompanyMdl();
            cmpmdl = compBLL.searchObject(Convert.ToInt32(objCookie.getCompCode()));
            if (cmpmdl.CompCode == 6)
            {
                cmpmdl.CmpName = "PRAG POLYMERS";
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //ReportDocument rptDocSub = new ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "IndentIssueSlip.rpt"));
            //rptDocSub.Load(Server.MapPath("../RptCrystal/SaleInvoiceValuesF.rpt"));
            setLoginInfo(rptDoc);
            rptDoc.RecordSelectionFormula = "{vw_indent_issue_slip_report.stkrecid}=" + stkrecid + "";
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtCmpName"];
            txtCmpName.Text = objCookie.getCmpName();
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress"];
            txtAddress.Text = cmpmdl.Footer1;
            //rptDoc.Subreports[0].SetDataSource(ds);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecordSelectionFormula = "";
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

        #endregion end-indent issue slip

        #region indent report

        [HttpGet]
        public ActionResult IndentReport()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getToDate();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname",objCookie.getCompCode());
            ViewBag.IndentStatusList = new SelectList(mc.getIndentStatusList(), "Value", "Text");
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult IndentReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.IndentReport, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.IndentReport, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "IndentReport/getIndentReportFile";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&indentstatus=" + rptOption.Status + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&indentid=" + rptOption.VNoFrom + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getIndentReportFile", new { dtfrom = rptOption.DateFrom, dtto=rptOption.DateTo, indentstatus=rptOption.Status, compcode=rptOption.CompCode, indentid=rptOption.VNoFrom });
        }

        [HttpGet]
        public ActionResult IndentReportById(int indentid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.IndentReport, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (indentid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid Attempt!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.IndentReport, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "IndentReport/getIndentReportFile";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(objCookie.getFromDate()) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(objCookie.getToDate()) + "";
                reportpms += "&indentstatus='0'";
                reportpms += "&compcode=" + objCookie.getCompCode() + "";
                reportpms += "&indentid=" + indentid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getIndentReportFile", new { dtfrom = objCookie.getFromDate(), dtto = objCookie.getToDate(), indentstatus = "0", compcode = objCookie.getCompCode(), indentid = indentid });
        }

        public ActionResult getIndentReportFile(DateTime dtfrom, DateTime dtto, string indentstatus, int compcode,int indentid=0)
        {
            //by usp_get_indent_report
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "IndentReport.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula = "{vw_order_report.orderno}=" + orderno + " and {vw_order_report.compcode}=" + objCookie.getCompCode() + " and {vw_order_report.finyear}='" + objCookie.getFinYear() + "'";
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpttitle"];
            txtrpttitle.Text = "Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (indentstatus == null) { indentstatus = "0"; };
            if(indentstatus != "0")
            {
                txtrpttitle.Text += ", Status: " + mc.getNameByKey(mc.getIndentStatus(), "indentstatus", indentstatus, "indentstatusname");
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters for --usp_get_indent_report
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@indentstatus", indentstatus);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@indentid", indentid);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecordSelectionFormula = "";
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

        #region pending indents to purchase by po //not in use

        [HttpPost]
        public ActionResult PendingToPurchaseByPO()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.IndentReport, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.IndentReport, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "IndentReport/getPendingToPurchaseByPOFile";
                string reportpms = "1=1";
                //string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                //reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                //reportpms += "&indentstatus=" + rptOption.Status + "";
                //reportpms += "&compcode=" + rptOption.CompCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getPendingToPurchaseByPOFile");
        }

        public ActionResult getPendingToPurchaseByPOFile()
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //usp_indents_pending_to_purchase_by_order
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "IndentsToPurchaseByPO.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula = "{vw_order_report.orderno}=" + orderno + " and {vw_order_report.compcode}=" + objCookie.getCompCode() + " and {vw_order_report.finyear}='" + objCookie.getFinYear() + "'";
            //CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpttitle"];
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(objCookie.getCompCode()));
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecordSelectionFormula = "";
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

        #region indent help

        //get
        public ActionResult IndentHelp()
        {
            Session["xsid"] = objCookie.getUserId();
            string reporturl = "IndentReport/getIndentHelpFile";
            string reportpms = "";
            return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
        }

        //get
        public FileResult getIndentHelpFile()
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            string fileName = "IndentHelp.pdf";
            string path = Server.MapPath("~/App_Data/FileTrf/");
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            return File(path + fileName, mc.getMimeType(fileName));
        }

        #endregion indent help

    }
}
