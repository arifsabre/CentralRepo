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
    public class JournalBookReportController : Controller
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
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region journal book report

        [HttpPost]
        public ActionResult DisplayJournalBook(rptOptionMdl rptOption)
        {
            //[100077]
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
                string reporturl = "JournalBookReport/GetReportHTML";
                string reportpms = "compcode=" + rptOption.CompCode.ToString() + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, finyear = rptOption.FinYear });
        }

        public ActionResult GetReportHTML(DateTime dtfrom, DateTime dtto, int compcode, string finyear)
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
            ds = rptBLL.GetJournalBookReportHtml(dtfrom, dtto, compcode, finyear);

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
            sbHeader.Append("<b><u>Journal Book Report</u></b>");
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
            System.Collections.ArrayList arl = new System.Collections.ArrayList();
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                if (arl.Contains(ds.Tables["tbl1"].Rows[i]["vtpvno"].ToString()) == false)
                {
                    arl.Add(ds.Tables["tbl1"].Rows[i]["vtpvno"].ToString());
                }
            }
            //
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            for (int x = 0; x < arl.Count; x++)
            {
                tldramt = 0;
                tlcramt = 0;
                sb.Append("<tr style='border-top:1px solid;border-bottom:1px solid;'>");
                sb.Append("<td colspan='2' style='border:none;'><b>" + arl[x].ToString() + "</b></td>");
                sb.Append("<td align='right' style='width:100px;border:none;'><b>Debit</b></td>");
                sb.Append("<td align='right' style='width:100px;border:none;'><b>Credit</b></td>");
                sb.Append("</tr>");
                for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
                {
                    if (arl[x].ToString() == ds.Tables["tbl1"].Rows[i]["vtpvno"].ToString())
                    {
                        dramt = Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["dramt"].ToString());
                        cramt = Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["cramt"].ToString());
                        sb.Append("<tr style='border-top:1px dotted;border-bottom:1px dotted;'>");
                        sb.Append("<td colspan='2' style='border:none;'><b>" + ds.Tables["tbl1"].Rows[i]["acdesc"].ToString() + "</b></td>");
                        sb.Append("<td style='border:none;'>&nbsp;</td>");
                        sb.Append("<td style='border:none;'>&nbsp;</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr style='border-top:1px dotted;border-bottom:1px dotted;'>");
                        sb.Append("<td style='width:10px;border:none;'>&nbsp;</td>");
                        sb.Append("<td style='border:none;'>" + ds.Tables["tbl1"].Rows[i]["narration"].ToString() + "</td>");
                        sb.Append("<td align='right' style='border:none;'>" + mc.getINRCFormat(dramt) + "</td>");
                        sb.Append("<td align='right' style='border:none;'>" + mc.getINRCFormat(cramt) + "</td>");
                        sb.Append("</tr>");
                        tldramt += dramt;
                        tlcramt += cramt;
                    }
                }
                //
                sb.Append("<tr style='border-top:1px solid;border-bottom:1px solid;'>");
                sb.Append("<td style='width:15px;border:none;'>&nbsp;</td>");
                sb.Append("<td style='width:15px;border:none;'><b>Total</b></td>");
                sb.Append("<td align='right' style='border:none;'><b>" + mc.getINRCFormat(tldramt) + "</b></td>");
                sb.Append("<td align='right' style='border:none;'><b>" + mc.getINRCFormat(tlcramt) + "</b></td>");
                sb.Append("</tr>");
                sb.Append("<tr><td style='border:none;'>&nbsp;</td></tr>");
                //
            }
            sb.Append("</table>");
            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        public ActionResult getJournalBookReport(int compcode, DateTime dtfrom, DateTime dtto, string finyear="")
        {
            //[100077]
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
            JournalBookRptBLL rptBLL = new JournalBookRptBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            Reports.dsReport dsr = new Reports.dsReport();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "JournalBook.rpt"));
            rptBLL.prepareJournalBookReport(dtfrom, dtto, dsr.Tables["dtVoucher"], compcode, finyear);
            //
            CompanyMdl compmdl = new CompanyMdl();
            compmdl = compBLL.searchObject(compcode);
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress1"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRptHead"];
            txtCmpName.Text = compmdl.CmpName;
            txtAddress1.Text = compmdl.Footer1;
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
