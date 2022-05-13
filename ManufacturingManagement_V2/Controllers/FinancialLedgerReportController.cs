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
    public class FinancialLedgerReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        FinancialLedgerRptBLL objBLL = new FinancialLedgerRptBLL();

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
            rptOption.Detailed = true; //Print account head
            rptOption.Above58 = true; //Print narration
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }       

        #region financial ledger report

        [HttpPost]
        public ActionResult DisplayFinancialLedger(rptOptionMdl rptOption)
        {
            //[100073]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Account_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Account_Report, permissionType.Edit);
            if (rptOption.ItemCode == null || rptOption.ItemCode.Length == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Account not selected!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "FinancialLedgerReport/GetReportHTML";
                string reportpms = "compcode=" + rptOption.CompCode.ToString() + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&accode=" + rptOption.ItemId + "";
                //reportpms += "&acdesc=" + rptOption.ItemCode + "";
                reportpms += "&printhead=" + rptOption.Detailed + "";
                reportpms += "&printnarr=" + rptOption.Above58 + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { accode = rptOption.ItemId, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, finyear=rptOption.FinYear, printhead = rptOption.Detailed, printnarr = rptOption.Above58 });
        }

        [HttpGet]
        public ActionResult FinLedgerEmployee(int newempid=0, string finyear="")
        {
            //[100073]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Account_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Account_Report, permissionType.Edit);
            if (newempid == 0 || finyear.Length == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid attempt!</h1></a>");
            }
            //
            VoucherBLL bllObj = new VoucherBLL();
            DataSet ds = new DataSet();
            ds = bllObj.getEmployeeAcCode(newempid);
            compBLL = new CompanyBLL();
            FinYearMdl fymdl = new FinYearMdl();
            fymdl = compBLL.getDateRangeByFinancialYear(Convert.ToInt32(ds.Tables[0].Rows[0]["compcode"].ToString()), finyear);
            bool opt = true;
            int ccode = Convert.ToInt32(ds.Tables[0].Rows[0]["compcode"].ToString());
            int accode = Convert.ToInt32(ds.Tables[0].Rows[0]["accode"].ToString());
            string acdesc = ds.Tables[0].Rows[0]["acdesc"].ToString();
            //
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "FinancialLedgerReport/GetReportHTML";
                string reportpms = "compcode=" + ccode + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(fymdl.FromDate) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(fymdl.ToDate) + "";
                reportpms += "&accode=" + accode + "";
                //reportpms += "&acdesc=" + acdesc + "";
                reportpms += "&printhead=" + opt + "";
                reportpms += "&printnarr=" + opt + "";
                reportpms += "&finyear=" + finyear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { accode = accode, dtfrom = fymdl.FromDate, dtto = fymdl.ToDate, compcode = ccode, finyear = finyear, printhead = opt, printnarr = opt });
        }

        public ActionResult GetReportHTML(int accode, DateTime dtfrom, DateTime dtto, int compcode, string finyear, bool printhead=true, bool printnarr=true)
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

            //dataset from dbProcedures/Account_FinancialLedgerRPT_SP.sql
            FinancialLedgerRptBLL rptBLL = new FinancialLedgerRptBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetFinancialLedgerReportHtml(accode, dtfrom, dtto, compcode, finyear);

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
            sbHeader.Append("<b><u>Financial Ledger Report</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString());
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            //report content
            double opb = Convert.ToDouble(ds.Tables["tbl1"].Rows[0]["opening"].ToString());
            double lgropb = opb;
            double balance = opb;
            double dramt = 0;
            double cramt = 0;
            string particulars = "";
            string drcr = "";
            string strv = "";
            double tldramt = 0;
            double tlcramt = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Date</th>");
            sb.Append("<th style='width:15px;'>VNo</th>");
            sb.Append("<th style='width:15px;width:auto;'>Particulars</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Debit</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Credit</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Balance</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            //opening row
            sb.Append("<tr class='tblrow'>");
            sb.Append("<td>&nbsp;</td>");
            sb.Append("<td>&nbsp;</td>");
            sb.Append("<td>&nbsp;</td>");
            sb.Append("<td>By Opening Balance</td>");
            sb.Append("<td>&nbsp;</td>");
            sb.Append("<td>&nbsp;</td>");
            strv = mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[0]["OpDisp"].ToString()));
            strv += "&nbsp;"+ds.Tables["tbl1"].Rows[0]["drcr"].ToString();
            sb.Append("<td align='right'>" + strv + "</td>");
            sb.Append("</tr>");
            for (int i = 0; i < ds.Tables["tbl2"].Rows.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl2"].Rows[i]["vdate"].ToString() + "</td>");
                strv = ds.Tables["tbl2"].Rows[i]["vtype"].ToString();
                strv += "&nbsp;"+ds.Tables["tbl2"].Rows[i]["vno"].ToString();
                sb.Append("<td>" + strv + "</td>");
                //
                particulars = "";
                if (printhead == true)
                {
                    particulars += ds.Tables["tbl2"].Rows[i]["acdesc"].ToString();
                }
                if (printnarr == true)
                {
                    if (particulars.Length > 0) { particulars += "<br/>"; };
                    particulars += ds.Tables["tbl2"].Rows[i]["narration"].ToString();
                }
                sb.Append("<td>" + particulars.Trim() + "</td>");
                //
                dramt = Convert.ToDouble(ds.Tables["tbl2"].Rows[i]["dramt"].ToString());
                cramt = Convert.ToDouble(ds.Tables["tbl2"].Rows[i]["cramt"].ToString());
                balance = opb + dramt - cramt;
                drcr = "Dr";
                if (balance < 0) { drcr = "Cr"; };
                tldramt += dramt;
                tlcramt += cramt;
                //
                sb.Append("<td align='right'>" + mc.getINRCFormat(dramt) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(cramt) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Math.Abs(balance)) +"&nbsp;"+ drcr + "</td>");
                opb = balance;
                sb.Append("</tr>");
            }
            //
            sb.Append("<tr class='tblrow'>");
            sb.Append("<td style='width:15px;'>&nbsp;</td>");
            sb.Append("<td style='width:15px;'>&nbsp;</td>");
            sb.Append("<td style='width:15px;'>&nbsp;</td>");
            sb.Append("<td style='width:auto;'>&nbsp;</td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(tldramt) + "</b></td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(tlcramt) + "</b></td>");
            double finbalance = lgropb + tldramt - tlcramt;
            drcr = "Dr";
            if (finbalance < 0) { drcr = "Cr"; };
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(Math.Abs(finbalance)) + "&nbsp;" + drcr + "</b></td>");
            sb.Append("</tr>");
            //
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        public ActionResult getFinancialLedger(int compcode, DateTime dtfrom, DateTime dtto, int accode, string acdesc, bool printhead, bool printnarr, string finyear = "")
        {
            //[100073]
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
            FinancialLedgerRptBLL rptBLL = new FinancialLedgerRptBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            Reports.dsReport dsr = new Reports.dsReport();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "FinancialLedger.rpt"));
            rptBLL.prepareFinancialLedger(accode.ToString(), acdesc, dtfrom, dtto, dsr.Tables["dtFinLedger"], compcode, finyear);
            //
            DataRow dr = dsr.Tables["dtRptVariables"].NewRow();
            dr["strcol1"] = "0";
            dr["strcol2"] = "0";
            if (printhead==true)
            {
                dr["strcol1"] = "1";
            }
            if (printnarr==true)
            {
                dr["strcol2"] = "1";
            }
            dsr.Tables["dtRptVariables"].Rows.Add(dr);
            //
            CompanyMdl compmdl = new CompanyMdl();
            compmdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtrpthead"];
            txtCmpName.Text = compmdl.CmpName;
            txtAddress1.Text = compmdl.Footer1;
            txtrpthead.Text = "From: " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
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
