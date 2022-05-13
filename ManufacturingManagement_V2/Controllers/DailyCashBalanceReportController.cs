using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.Data;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class DailyCashBalanceReportController : Controller
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

        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region daily cash balance report

        [HttpPost]
        public ActionResult DisplayDailyCashBalance(rptOptionMdl rptOption)
        {
            //[100078]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Account_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Account_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "DailyCashBalanceReport/GetReportHTML";
                string reportpms = "compcode=" + rptOption.CompCode.ToString() + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                //reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { compcode = rptOption.CompCode, finyear = rptOption.FinYear });
        }

        public ActionResult GetReportHTML(int compcode, string finyear)
        {
            setViewData();
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            AccountReportsBLL rptBLL = new AccountReportsBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetDailyCashBalanceReportHtml(compcode, finyear);

            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            sbHeader.Append("<div style='text-align:center;'>");//div main
            //company
            sbHeader.Append("<div style='font-size:12pt;'>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpName"].ToString());
            sbHeader.Append("</div>");
            //cmp address
            sbHeader.Append("<div style='font-size:10pt;'>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpFooter1"].ToString());
            sbHeader.Append("</div>");
            //repoprt name
            sbHeader.Append("<div style='font-size:11pt;background-color:lightgray;'>");
            sbHeader.Append("<b><u>Daily Cash Balance Report</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString());
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            //report content
            double balance = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>"); sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:auto;border-right:1px dotted;'>Date</th>");
            sb.Append("<th style='width:15px;text-align:right;border-left:1px dotted;'>Amount</th>");
            sb.Append("<th style='width:auto;border-right:1px dotted;'>Date</th>");
            sb.Append("<th style='width:15px;text-align:right;border-left:1px dotted;'>Amount</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                //c1
                balance = Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["balance"].ToString());
                sb.Append("<td style='border-right:1px dotted;'>" + ds.Tables["tbl1"].Rows[i]["vdate"].ToString() + "</td>");
                sb.Append("<td style='border-left:1px dotted;'>" + mc.getINRCFormat(balance) + "</td>");
                //c2
                if (i + 1 < ds.Tables["tbl1"].Rows.Count)
                {
                    balance = Convert.ToDouble(ds.Tables["tbl1"].Rows[i + 1]["balance"].ToString());
                    sb.Append("<td style='border-right:1px dotted;'>" + ds.Tables["tbl1"].Rows[i + 1]["vdate"].ToString() + "</td>");
                    sb.Append("<td style='border-left:1px dotted;'>" + mc.getINRCFormat(balance) + "</td>");
                }
                else
                {
                    sb.Append("<td style='border-right:1px dotted;'>&nbsp;</td>");
                    sb.Append("<td style='border-left:1px dotted;'>&nbsp;</td>");
                }
                i += 1;
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        public ActionResult getDailyCashBalanceReport(int compcode, string finyear)
        {
            //[100078]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            DailyCashBalanceRptBLL rptBLL = new DailyCashBalanceRptBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            Reports.dsReport dsr = new Reports.dsReport();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "DailyCashBalance.rpt"));
            rptBLL.prepareDailyCashBalanceReport(dsr.Tables["dtCashBalance"], compcode, finyear);
            //
            CompanyMdl compmdl = new CompanyMdl();
            compmdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptHead"];
            txtCmpName.Text = compmdl.CmpName;
            txtAddress1.Text = compmdl.Footer1;
            txtRptHead.Text = "Daily Cash Balance, Financial Year: " + finyear;
            setLoginInfo(rptDoc);
            rptDoc.SetDataSource(dsr);
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
