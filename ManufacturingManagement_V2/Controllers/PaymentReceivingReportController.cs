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
    public class PaymentReceivingReportController : Controller
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

        #region payment receiving report

        [HttpPost]
        public ActionResult PaymentReceiptRpt(rptOptionMdl rptOption)
        {
            //[100015]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Billing_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Billing_Report, permissionType.Edit);
            //if (rptOption.EmpName == null) { rptOption.EmpName = ""; };
            if (rptOption.FgItemCode == null) { rptOption.FgItemId = 0; };
            string methodName = "getPaymentReceiptReport";
            if (rptOption.ReportFormat.ToLower() == "excel")
            {
                methodName = "getPaymentReceiptReportCSV";
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "PaymentReceivingReport/" + methodName;
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&railwayid=" + rptOption.RailwayId + "";
                reportpms += "&billos=" + rptOption.Detailed + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction(methodName, new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, railwayid = rptOption.RailwayId, billos = rptOption.Detailed });
        }

        [HttpGet]
        public FileResult getPaymentReceiptReport(DateTime dtfrom, DateTime dtto, int compcode, int railwayid=0, bool billos = false)
        {
            //[100015]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            if (billos == true)
            {
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "PaymentReceivingRpt_V2.rpt"));
            }
            else
            {
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "PaymentReceivingRpt.rpt"));//TestXCrystalReport1
            }
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Receiving Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (railwayid > 0)
            {
                DataSet ds = rptBll.getRailwayData();
                txtrpthead.Text += ", Railway : " + mc.getNameByKey(ds, "railwayid", railwayid.ToString(), "railwayname");
            }
            //dbp parameters   --usp_get_payment_receipt_report
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@RailwayId", railwayid);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //add these lines to download
                //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //return File(stream, "application/pdf", "ReportName.pdf");
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
            return File(stream, "application/pdf");
        }

        public ActionResult getPaymentReceiptReportCSV(DateTime dtfrom, DateTime dtto, int compcode, int railwayid = 0, bool billos = false)
        {
            //[100015]-Excel
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            string rptname = "Payment Receipt Report";
            ERP_V1_ReportBLL rptBLL = new ERP_V1_ReportBLL();
            DataSet ds = new System.Data.DataSet();
            //to be changed for billos
            ds = rptBLL.getPaymentReceiptReportCSVData(dtfrom, dtto, railwayid, compcode);
            //
            DataTable dtr = new DataTable();
            dtr.Columns.Add("SNo");
            dtr.Columns.Add("Company");
            dtr.Columns.Add("Railway");
            dtr.Columns.Add("Invoice No");
            dtr.Columns.Add("Invoice Date");
            dtr.Columns.Add("Paying Authority");
            dtr.Columns.Add("Invoice Amount");
            dtr.Columns.Add("Bill Per");
            dtr.Columns.Add("Bill Amount");
            dtr.Columns.Add("Bill Date");
            dtr.Columns.Add("Received Amount");
            dtr.Columns.Add("Receiving Date");
            dtr.Columns.Add("Memo No");
            dtr.Columns.Add("Delay Days");
            DataRow dr = dtr.NewRow();
            DateTime vdate = new DateTime();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                dr = dtr.NewRow();
                dr["SNo"] = (i + 1).ToString();
                dr["Railway"] = ds.Tables[0].Rows[i]["RailwayName"].ToString();
                dr["Company"] = ds.Tables[0].Rows[i]["CmpName"].ToString();
                dr["Invoice No"] = ds.Tables[0].Rows[i]["BillNo"].ToString();
                dr["Paying Authority"] = ds.Tables[0].Rows[i]["PayingAuthName"].ToString();
                dr["Invoice Amount"] = ds.Tables[0].Rows[i]["InvAmount"].ToString();
                dr["Bill Per"] = ds.Tables[0].Rows[i]["BillPer"].ToString();
                dr["Bill Amount"] = ds.Tables[0].Rows[i]["BillAmount"].ToString();
                dr["Received Amount"] = ds.Tables[0].Rows[i]["RecAmount"].ToString();
                dr["Memo No"] = ds.Tables[0].Rows[i]["MemoNo"].ToString();
                dr["Delay Days"] = ds.Tables[0].Rows[i]["DelayDays"].ToString();
                vdate = Convert.ToDateTime(ds.Tables[0].Rows[i]["invdate"].ToString());
                dr["Invoice Date"] = mc.getStringByDateDDMMYYYY(vdate);
                vdate = Convert.ToDateTime(ds.Tables[0].Rows[i]["BillDate"].ToString());
                dr["Bill Date"] = mc.getStringByDateDDMMYYYY(vdate);
                vdate = Convert.ToDateTime(ds.Tables[0].Rows[i]["receivingdate"].ToString());
                dr["Receiving Date"] = mc.getStringByDateDDMMYYYY(vdate);
                dtr.Rows.Add(dr);
            }
            //
            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = dtr;
            grid.DataBind();
            //
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + rptname + ".xls");
            Response.ContentType = "application/ms-excel";
            //Response.AddHeader("content-disposition", "attachment; filename=" + rptname + ".html");
            //Response.ContentType = "application/htm";
            //
            Response.Charset = "";
            //
            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            //
            compBLL = new CompanyBLL();
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            //
            //for company group
            htw.AddAttribute("CustomAttribute", "CustomAttributeValue");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "16pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "bold");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.TextAlign, "center");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine("PRAG GROUP");
            htw.WriteBreak();
            //for report name
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "14pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "bold");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.TextAlign, "center");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine(rptname);
            htw.WriteBreak();
            //for rpt head
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "12pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "bold");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.TextAlign, "center");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            string rptheader = "Receiving Date From: " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (railwayid > 0)
            {
                DataSet dsrly = new DataSet();
                dsrly = rptBll.getRailwayData();
                rptheader += ", Railway : " + mc.getNameByKey(dsrly, "railwayid", railwayid.ToString(), "railwayname");
            }
            htw.WriteLine(rptheader);
            htw.WriteBreak();
            //
            grid.RenderControl(htw);
            //
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "10pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "regular");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine("---End of Report---[100015], Note: PO Type: Railway & Private Party / Sorted on Company + Invoice Date + Bill Part. Delay from Bill Date. Not from BillOS.");
            htw.WriteBreak();
            //
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            //
            return View();
            //
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
