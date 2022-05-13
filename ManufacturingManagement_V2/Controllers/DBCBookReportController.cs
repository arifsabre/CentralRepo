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
    public class DBCBookReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        DBCBookRptBLL objBLL = new DBCBookRptBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        public ActionResult DayBookIndex()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            rptOption.Detailed = true;
            rptOption.Above58 = true;
            return View(rptOption);
        }

        public ActionResult CashBookIndex()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            rptOption.Detailed = true;
            rptOption.Above58 = true;
            return View(rptOption);
        }

        public ActionResult BankBookIndex()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            rptOption.Detailed = true;
            rptOption.Above58 = true;
            return View(rptOption);
        }

        #region financial ledger report

        [HttpPost]
        public ActionResult DisplayDayBook(rptOptionMdl rptOption)
        {
            //[100074]
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
                string reporturl = "DBCBookReport/GetReportHTML";
                string reportpms = "compcode=" + rptOption.CompCode.ToString() + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&accode=" + rptOption.ItemId + "";
                reportpms += "&printvno=" + rptOption.Detailed + "";
                reportpms += "&printnarr=" + rptOption.Above58 + "";
                reportpms += "&rptname=" + reportName.DayBook + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { rptname = reportName.DayBook, compcode = rptOption.CompCode, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, printvno = rptOption.Detailed, printnarr = rptOption.Above58, finyear = rptOption.FinYear });
        }

        [HttpPost]
        public ActionResult DisplayCashBook(rptOptionMdl rptOption)
        {
            //[100075]
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
                string reporturl = "DBCBookReport/GetReportHTML";
                string reportpms = "compcode=" + rptOption.CompCode.ToString() + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&accode=" + rptOption.ItemId + "";
                reportpms += "&printvno=" + rptOption.Detailed + "";
                reportpms += "&printnarr=" + rptOption.Above58 + "";
                reportpms += "&rptname=" + reportName.CashBook + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { rptname = reportName.CashBook, compcode = rptOption.CompCode, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, printvno = rptOption.Detailed, printnarr = rptOption.Above58, finyear = rptOption.FinYear });
        }

        [HttpPost]
        public ActionResult DisplayBankBook(rptOptionMdl rptOption)
        {
            //[100076]
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
                string reporturl = "DBCBookReport/GetReportHTML";
                string reportpms = "compcode=" + rptOption.CompCode.ToString() + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&accode=" + rptOption.ItemId + "";
                reportpms += "&printvno=" + rptOption.Detailed + "";
                reportpms += "&printnarr=" + rptOption.Above58 + "";
                reportpms += "&rptname=" + reportName.BankBook + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { rptname = reportName.BankBook, compcode = rptOption.CompCode, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, printvno = rptOption.Detailed, printnarr = rptOption.Above58, accode = rptOption.ItemId, finyear = rptOption.FinYear });
        }

        public ActionResult GetReportHTML(reportName rptname, int compcode, DateTime dtfrom, DateTime dtto, bool printvno, bool printnarr, int accode = 0, string finyear = "")
        {
            setViewData();
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            AccountReportsBLL rptBLL = new AccountReportsBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetBookReportHtml(rptname, compcode, accode, dtfrom, dtto, printvno, printnarr, finyear);
            //
            string reportname = "";
            if (rptname == reportName.DayBook)
            {
                reportname = "Day Book Report";
            }
            else if (rptname == reportName.BankBook)
            {
                reportname = "Bank Book Report";
            }
            else if (rptname == reportName.CashBook)
            {
                reportname = "Cash Book Report";
            }
            //
            compBLL = new CompanyBLL();
            CompanyMdl cmpMdl = new CompanyMdl();
            cmpMdl = compBLL.searchObject(compcode);

            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            sbHeader.Append("<div style='text-align:center;'>");//div main
            //company
            sbHeader.Append("<div style='font-size:12pt;'>");
            sbHeader.Append(cmpMdl.CmpName);
            sbHeader.Append("</div>");
            //cmp address
            sbHeader.Append("<div style='font-size:10pt;'>");
            sbHeader.Append(cmpMdl.Footer1);
            sbHeader.Append("</div>");
            //repoprt name
            sbHeader.Append("<div style='font-size:11pt;background-color:lightgray;'>");
            sbHeader.Append("<b><u>"+ reportname + "</u></b>");
            sbHeader.Append("</div>");
            //report filters
            string fltr = "Date From :" + mc.getStringByDate(dtfrom) + " To: " + mc.getStringByDate(dtto);
            if(accode != 0)
            {
                AccountMdl acctMdl = new AccountMdl();
                AccountBLL acctBll = new AccountBLL();
                acctMdl = acctBll.searchAccountMaster(accode);
                fltr += ", Account: " + acctMdl.AcDesc;
            }
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            sbHeader.Append(fltr);
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            //report content
            double tldramt = 0;
            double tlcramt = 0;
            DateTime vdate = dtfrom;
            string vdst = "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<br/>");
            while (vdate <= dtto)
            {
                string vstr = vdate.DayOfWeek + ", "+vdate.Day.ToString()+" "+vdate.ToString("MMMM")+", "+vdate.Year.ToString();
                sb.Append("<div style='text-align:center;'><b>" + vstr + "</b></div>");
                //rptblock
                sb.Append("<table class='tblcontainer' style='width:100%;'>");
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th style='width:auto;'>Particulars(CREDIT)</th>");
                sb.Append("<th style='width:15px;text-align:right;'>Dr&nbsp;Amount</th>");
                sb.Append("<th style='width:auto;'>Particulars(DEDIT)</th>");
                sb.Append("<th style='width:15px;text-align:right;'>Cr&nbsp;Amount</th>");
                sb.Append("</tr>");
                sb.Append("</thead>");
                tldramt = 0;
                tlcramt = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    vdst = mc.getStringByDate(Convert.ToDateTime(ds.Tables[0].Rows[i]["vdate"].ToString()));
                    if (vdst == mc.getStringByDate(vdate))
                    {
                        sb.Append("<tr class='tblrow'>");
                        sb.Append("<td>" + ds.Tables[0].Rows[i]["CrPart"].ToString() + "</td>");
                        if (ds.Tables[0].Rows[i]["CrPart"].ToString().Length > 0)
                        {
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables[0].Rows[i]["CrAmount"].ToString())) + "</td>");
                        }
                        else
                        {
                            sb.Append("<td>&nbsp;</td>");
                        }
                        sb.Append("<td>" + ds.Tables[0].Rows[i]["DrPart"].ToString() + "</td>");
                        if (ds.Tables[0].Rows[i]["DrPart"].ToString().Length > 0)
                        {
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables[0].Rows[i]["DrAmount"].ToString())) + "</td>");
                        }
                        else
                        {
                            sb.Append("<td>&nbsp;</td>");
                        }
                        sb.Append("</tr>");
                        tldramt += Convert.ToDouble(ds.Tables[0].Rows[i]["DrAmount"].ToString());
                        tlcramt += Convert.ToDouble(ds.Tables[0].Rows[i]["CrAmount"].ToString());
                    }
                }
                //
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td style='width:15px;'><b>Total</b></td>");
                sb.Append("<td align='right'><b>" + mc.getINRCFormat(tldramt) + "</b></td>");
                sb.Append("<td style='width:15px;'><b>Total</b></td>");
                sb.Append("<td align='right'><b>" + mc.getINRCFormat(tldramt) + "</b></td>");
                sb.Append("</tr>");
                //
                sb.Append("</table><br/>");
                //cl-rptblock
                //----------------------
                vdate = vdate.AddDays(1);
            }
            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        public ActionResult getDBCBookReport(reportName rptname, int compcode, DateTime dtfrom, DateTime dtto, bool printvno, bool printnarr, int accode=0, string finyear="")
        {
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
            if (printvno == false && printnarr == false)
            {
                printvno = true;
            }
            DBCBookRptBLL rptBLL = new DBCBookRptBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            Reports.dsReport dsr = new Reports.dsReport();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "DayBankCashBook.rpt"));
            if(rptname == reportName.DayBook)
            {
                //[100074]
                rptBLL.prepareBookReport(reportName.DayBook, compcode, "0", dtfrom, dtto, dsr.Tables["dtDayBook"], printvno, printnarr, finyear);
            }
            else if (rptname == reportName.CashBook)
            {
                //[100075]
                rptBLL.prepareBookReport(reportName.CashBook, compcode, "0", dtfrom, dtto, dsr.Tables["dtDayBook"], printvno, printnarr, finyear);
            }
            else if (rptname == reportName.BankBook)
            {
                //[100076]
                rptBLL.prepareBookReport(reportName.BankBook, compcode, accode.ToString(), dtfrom, dtto, dsr.Tables["dtDayBook"], printvno, printnarr, finyear);
            }
            //
            DataRow dr = dsr.Tables["dtRptVariables"].NewRow();
            dr["strcol1"] = "0";
            dr["strcol2"] = "0";
            if (printvno == true)
            {
                dr["strcol1"] = "1";
            }
            if (printnarr == true)
            {
                dr["strcol2"] = "1";
            }
            dsr.Tables["dtRptVariables"].Rows.Add(dr);
            //
            CompanyMdl compmdl = new CompanyMdl();
            compmdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptHead"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptName"];
            txtCmpName.Text = compmdl.CmpName;
            txtAddress1.Text = compmdl.Footer1;
            txtRptName.Text = rptname.ToString() + " Report";
            txtRptHead.Text = "Date From: " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
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
