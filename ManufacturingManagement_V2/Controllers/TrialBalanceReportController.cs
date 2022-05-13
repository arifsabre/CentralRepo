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
    public class TrialBalanceReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        TrialBalanceRptBLL objBLL = new TrialBalanceRptBLL();

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
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region opening trail report

        [HttpPost]
        public ActionResult DisplayTrialBalance(rptOptionMdl rptOption)
        {
            //[100072]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Account_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Account_Report, permissionType.Edit);
            if (rptOption.ItemCode == null) { rptOption.ItemId = 0; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "TrailBalalnceReport/GetReportHTML";
                string reportpms = "compcode=" + rptOption.CompCode.ToString() + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&grcode=" + rptOption.ItemId + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { compcode = rptOption.CompCode, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, grcode = rptOption.ItemId, finyear = rptOption.FinYear });
        }

        public ActionResult GetReportHTML(int compcode, DateTime dtfrom, DateTime dtto, int grcode, string finyear)
        {
            setViewData();
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (mc.isValidDateForFinYear(finyear, dtfrom) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid from date for selected financial year!</h1></a>");
            }
            if (mc.isValidDateForFinYear(finyear, dtto) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid to date for selected financial year!</h1></a>");
            }
            
            AccountReportsBLL rptBLL = new AccountReportsBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetTrialBalanceReportHtml(compcode, finyear, dtfrom, dtto, grcode);

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
            sbHeader.Append("<b><u>Trial Balance Report</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString());
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            //report content
            double opdramt = 0;
            double opcramt = 0;
            double txdramt = 0;
            double txcramt = 0;
            double cldramt = 0;
            double clcramt = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;vertical-align:middle;' rowspan='2'>SlNo</th>");
            sb.Append("<th style='width:auto;vertical-align:middle;' rowspan='2'>Particulars</th>");
            sb.Append("<th style='width:15px;text-align:center;' colspan='2'>Opening</th>");
            sb.Append("<th style='width:15px;text-align:center;' colspan='2'>Transaction</th>");
            sb.Append("<th style='width:15px;text-align:center;' colspan='2'>Closing</th>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;text-align:right;'>Debit&nbsp;Amount</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Credit&nbsp;Amount</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Debit&nbsp;Amount</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Credit&nbsp;Amount</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Debit&nbsp;Amount</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Credit&nbsp;Amount</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["acdesc"].ToString() + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["opdramt"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["opcramt"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["txdramt"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["txcramt"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["cldramt"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["clcramt"].ToString())) + "</td>");
                sb.Append("</tr>");
                opdramt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["opdramt"].ToString());
                opcramt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["opcramt"].ToString());
                txdramt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["txdramt"].ToString());
                txcramt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["txcramt"].ToString());
                cldramt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["cldramt"].ToString());
                clcramt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["clcramt"].ToString());
            }
            //
            sb.Append("<tr class='tblrow'>");
            sb.Append("<td style='width:15px;'>&nbsp;</td>");
            sb.Append("<td style='width:auto;'><b>Total</b></td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(opdramt) + "</b></td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(opcramt) + "</b></td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(txdramt) + "</b></td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(txcramt) + "</b></td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(cldramt) + "</b></td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(clcramt) + "</b></td>");
            sb.Append("</tr>");
            //
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }
       
        public ActionResult getTrialBalance(int compcode, DateTime dtfrom, DateTime dtto, int grcode, string grdesc="", string finyear="")
        {
            //[100072]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            if (mc.isValidDateForFinYear(finyear, dtfrom) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid from date for selected financial year!</h1></a>");
            }
            if (mc.isValidDateForFinYear(finyear, dtto) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid to date for selected financial year!</h1></a>");
            }
            //
            TrialBalanceRptBLL rptBLL = new TrialBalanceRptBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            Reports.dsReport dsr = new Reports.dsReport();

            compBLL = new CompanyBLL();
            FinYearMdl fymdl = new FinYearMdl();
            fymdl = compBLL.getDateRangeByFinancialYear(compcode, finyear);

            if (dtfrom == fymdl.FromDate)//if (dtfrom == objCookie.getFromDate())
            {
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "TrialBalance.rpt"));//[100072]/F1
                rptBLL.prepareTrialBalance(compcode, fymdl.FromDate, dtto, dsr.Tables["dtTrialBalance"], finyear);
                string sf = "{dtTrialBalance.rectype}='a'";
                if (grcode != 0)
                {
                    sf += " and {dtTrialBalance.grcode} = " + grcode + "";
                }
                rptDoc.RecordSelectionFormula = sf;
            }
            else
            {
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "TrialBalanceBtwDates.rpt"));//[100072]/F2
                rptBLL.prepareTrialBalanceBetweenDates(compcode, fymdl.FromDate, dtfrom, dtto, dsr.Tables["dtTrbDate"], finyear);
                string sf = "{dtTrbDate.rectype}='a' and ({dtTrbDate.opdramt} <> 0 or {dtTrbDate.opcramt} <> 0 or {dtTrbDate.pddramt} <> 0 or {dtTrbDate.pdcramt} <> 0 or {dtTrbDate.cldramt} <> 0 or {dtTrbDate.clcramt} <> 0)";
                if (grcode != 0)
                {
                    sf += " and {dtTrbDate.grcode} = " + grcode + "";
                }
                rptDoc.RecordSelectionFormula = sf;
            }
            //
            CompanyMdl compmdl = new CompanyMdl();
            compmdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtrpthead"];
            txtCmpName.Text = compmdl.CmpName;
            txtAddress1.Text = compmdl.Footer1;
            txtrpthead.Text = "Trial Balance From: " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto) + " as on Date :" + mc.getStringByDate(DateTime.Now);
            if (grcode != 0)
            {
                txtrpthead.Text += "\r\nAccount Group: " + grdesc;
            }
            setLoginInfo(rptDoc);
            rptDoc.SetDataSource(dsr);
            //rd.Subreports[0].SetDataSource(dsr);
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
