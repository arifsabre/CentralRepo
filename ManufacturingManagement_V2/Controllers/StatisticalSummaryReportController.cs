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
    public class StatisticalSummaryReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        StatisticalSummaryBLL bllObject = new StatisticalSummaryBLL();
        //
        // GET: /AttendanceReport/

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
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

        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname");
            ViewBag.SectionList = new SelectList(bllObject.getSectionList(), "shortname", "cmpname");
            ViewBag.SegmentList = new SelectList(bllObject.getSegmentList(""), "shortname", "cmpname");
            rptOption.ReportDate = DateTime.Now;
            return View(rptOption);
        }

        public ActionResult IndexView(rptOptionMdl rptOpt=null)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if(mc.getPermission(Entry.StatisticalSummary_Report, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //if called directly
            if (rptOpt.ReportDate.Day == 1 && rptOpt.ReportDate.Month == 1 && rptOpt.ReportDate.Year == 1)
            {
                rptOpt.CompCode = 0;
                rptOpt.ReportDate = DateTime.Now;
            }
            StatisticalSummaryMdl modelObject = new StatisticalSummaryMdl();
            modelObject = bllObject.getStatisticalSummaryData(rptOpt.ReportDate, rptOpt.CompCode);
            return View(modelObject);
        }

        #region to print

        [HttpPost]
        public ActionResult StatisticalReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.StatisticalSummary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.StatisticalSummary_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "WorkListReport/getComplianceReport";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&iscompleted=" + rptOption.Detailed + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&depid=" + rptOption.RailwayId + "";
                reportpms += "&taskopt=" + rptOption.ItemId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getComplianceReport", new { iscompleted = rptOption.Detailed, depid = rptOption.RailwayId, taskopt = rptOption.ItemId, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode });
        }

        [HttpGet]
        public FileResult getStatisticalReport(bool iscompleted, int depid, int taskopt, DateTime dtfrom, DateTime dtto, int compcode)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ComplianceRpt.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            string comp = iscompleted == true ? "Completed" : "Pending";
            txtrpthead.Text += "\r\nStatus: " + comp;
            //dbp parameters   --usp_get_tbl_worklist
            rptDoc.SetParameterValue("@IsCompleted", iscompleted);
            rptDoc.SetParameterValue("@depid", depid);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@TaskOpt", taskopt);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@userid", Convert.ToInt32(objCookie.getUserId()));
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
                //add these lines to download
                //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //return File(stream, "application/pdf", "ReportName.pdf");
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
            return File(stream, "application/pdf");
        }

        #endregion

    }
}
