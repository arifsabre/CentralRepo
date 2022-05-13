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
    public class BalanceSheetReportController : Controller
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
            rptOption.ReportDate = objCookie.getDispToDate();
            return View(rptOption);
        }

        #region balance sheet

        [HttpPost]
        public ActionResult DisplayBalanceSheet(rptOptionMdl rptOption)
        {
            //[100079]
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
                string reporturl = "BalanceSheetReport/GetReportHTML";
                string reportpms = "compcode=" + rptOption.CompCode.ToString() + "";
                reportpms += "&vdate=" + mc.getStringByDateForReport(rptOption.ReportDate) + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { compcode = rptOption.CompCode, finyear = rptOption.FinYear, vdate = rptOption.ReportDate });
        }

        public ActionResult GetReportHTML(int compcode, string finyear, DateTime vdate)
        {
            setViewData();
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (mc.isValidDateForFinYear(finyear, vdate) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid to date for selected financial year!</h1></a>");
            }

            AccountReportsBLL rptBLL = new AccountReportsBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetBalanceSheetReportHtml(compcode, finyear, vdate);

            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            sbHeader.Append("<div style='text-align:center;'>");//div main
            //temp note
            sbHeader.Append("<div style='font-size:9pt;color:red;'>* To be revised for display</div>");
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
            sbHeader.Append("<b><u>Balance Sheet</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString());
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            //report content
            double dramt = 0;
            double cramt = 0;
            double tldramt = 0;
            double tlcramt = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:225px;'>Liability</th>");
            sb.Append("<th style='width:150px;text-align:right;'>Amount(CR)</th>");//cramt
            sb.Append("<th style='width:225;'>Asset</th>");
            sb.Append("<th style='width:150px;text-align:right;'>Amount(DR)</th>");//dramt
            sb.Append("</tr>");
            sb.Append("</thead>");
            int cnt = ds.Tables["tbl1"].Rows.Count;
            if (ds.Tables["tbl1"].Rows.Count >= ds.Tables["tbl2"].Rows.Count)
            {
                cnt = ds.Tables["tbl2"].Rows.Count;
            }
            for (int i = 0; i < cnt; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["crdesc"].ToString() + "</td>");
                cramt = Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["cramt"].ToString());
                sb.Append("<td align='right'>" + mc.getINRCFormat(cramt) + "</td>");
                sb.Append("<td>" + ds.Tables["tbl2"].Rows[i]["drdesc"].ToString() + "</td>");
                dramt = Convert.ToDouble(ds.Tables["tbl2"].Rows[i]["dramt"].ToString());
                sb.Append("<td align='right'>" + mc.getINRCFormat(dramt) + "</td>");
                sb.Append("</tr>");
                tlcramt += cramt;
                tldramt += dramt;
            }
            if (ds.Tables["tbl1"].Rows.Count > ds.Tables["tbl2"].Rows.Count)
            {
                for (int i = cnt; i < ds.Tables["tbl1"].Rows.Count; i++)
                {
                    sb.Append("<tr class='tblrow'>");
                    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["crdesc"].ToString() + "</td>");
                    cramt = Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["cramt"].ToString());
                    sb.Append("<td align='right'>" + mc.getINRCFormat(cramt) + "</td>");
                    sb.Append("<td>&nbsp;</td>");
                    sb.Append("<td align='right'>&nbsp;</td>");
                    sb.Append("</tr>");
                    tlcramt += cramt;
                }
            }
            if (ds.Tables["tbl2"].Rows.Count > ds.Tables["tbl1"].Rows.Count)
            {
                for (int i = cnt; i < ds.Tables["tbl2"].Rows.Count; i++)
                {
                    sb.Append("<tr class='tblrow'>");
                    sb.Append("<td>&nbsp;</td>");
                    sb.Append("<td align='right'>&nbsp;</td>");
                    sb.Append("<td>" + ds.Tables["tbl2"].Rows[i]["drdesc"].ToString() + "</td>");
                    dramt = Convert.ToDouble(ds.Tables["tbl2"].Rows[i]["dramt"].ToString());
                    sb.Append("<td align='right'>" + mc.getINRCFormat(dramt) + "</td>");
                    sb.Append("</tr>");
                    tldramt += dramt;
                }
            }
            //
            sb.Append("<tr class='tblrow'>");
            sb.Append("<td style='width:auto;'><b>Total</b></td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(tlcramt) + "</b></td>");
            sb.Append("<td style='width:auto;'><b>Total</b></td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(tldramt) + "</b></td>");
            sb.Append("</tr>");
            //
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        public ActionResult getDisplayBalanceSheet(int compcode, DateTime vdate, string finyear="")
        {
            //[100079]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            if (mc.isValidDateForFinYear(finyear, vdate) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid date for selected financial year!</h1></a>");
            }
            //
            BalanceSheetRptBLL rptBLL = new BalanceSheetRptBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            Reports.dsReport dsr = new Reports.dsReport();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "BalanceSheet.rpt"));
            rptBLL.prepareBalanceSheet(vdate, dsr.Tables["dtTrialBalance"], dsr.Tables["dtDebit"], dsr.Tables["dtCredit"], dsr.Tables["dtBlSheet"], compcode, finyear);
            //
            CompanyMdl compmdl = new CompanyMdl();
            compmdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptHead"];
            txtCmpName.Text = compmdl.CmpName;
            txtAddress1.Text = compmdl.Footer1;
            txtRptHead.Text = "Balance Sheet as on Date: " + mc.getStringByDate(vdate)+", Financial Year: "+finyear;
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
