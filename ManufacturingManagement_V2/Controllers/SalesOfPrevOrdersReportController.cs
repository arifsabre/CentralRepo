
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;
using System.Text;

namespace ManufacturingManagement_V2.Controllers
{
    public class SalesOfPrevOrdersReportController : Controller
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
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.POTypeList = new SelectList(mc.getPOTypeRptList(), "Value", "Text");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getToDate();
            rptOption.DateFrom1 = objCookie.getFromDate();
            rptOption.DateTo1 = objCookie.getToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult DisplayReport(rptOptionMdl rptOption)
        {
            //[100038]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (rptOption.GroupName == null) { rptOption.GroupId = 0; };
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalesOfPrvOrdersReport/getSaleOfPrvOrderReport";
                string reportpms = "podtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&podtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&sldtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom1) + "";
                reportpms += "&sldtto=" + mc.getStringByDateForReport(rptOption.DateTo1) + "";
                reportpms += "&potype=" + rptOption.POType + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSaleOfPrvOrderReport", new { podtfrom = rptOption.DateFrom, podtto = rptOption.DateTo, sldtfrom = rptOption.DateFrom1, sldtto = rptOption.DateTo1, potype= rptOption.POType, compcode = rptOption.CompCode });
        }

        //get
        public FileResult getSaleOfPrvOrderReport(DateTime podtfrom, DateTime podtto,DateTime sldtfrom, DateTime sldtto, string potype, int compcode)
        {
            //[100038]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "SalesOfPreviousOrders.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "PO Date From : " + mc.getStringByDate(podtfrom) + " To " + mc.getStringByDate(podtto);
            txtRptHead.Text += ", Sale Date From : " + mc.getStringByDate(sldtfrom) + " To " + mc.getStringByDate(sldtto);
            if (potype != "0")
            {
                txtRptHead.Text += ", PO Type: " + mc.getNameByKey(mc.getPOTypesRpt(), "potype", potype, "potypename");
            }
            //
            rptDoc.SetParameterValue("@podtfrom", podtfrom);
            rptDoc.SetParameterValue("@podtto", podtto);
            rptDoc.SetParameterValue("@sldtfrom", sldtfrom);
            rptDoc.SetParameterValue("@sldtto", sldtto);
            rptDoc.SetParameterValue("@potype", potype);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                //if (rptformat.ToLower() == "pdf")
                //{
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //    //add these lines to download
                //    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //    //return File(stream, "application/pdf", "ReportName.pdf");
                //}
                //else if (rptformat.ToLower() == "excel")
                //{
                //    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                //    stream.Seek(0, System.IO.SeekOrigin.Begin);
                //}
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
            //if (rptformat.ToLower() == "excel")
            //{
            //    return File(stream, "application/excel", "SalaryReportHO.xls");
            //}
            return File(stream, "application/pdf");
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

    }
}
