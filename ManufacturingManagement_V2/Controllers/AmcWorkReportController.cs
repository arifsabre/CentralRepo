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
    //from dbProcedures/CaseFileClosureRPT_sp.sql
    public class AmcWorkReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        //
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
            //not in use
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult DisplayReport(rptOptionMdl rptOption)
        {
            //not in use
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.AMC_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.AMC_Report, permissionType.Edit);
            //if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            if (rptOption.InvoiceNo == null) { rptOption.NewEmpId = 0; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "AmcWorkReport/GetReportHTML";
                string reportpms = "amcid=" + rptOption.NewEmpId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { amcid = rptOption.NewEmpId });
        }

        [HttpGet]
        public ActionResult DisplayReportLink(int amcid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.AMC_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.AMC_Report, permissionType.Edit);
            //if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "AmcWorkReport/GetReportHTML";
                string reportpms = "amcid=" + amcid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { amcid = amcid });
        }

        public ActionResult GetReportHTML(int amcid)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (amcid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Challan number not selected!</h1></a>");
            }
            setViewData();

            //from dbProcedures/AMC_Work_RPT_SP.sql
            AmcWorkBLL rptBLL = new AmcWorkBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetAmcReportHtml(amcid);
            ReportModelObject.ReportHeader = ds.Tables[0].Rows[0]["rptheader"].ToString();
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(ds.Tables[0].Rows[0]["Result"].ToString());
            ReportModelObject.ReportContent = sb.ToString();

            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

    }
}
