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
    public class CaseFileClosureReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        AttendanceBLL attBLL = new AttendanceBLL();
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
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult DisplayClosureReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Edit);
            //if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            if (rptOption.InvoiceNo == null) { rptOption.NewEmpId = 0; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "CaseFileClosureReport/ClosureReportHTML";
                string reportpms = "porderid=" + rptOption.NewEmpId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("ClosureReportHTML", new { porderid = rptOption.NewEmpId });
        }

        [HttpGet]
        public ActionResult ClosureReportLink(int porderid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Edit);
            //if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "CaseFileClosureReport/ClosureReportHTML";
                string reportpms = "porderid=" + porderid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("ClosureReportHTML", new { porderid = porderid });
        }

        public ActionResult ClosureReportHTML(int porderid)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (porderid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Purchase number not selected!</h1></a>");
            }
            setViewData();
            CaseFileClosureReportBLL rptBLL = new CaseFileClosureReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            ReportModelObject.ReportHeader = rptBLL.getRptHeader(porderid).Tables[0].Rows[0]["rptheader"].ToString();
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<b><u> PO Detail </u></b>");
            sb.Append(rptBLL.getPODetail(porderid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Dispatch Detail </u></b>");
            sb.Append(rptBLL.getPODispatchDetail(porderid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Payment Detail </u></b>");
            sb.Append(rptBLL.getPOPaymentDetail(porderid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> BG Detail </u></b>");
            sb.Append(rptBLL.getPOBGDetail(porderid).Tables[0].Rows[0]["Result"].ToString());

            ReportModelObject.ReportContent = sb.ToString();

            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

    }
}
