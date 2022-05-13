
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;
using System.Text;
using System.Collections;

namespace ManufacturingManagement_V2.Controllers
{
    public class MACorrectionReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        ERP_V1_ReportBLL rptBll = new ERP_V1_ReportBLL();

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
            ViewBag.POTypeList = new SelectList(mc.getPOTypeRptList(), "Value", "Text","t");
            ViewBag.OrderStatusList = new SelectList(mc.getOrderStatusList(), "Value", "Text","x");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            ViewBag.MAReasonList = new SelectList(mc.getMAReasonList(), "Value", "Text");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getToDate();
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
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            if (rptOption.GroupName == null) { rptOption.GroupId = 0; }
            if (rptOption.ItemCode == null) { rptOption.ItemId = 0; }
            if (rptOption.RailwayName == null) { rptOption.RailwayId = 0; }
            if (rptOption.EmpId == null) { rptOption.EmpId = "0"; }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "MACorrectionReport/GetReportHTML";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&groupid=" + rptOption.GroupId + "";
                reportpms += "&itemid=" + rptOption.ItemId + "";
                reportpms += "&railwayid=" + rptOption.RailwayId + "";
                reportpms += "&orderstatus=" + rptOption.OrderStatus + "";
                reportpms += "&potype=" + rptOption.POType + "";
                reportpms += "&mareason=" + rptOption.EmpId.Trim() + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, groupid = rptOption.GroupId, itemid = rptOption.ItemId, railwayid = rptOption.RailwayId, orderstatus = rptOption.OrderStatus, potype=rptOption.POType, compcode = rptOption.CompCode, mareason= rptOption.EmpId });
        }

        public ActionResult GetReportHTML(DateTime dtfrom, DateTime dtto, int groupid, int itemid, int railwayid, string orderstatus, string potype, int compcode, string mareason="0")
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();

            PurchaseOrderBLL rptBLL = new PurchaseOrderBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.getMACorrectionReportHtml(dtfrom, dtto, groupid, itemid, railwayid, orderstatus, potype, compcode, mareason);

            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            sbHeader.Append("<div style='text-align:center;'>");//div main
            //company
            sbHeader.Append("<div style='font-size:12pt;'>");
            //sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpName"].ToString());
            sbHeader.Append("PRAG GROUP");
            sbHeader.Append("</div>");
            //cmp address
            //sbHeader.Append("<div style='font-size:10pt;'>");
            //sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpFooter1"].ToString());
            //sbHeader.Append("</div>");
            //repoprt name
            sbHeader.Append("<div style='font-size:11pt;background-color:lightgray;'>");
            sbHeader.Append("<b><u>MA Correction Report</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString());
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            ArrayList arlGroup = new ArrayList();
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                if (!arlGroup.Contains(ds.Tables["tbl1"].Rows[i]["cmpname"].ToString()))
                {
                    arlGroup.Add(ds.Tables["tbl1"].Rows[i]["cmpname"].ToString());
                }
            }

            //report content
            int sn = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Case<br/>File&nbsp;No</th>");
            sb.Append("<th style='width:15px;'>Railway</th>");
            sb.Append("<th style='width:15px;'>PONo<br/>Date</th>");
            sb.Append("<th style='width:15px;'>Items-DP</th>");
            sb.Append("<th style='width:15px;'>POStatus</th>");
            sb.Append("<th style='width:15px;'>MA&nbsp;Reason</th>");
            sb.Append("<th style='width:auto;'>Remarks</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int x = 0; x < arlGroup.Count; x++)
            {
                sn = 1;
                sb.Append("<tr class='tblrow'><td colspan='8'>");
                sb.Append("<b><div style='background-color:lightgray;text-align:center;'>" + arlGroup[x].ToString() + "</div></b>");
                sb.Append("</td></tr>");
                //records
                for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
                {
                    if (ds.Tables["tbl1"].Rows[i]["cmpname"].ToString() == arlGroup[x].ToString())
                    {
                        sb.Append("<tr class='tblrow'>");
                        sb.Append("<td>" + sn.ToString() + "</td>");
                        //
                        sb.Append("<td><a target='_new' href='https://pragerp.com:130/Report/DisplayControlledDocument.aspx?strvalue=" + ds.Tables["tbl1"].Rows[i]["porderid"].ToString() + ".pdf?CaseFile?0?0?1'>" + ds.Tables["tbl1"].Rows[i]["casefileno"].ToString() + "</a></td>");
                        //
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["railwayname"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["PONumDate"].ToString() + "</td>");
                        sb.Append("<td style='border-top:none;border-bottom:none;border-left:none;border-right:none;'>");//open
                        sb.Append("<table class='tblcontainer' style='border:none;'>");
                        for (int j = 0; j < ds.Tables["tbl2"].Rows.Count; j++)
                        {
                            if (ds.Tables["tbl2"].Rows[j]["porderid"].ToString() == ds.Tables["tbl1"].Rows[i]["porderid"].ToString())
                            {
                                sb.Append("<tr style='border-top:none;border-bottom:none;border-left:none;border-right:none;'>");
                                sb.Append("<td style='width:150px;border-top:none;border-bottom:1px dotted;border-left:none;border-right:1px dotted;padding:1px;'>" + ds.Tables["tbl2"].Rows[j]["shortname"].ToString() + "</td>");
                                sb.Append("<td style='width:50px;border-top:none;border-bottom:1px dotted;border-left:none;border-right:none;padding:1px;'>" + ds.Tables["tbl2"].Rows[j]["dp"].ToString() + "</td>");
                                sb.Append("</tr>");
                            }
                        }
                        sb.Append("</table>");
                        sb.Append("</td>");//close
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["OrderStatusName"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["MAReason"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["Remarks"].ToString() + "</td>");
                        sb.Append("</tr>");
                        sn += 1;
                    }
                }
            }
            sb.Append("</table><br/>");

            sb.Append("<div>");
            sb.Append("Note: Sorted on: Company + PO, DP + Item Short Name");
            sb.Append("</div>");

            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
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
