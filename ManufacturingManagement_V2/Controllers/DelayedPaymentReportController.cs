﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;

namespace ManufacturingManagement_V2.Controllers
{
    public class DelayedPaymentReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
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
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.POTypeList = new SelectList(mc.getPOTypeRptList(), "Value", "Text");
            rptOption.DateFrom = DateTime.Now;
            rptOption.DateTo = DateTime.Now;
            rptOption.VNoFrom = 1;
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }
        
        #region delayed payment report

        [HttpPost]
        public ActionResult DelayedPaymentRpt(rptOptionMdl rptOption)
        {
            //[100002]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Billing_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Billing_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            //if (rptOption.EmpName == null) { rptOption.EmpName = ""; };
            if (rptOption.FgItemCode == null) { rptOption.FgItemId = 0; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "DelayedPaymentReport/getDelayedPaymentReport";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&billpart=" + rptOption.VNoFrom + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&potype=" + rptOption.POType + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getDelayedPaymentReport", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, billpart=rptOption.VNoFrom, compcode = rptOption.CompCode, potype=rptOption.POType});
        }

        [HttpGet]
        public FileResult getDelayedPaymentReport(DateTime dtfrom, DateTime dtto, int billpart, int compcode, string potype)
        {
            //[100002]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "DelayedPaymentRpt.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Invoice Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (potype != "0")
            {
                txtrpthead.Text += ", PO Type: " + mc.getNameByKey(mc.getPOTypesRpt(), "potype", potype, "potypename");
            }
            txtrpthead.Text += ", Bill Part: " + billpart.ToString();
            //dbp parameters
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@billpart", billpart);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@potype", potype);
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
