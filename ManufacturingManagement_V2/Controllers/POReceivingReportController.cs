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
    public class POReceivingReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        ERP_V1_ReportBLL rptBll = new ERP_V1_ReportBLL();
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
            ViewBag.RailwayList = new SelectList(rptBll.getRailwayList(), "railwayid", "railwayname");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult DisplayReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Edit);
            if (rptOption.FgItemCode == null) { rptOption.FgItemId = 0; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "POReceivingReport/GetReportHTML";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&railwayid=" + rptOption.RailwayId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, railwayid = rptOption.RailwayId, compcode = rptOption.CompCode});
        }

        public ActionResult GetReportHTML(DateTime dtfrom, DateTime dtto, int railwayid = 0, int compcode = 0)
        {
            setViewData();
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            //dataset from dbProcedures/A2_All_Reports_SP.sql
            ERP_V1_ReportBLL rptBLL = new ERP_V1_ReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetPurchaseOrderReceivingReportHtml(dtfrom, dtto, railwayid, compcode);

            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            sbHeader.Append("<div style='text-align:center;'>");//div main
            //company
            sbHeader.Append("<div style='font-size:12pt;'>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpName"].ToString());
            sbHeader.Append("</div>");
            //cmp address-n/a
            //repoprt name
            sbHeader.Append("<div style='font-size:11pt;background-color:lightgray;'>");
            sbHeader.Append("<b><u>PO Receipt Report</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString());
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            //report content
            int cnt = 0;
            int cntr = 0;
            string cmpcode = "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Railway</th>");
            sb.Append("<th style='width:135px;'>Case&nbsp;File</th>");
            sb.Append("<th style='width:15px;'>PO&nbsp;Number</th>");
            sb.Append("<th style='width:15px;'>PO&nbsp;Date</th>");
            sb.Append("<th style='width:auto;'>Paying&nbsp;Authority</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                if (cmpcode != ds.Tables["tbl1"].Rows[i]["compcode"].ToString())
                {
                    cnt = 1;
                    cmpcode = ds.Tables["tbl1"].Rows[i]["compcode"].ToString();
                    sb.Append("<tr class='tblrow'>");
                    sb.Append("<td style='font-size:11pt;text-align:center;' colspan='7'><b><u>" + ds.Tables["tbl1"].Rows[i]["CmpName"].ToString() + "</u></b></td>");
                    sb.Append("</tr>");
                }
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + cnt.ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["RailwayName"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["CaseFileNo"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["PONumber"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["PODate"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["PayingAuthName"].ToString() + "</td>");
                sb.Append("</tr>");
                if (ds.Tables["tbl1"].Rows[i]["Remarks"].ToString().Length>0)
                {
                    sb.Append("<tr class='tblrow'>");//tr-remarks
                    sb.Append("<td>&nbsp;</td>");
                    sb.Append("<td colspan='5'>Remarks: " + ds.Tables["tbl1"].Rows[i]["Remarks"].ToString() + "</td>");
                    sb.Append("</tr>");//cl-tr-remarks
                }
                sb.Append("<tr class='tblrow'>");//tr-sub
                sb.Append("<td>&nbsp;</td>");
                sb.Append("<td colspan='5'>");//td-sub
                //sub-rpt
                sb.Append("<table class='tblcontainer' style='width:100%;'>");
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th style='width:15px;'>SlNo</th>");
                sb.Append("<th style='width:auto;'>Item</th>");
                sb.Append("<th style='width:15px;text-align:right;'>Rate</th>");
                sb.Append("<th style='width:15px;text-align:right;'>PO&nbsp;Qty</th>");
                sb.Append("<th style='width:15px;'>Unit</th>");
                sb.Append("<th style='width:15px;'>DP</th>");
                sb.Append("<th style='width:auto;'>Consignee</th>");
                sb.Append("</tr>");
                sb.Append("</thead>");
                cntr = 1;
                for (int x = 0; x < ds.Tables["tbl2"].Rows.Count; x++)
                {
                    if (ds.Tables["tbl1"].Rows[i]["POrderId"].ToString() == ds.Tables["tbl2"].Rows[x]["POrderId"].ToString())
                    {
                        sb.Append("<tr class='tblrow'>");
                        sb.Append("<td>" + cntr.ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl2"].Rows[x]["ShortName"].ToString() + "</td>");
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[x]["Rate"].ToString())) + "</td>");
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[x]["OrdQty"].ToString())) + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl2"].Rows[x]["UnitName"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl2"].Rows[x]["DelvDate"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl2"].Rows[x]["ConsigneeName"].ToString() + "</td>");
                        sb.Append("</tr>");
                        cntr += 1;
                    }
                }
                sb.Append("</table>");//cl-subrpt
                //
                sb.Append("</td>");//cl-td-sub
                sb.Append("</tr>");//cl-tr-sub
                cnt += 1;
            }
            //
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

    }
}
