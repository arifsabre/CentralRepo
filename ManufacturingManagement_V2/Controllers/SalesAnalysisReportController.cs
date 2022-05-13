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
    public class SalesAnalysisReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
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
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            ViewBag.SOType = new SelectList(mc.getSaleOrderTypeList(), "Value", "Text");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region sales analysis report

        [HttpPost]
        public ActionResult SalesAnalysisRpt(rptOptionMdl rptOption)
        {
            //[100019]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //validation for vdate according to fin year
            bool viewper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalesAnalysisReport/getSalesAnalysisRpt";
                string reportpms = "finyear=" + rptOption.FinYear + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&sotype=" + rptOption.POType + "";
                reportpms += "&applydtr=" + rptOption.Above58 + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSalesAnalysisRpt", new { finyear = rptOption.FinYear, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, sotype=rptOption.POType, applydtr = rptOption.Above58 });
        }

        [HttpGet]
        public ActionResult getSalesAnalysisRpt(string finyear, DateTime dtfrom,DateTime dtto, string sotype, bool applydtr)
        {
            //[100019]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            if (mc.isValidDateForFinYear(finyear,dtfrom)==false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid from date for selected financial year!</h1></a>");
            }
            if (mc.isValidDateForFinYear(finyear, dtto) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid to date for selected financial year!</h1></a>");
            }
            //
            if (applydtr == false)
            {
                compBLL = new CompanyBLL();
                FinYearMdl fymdl = new FinYearMdl();
                fymdl = compBLL.getDateRangeByFinancialYear(2, finyear);//note: ccode 2 = PI
                dtfrom = fymdl.FromDate;
                dtto = fymdl.ToDate;
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SalesAnalysisRPT/"), "SalesAnalysisReport_1R.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SalesAnalysisRPT/"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string sotypename = mc.getNameByKey(mc.getSaleOrderTypes(), "sotype", sotype, "sotypename");
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Current Financial Year :" + finyear + ", From Date " + mc.getStringByDate(dtfrom) +" To "+ mc.getStringByDate(dtto)+ " For " + sotypename + " Records";
            if (applydtr == true)
            {
                txtrpthead.Text += "\r\n[To Compare with Supporting/Sub-Reports Only]";
            }
            //dbp parameters   --usp_sales_analysis_report
            rptDoc.SetParameterValue("@cntfinyear", finyear);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@filteropt", sotype);
            //additional parameters --defined in crpt with @name/static
            rptDoc.SetParameterValue("@ApplyDTR", applydtr);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                //if (rptformat.ToLower() == "pdf")
                //{
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                //}
                //else if (rptformat.ToLower() == "excel")
                //{
                    //stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //}
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
            //if (rptformat.ToLower() == "excel")
            //{
                //return File(stream, "application/excel", "SalesAnalysisReport.xls");
            //}
            return File(stream, "application/pdf");
        }

        #endregion

        #region tender report L1

        [HttpPost]
        public ActionResult DisplayTenderReportL1(rptOptionMdl rptOption)
        {
            //[100020]/L1
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (rptOption.POType.ToLower() =="nonacct")//invalid option
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Not applicable for Non-Accountable type records!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalesAnalysisReport/GetTenderReportL1HTML";
                string reportpms = "finyear=" + rptOption.FinYear + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&applydtr=" + rptOption.Above58 + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            //return RedirectToAction("getTenderReportL1", new { finyear = rptOption.FinYear, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, applydtr = rptOption.Above58 });
            return RedirectToAction("GetTenderReportL1HTML", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, finyear = rptOption.FinYear, applydtr = rptOption.Above58 });
        }

        //get
        public ActionResult GetTenderReportL1HTML(DateTime dtfrom, DateTime dtto, int compcode, string finyear, bool applydtr)
        {
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
            //
            setViewData();

            ERP_V1_ReportBLL rptBLL = new ERP_V1_ReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetQuotedTendersForSAReportL1Html(dtfrom, dtto, compcode, finyear, applydtr);

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
            sbHeader.Append("<b><u>Quoted Tenders -L1</u></b>");
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
                if (!arlGroup.Contains(ds.Tables["tbl1"].Rows[i]["groupname"].ToString()))
                {
                    arlGroup.Add(ds.Tables["tbl1"].Rows[i]["groupname"].ToString());
                }
            }

            //report content
            double amt = 0;
            double gtamt = 0;
            int sn = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Railway</th>");
            sb.Append("<th style='width:15px;'>Tender&nbsp;No</th>");
            sb.Append("<th style='width:100px;'>Opening&nbsp;Date</th>");
            sb.Append("<th style='width:250px;'>Item</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Tender&nbsp;Qty</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Quoted&nbsp;Rate<br/>(Basic)</th>");
            sb.Append("<th style='width:15px;text-align:right;'>40%<br/>Qty</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Basic&nbsp;Value<br/>of&nbsp;40%&nbsp;Qty</th>");
            sb.Append("<th style='width:15px;'>Delivery<br/>Period</th>");
            //sb.Append("<th style='width:250px;'>Delivery&nbsp;Schedule</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int x = 0; x < arlGroup.Count; x++)
            {
                amt = 0;
                sn = 1;
                sb.Append("<tr class='tblrow'><td colspan='11'>");
                sb.Append("<b><div style='background-color:lightgray;'>" + arlGroup[x].ToString() + "</div></b>");
                sb.Append("</td></tr>");
                //records
                for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
                {
                    if (ds.Tables["tbl1"].Rows[i]["groupname"].ToString() == arlGroup[x].ToString())
                    {
                        sb.Append("<tr class='tblrow'>");
                        sb.Append("<td>" + sn.ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["rlyshortname"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["TenderNo"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["OpeningDateStr"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["ShortName"].ToString() + "</td>");
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["qty"].ToString())) + "<br/>"+ds.Tables["tbl1"].Rows[i]["unitname"].ToString()+"</td>");
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["rate"].ToString())) + "</td>");
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["ourqty"].ToString())) + "</td>");
                        amt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["basicvalue"].ToString());
                        gtamt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["basicvalue"].ToString());
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["basicvalue"].ToString())) + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["TC2DelvPeriod"].ToString() + "</td>");
                        //sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["DelvSchedule"].ToString() + "</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr class='tblrow'>");
                        sb.Append("<td></td>");
                        sb.Append("<td colspan='9'><b>Delivery Schedule:</b> " + ds.Tables["tbl1"].Rows[i]["DelvSchedule"].ToString() + "</td>");
                        sb.Append("</tr>");
                        sn += 1;
                    }
                }
                sb.Append("<tr class='tblrow'>");//group-total
                sb.Append("<td style='text-align:right;' colspan='9'>");
                sb.Append("<b>" + "Group Total For " + arlGroup[x].ToString() + ": ");
                sb.Append(mc.getINRCFormat(amt) + "</b>");
                sb.Append("<td colspan='2'></td>");
                sb.Append("</td></tr>");
            }
            sb.Append("<tr class='tblrow'>");//grand-total
            sb.Append("<td style='text-align:right;' colspan='9'>");
            sb.Append("<b>" + "Grand Total: ");
            sb.Append(mc.getINRCFormat(gtamt) + "</b>");
            sb.Append("<td colspan='2'></td>");
            sb.Append("</td></tr>");
            sb.Append("</table><br/>");
            
            sb.Append("<div>");
            sb.Append("[Note] Sorted On: Item Group + Item");
            sb.Append("</div>");

            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        //get //not in use
        public ActionResult getTenderReportL1(string finyear, DateTime dtfrom, DateTime dtto, int compcode, bool applydtr)
        {
            //[100020]/L1
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
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
            if (applydtr == false)
            {
                compBLL = new CompanyBLL();
                FinYearMdl fymdl = new FinYearMdl();
                fymdl = compBLL.getDateRangeByFinancialYear(compcode, finyear);
                dtfrom = fymdl.FromDate;
                dtto = fymdl.ToDate;
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SalesAnalysisRPT/"), "SalesAnalysisReport_TenderL1.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Opening Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto) + " For Accountable Records";
            if (applydtr == false)
            {
                txtRptHead.Text = "Opening Date upto " + mc.getStringByDate(DateTime.Now) + " [Fin. Year " + finyear + "] For Accountable Records";
            }
            //dbp --usp_get_quoted_tenders_for_sa_report
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            rptDoc.SetParameterValue("@ApplyDTR", applydtr);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            //records info
            if (!rptDoc.HasRecords)
            {
                txtRptHead.Text += "\r\nNo Recoords found for: " + objCookie.getCmpName();
            }
            //cl-records info

            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                //if (rptformat.ToLower() == "pdf")
                //{
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //    //add these lines to download
                //    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //    //return File(stream, "application/pdf", "ReportName.pdf");
                //}
                //else if (rptformat.ToLower() == "excel")
                //{
                //    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                //    stream.Seek(0, System.IO.SeekOrigin.Begin);
                //}
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
            //if (rptformat.ToLower() == "excel")
            //{
            //    return File(stream, "application/excel", "SalaryReportHO.xls");
            //}
            return File(stream, "application/pdf");
        }

        #endregion

        #region tender report L2

        [HttpPost]
        public ActionResult DisplayTenderReportL2(rptOptionMdl rptOption)
        {
            //[100020]/l2
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (rptOption.POType.ToLower() == "nonacct")//invalid option
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Not applicable for Non-Accountable type records!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalesAnalysisReport/GetTenderReportL2HTML";
                string reportpms = "finyear=" + rptOption.FinYear + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&applydtr=" + rptOption.Above58 + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            //return RedirectToAction("getTenderReportL2", new { finyear = rptOption.FinYear, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, applydtr = rptOption.Above58 });
            return RedirectToAction("GetTenderReportL2HTML", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, finyear = rptOption.FinYear, applydtr = rptOption.Above58 });
        }

        //get
        public ActionResult GetTenderReportL2HTML(DateTime dtfrom, DateTime dtto, int compcode, string finyear, bool applydtr)
        {
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
            //
            setViewData();

            ERP_V1_ReportBLL rptBLL = new ERP_V1_ReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetQuotedTendersForSAReportL2Html(dtfrom, dtto, compcode, finyear, applydtr);

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
            sbHeader.Append("<b><u>Quoted Tenders -L2</u></b>");
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
                if (!arlGroup.Contains(ds.Tables["tbl1"].Rows[i]["groupname"].ToString()))
                {
                    arlGroup.Add(ds.Tables["tbl1"].Rows[i]["groupname"].ToString());
                }
            }

            //report content
            double amt = 0;
            double gtamt = 0;
            int sn = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Railway</th>");
            sb.Append("<th style='width:15px;'>Tender&nbsp;No</th>");
            sb.Append("<th style='width:100px;'>Opening&nbsp;Date</th>");
            sb.Append("<th style='width:250px;'>Item</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Tender&nbsp;Qty</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Quoted&nbsp;Rate<br/>(Basic)</th>");
            sb.Append("<th style='width:15px;text-align:right;'>40%<br/>Qty</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Basic&nbsp;Value<br/>of&nbsp;40%&nbsp;Qty</th>");
            sb.Append("<th style='width:15px;'>Delivery<br/>Period</th>");
            sb.Append("<th style='width:250px;'>Delivery&nbsp;Schedule</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int x = 0; x < arlGroup.Count; x++)
            {
                amt = 0;
                sn = 1;
                sb.Append("<tr class='tblrow'><td colspan='11'>");
                sb.Append("<b><div style='background-color:lightgray;'>" + arlGroup[x].ToString() + "</div></b>");
                sb.Append("</td></tr>");
                //records
                for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
                {
                    if (ds.Tables["tbl1"].Rows[i]["groupname"].ToString() == arlGroup[x].ToString())
                    {
                        sb.Append("<tr class='tblrow'>");
                        sb.Append("<td>" + sn.ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["rlyshortname"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["TenderNo"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["OpeningDateStr"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["ShortName"].ToString() + "</td>");
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["qty"].ToString())) + "<br/>" + ds.Tables["tbl1"].Rows[i]["unitname"].ToString() + "</td>");
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["rate"].ToString())) + "</td>");
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["ourqty"].ToString())) + "</td>");
                        amt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["basicvalue"].ToString());
                        gtamt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["basicvalue"].ToString());
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["basicvalue"].ToString())) + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["TC2DelvPeriod"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["DelvSchedule"].ToString() + "</td>");
                        sb.Append("</tr>");
                        sn += 1;
                    }
                }
                sb.Append("<tr class='tblrow'>");//group-total
                sb.Append("<td style='text-align:right;' colspan='9'>");
                sb.Append("<b>" + "Group Total For " + arlGroup[x].ToString() + ": ");
                sb.Append(mc.getINRCFormat(amt) + "</b>");
                sb.Append("<td colspan='2'></td>");
                sb.Append("</td></tr>");
            }
            sb.Append("<tr class='tblrow'>");//grand-total
            sb.Append("<td style='text-align:right;' colspan='9'>");
            sb.Append("<b>" + "Grand Total: ");
            sb.Append(mc.getINRCFormat(gtamt) + "</b>");
            sb.Append("<td colspan='2'></td>");
            sb.Append("</td></tr>");
            sb.Append("</table><br/>");
            
            sb.Append("<div>");
            sb.Append("[Note] Sorted On: Item Group, Opening Date + Time");
            sb.Append("</div>");

            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        //get //not in use
        public ActionResult getTenderReportL2(string finyear, DateTime dtfrom, DateTime dtto, int compcode, bool applydtr)
        {
            //[100020]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
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
            if (applydtr == false)
            {
                compBLL = new CompanyBLL();
                FinYearMdl fymdl = new FinYearMdl();
                fymdl = compBLL.getDateRangeByFinancialYear(compcode, finyear);
                dtfrom = fymdl.FromDate;
                dtto = fymdl.ToDate;
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SalesAnalysisRPT/"), "SalesAnalysisReport_TenderL2.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Opening Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto) + " For Accountable Records";
            if (applydtr == false)
            {
                txtRptHead.Text = "Opening Date upto " + mc.getStringByDate(DateTime.Now) + " [Fin. Year " + finyear + "] For Accountable Records";
            }
            //dbp --usp_get_quoted_tenders_for_sa_report
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            rptDoc.SetParameterValue("@ApplyDTR", applydtr);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            //records info
            if (!rptDoc.HasRecords)
            {
                txtRptHead.Text += "\r\nNo Recoords found for: " + objCookie.getCmpName();
            }
            //cl-records info

            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                //if (rptformat.ToLower() == "pdf")
                //{
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //    //add these lines to download
                //    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //    //return File(stream, "application/pdf", "ReportName.pdf");
                //}
                //else if (rptformat.ToLower() == "excel")
                //{
                //    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                //    stream.Seek(0, System.IO.SeekOrigin.Begin);
                //}
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
            //if (rptformat.ToLower() == "excel")
            //{
            //    return File(stream, "application/excel", "SalaryReportHO.xls");
            //}
            return File(stream, "application/pdf");
        }

        #endregion

        #region aal/co report

        [HttpPost]
        public ActionResult DisplayAALCOReport(rptOptionMdl rptOption)
        {
            //[100033]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (rptOption.POType.ToLower() == "nonacct")//invalid option
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Not applicable for Non-Accountable type records!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalesAnalysisReport/getAALCOReport";
                string reportpms = "finyear=" + rptOption.FinYear + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&applydtr=" + rptOption.Above58 + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAALCOReport", new { finyear = rptOption.FinYear, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, applydtr = rptOption.Above58 });
        }

        //get
        public ActionResult getAALCOReport(string finyear, DateTime dtfrom, DateTime dtto, int compcode, bool applydtr)
        {
            //[100033]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
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
            compBLL = new CompanyBLL();
            if (applydtr == false)
            {
                FinYearMdl fymdl = new FinYearMdl();
                fymdl = compBLL.getDateRangeByFinancialYear(compcode, finyear);
                dtfrom = fymdl.FromDate;
                dtto = fymdl.ToDate;
            }
            CompanyMdl cmpMdl = new CompanyMdl();
            cmpMdl = compBLL.searchObject(compcode);
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SalesAnalysisRPT/"), "SalesAnalysisReport_AALCO.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtFooter1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtFooter1"];
            txtCmpName.Text = cmpMdl.CmpName;
            txtFooter1.Text = cmpMdl.Footer1;
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "LOA/CO Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto) + " For Accountable Records";
            if (applydtr == false)
            {
                txtRptHead.Text = "LOA/CO Date as on " + mc.getStringByDate(DateTime.Now) + " [Fin. Year " + finyear + "] For Accountable Records";
            }
            //dbp --usp_get_aalco_tenders_for_sa_report
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            rptDoc.SetParameterValue("@ApplyDTR", applydtr);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            //records info
            if (!rptDoc.HasRecords)
            {
                txtRptHead.Text += "\r\nNo Recoords found for: " + objCookie.getCmpName();
            }
            //cl-records info

            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                //if (rptformat.ToLower() == "pdf")
                //{
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //    //add these lines to download
                //    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //    //return File(stream, "application/pdf", "ReportName.pdf");
                //}
                //else if (rptformat.ToLower() == "excel")
                //{
                //    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                //    stream.Seek(0, System.IO.SeekOrigin.Begin);
                //}
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
            //if (rptformat.ToLower() == "excel")
            //{
            //    return File(stream, "application/excel", "SalaryReportHO.xls");
            //}
            return File(stream, "application/pdf");
        }

        #endregion

        #region pending po report

        [HttpPost]
        public ActionResult DisplayPendingPOReport(rptOptionMdl rptOption)
        {
            //[100034]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalesAnalysisReport/getPendingPOReport";
                string reportpms = "finyear=" + rptOption.FinYear + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&sotype=" + rptOption.POType + "";
                reportpms += "&applydtr=" + rptOption.Above58 + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getPendingPOReport", new { finyear = rptOption.FinYear, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, sotype = rptOption.POType, compcode = rptOption.CompCode, applydtr = rptOption.Above58 });
        }

        //get
        public ActionResult getPendingPOReport(string finyear, DateTime dtfrom, DateTime dtto, string sotype, int compcode, bool applydtr)
        {
            //[100034]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
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
            if (applydtr == false)
            {
                compBLL = new CompanyBLL();
                FinYearMdl fymdl = new FinYearMdl();
                fymdl = compBLL.getDateRangeByFinancialYear(compcode, finyear);
                dtfrom = fymdl.FromDate;
                dtto = fymdl.ToDate;
            }
            CompanyMdl cmpMdl = new CompanyMdl();
            cmpMdl = compBLL.searchObject(compcode);
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SalesAnalysisRPT/"), "SalesAnalysisReport_PendingPO.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtFooter1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtFooter1"];
            txtCmpName.Text = cmpMdl.CmpName;
            txtFooter1.Text = cmpMdl.Footer1;
            string sotypename = mc.getNameByKey(mc.getSaleOrderTypes(), "sotype", sotype, "sotypename");
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "DP Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto) + " For " + sotypename + " Records";
            if (applydtr == false)
            {
                txtRptHead.Text = "DP Date upto " + mc.getStringByDate(dtto) + " [Fin. Year " + finyear + "] For " + sotypename + " Records";
            }
            //dbp --usp_get_pending_po_for_sa_report
            rptDoc.SetParameterValue("@filteropt", sotype);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            rptDoc.SetParameterValue("@ApplyDTR", applydtr);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                //if (rptformat.ToLower() == "pdf")
                //{
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //    //add these lines to download
                //    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //    //return File(stream, "application/pdf", "ReportName.pdf");
                //}
                //else if (rptformat.ToLower() == "excel")
                //{
                //    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                //    stream.Seek(0, System.IO.SeekOrigin.Begin);
                //}
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
            //if (rptformat.ToLower() == "excel")
            //{
            //    return File(stream, "application/excel", "SalaryReportHO.xls");
            //}
            return File(stream, "application/pdf");
        }

        #endregion

        #region sale summary

        [HttpPost]
        public ActionResult DisplaySaleSummaryReport(rptOptionMdl rptOption)
        {
            //[100035]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Analysis_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalesAnalysisReport/getSaleSummaryReport";
                string reportpms = "finyear=" + rptOption.FinYear + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&detailed=" + rptOption.Detailed + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&sotype=" + rptOption.POType + "";
                reportpms += "&applydtr=" + rptOption.Above58 + "";
                reportpms += "&newformat=" + rptOption.FilterByDT + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSaleSummaryReport", new { finyear = rptOption.FinYear, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, sotype = rptOption.POType, compcode = rptOption.CompCode, detailed = rptOption.Detailed, applydtr = rptOption.Above58, newformat = rptOption.FilterByDT });
        }

        //get
        public ActionResult getSaleSummaryReport(string finyear, DateTime dtfrom, DateTime dtto, string sotype, int compcode, bool detailed = false, bool applydtr = false, bool newformat = false)
        {
            //[100035]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
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
            if (applydtr == false)
            {
                compBLL = new CompanyBLL();
                FinYearMdl fymdl = new FinYearMdl();
                fymdl = compBLL.getDateRangeByFinancialYear(compcode, finyear);
                dtfrom = fymdl.FromDate;
                dtto = fymdl.ToDate;
            }
            CompanyMdl cmpMdl = new CompanyMdl();
            cmpMdl = compBLL.searchObject(compcode);
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            if (newformat == true)
            {
                //Record Selection Formula Applied as-
                //{usp_get_sale_summary_for_sa_report;1.PrvYrAmt}+{usp_get_sale_summary_for_sa_report;1.SuppAmt}<>0
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SalesAnalysisRPT/"), "SalesAnalysisReport_SaleSummary_New.rpt"));
            }
            else //old format
            {
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SalesAnalysisRPT/"), "SalesAnalysisReport_SaleSummary_Old.rpt"));
            }
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtFooter1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtFooter1"];
            txtCmpName.Text = cmpMdl.CmpName;
            txtFooter1.Text = cmpMdl.Footer1;
            string sotypename = mc.getNameByKey(mc.getSaleOrderTypes(), "sotype", sotype, "sotypename");
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Financial Year :" + finyear + ", From Date " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto) + " For " + sotypename + " Records";
            //dbp --usp_get_sale_summary_for_sa_report
            rptDoc.SetParameterValue("@finyear", finyear);
            rptDoc.SetParameterValue("@filteropt", sotype);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            rptDoc.SetParameterValue("@IsDetailed", detailed);
            rptDoc.SetParameterValue("@ApplyDTR", applydtr);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                //if (rptformat.ToLower() == "pdf")
                //{
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //    //add these lines to download
                //    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //    //return File(stream, "application/pdf", "ReportName.pdf");
                //}
                //else if (rptformat.ToLower() == "excel")
                //{
                //    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                //    stream.Seek(0, System.IO.SeekOrigin.Begin);
                //}
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
            //if (rptformat.ToLower() == "excel")
            //{
            //    return File(stream, "application/excel", "SalaryReportHO.xls");
            //}
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
