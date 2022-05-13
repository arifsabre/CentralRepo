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
    public class PendingPaymentReportController : Controller
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
            ViewBag.RailwayList = new SelectList(rptBll.getRailwayList(), "railwayid", "railwayname");
            ViewBag.POTypeList = new SelectList(mc.getPOTypeRptList(), "Value", "Text");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region pending payment report

        [HttpPost]
        public ActionResult DisplayPendingPaymentReport(rptOptionMdl rptOption)
        {
            //[100110]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Billing_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Billing_Report, permissionType.Edit);
            string methodName = "GetReportHTML";
            if (rptOption.ReportFormat.ToLower() == "excel")
            {
                methodName = "getPendingPaymentReportCSV";
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "PendingPaymentReport/" + methodName;
                string reportpms = "potype=" + rptOption.POType + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&vdate=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&railwayid=" + rptOption.RailwayId + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&billos=" + rptOption.Detailed + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            //return RedirectToAction(methodName, new { potype = rptOption.POType, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, railwayid = rptOption.RailwayId, compcode = rptOption.CompCode, billos = rptOption.Detailed});
            return RedirectToAction(methodName, new { potype = rptOption.POType, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, railwayid = rptOption.RailwayId, compcode = rptOption.CompCode });
        }

        //get
        public ActionResult GetReportHTML(string potype, DateTime dtfrom, DateTime dtto, int railwayid, int compcode)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();

            ERP_V1_ReportBLL rptBLL = new ERP_V1_ReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetPendingPaymentReportHtml(potype, dtfrom, dtto, railwayid, compcode);

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
            sbHeader.Append("<b><u>Pending Payment Report</u></b>");
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
            double netamt = 0;
            //double billbalance = 0;
            double invbalance = 0;
            double gtnetamt = 0;
            //double gtbillbalance = 0;
            double gtinvbalance = 0;
            int sn = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Case<br/>File&nbsp;No</th>");
            sb.Append("<th style='width:15px;'>Railway</th>");
            sb.Append("<th style='width:15px;'>Invoice<br/>No</th>");
            sb.Append("<th style='width:15px;'>Invoice<br/>Date</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Invoice<br/>Amount</th>");
            //sb.Append("<th style='width:15px;text-align:right;'>Bill<br/>Balance</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Invoice<br/>Balance</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Delay<br/>Days</th>");
            sb.Append("<th style='width:auto;'>Remarks</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int x = 0; x < arlGroup.Count; x++)
            {
                netamt = 0;
                //billbalance = 0;
                invbalance = 0;
                sn = 1;
                sb.Append("<tr class='tblrow'><td colspan='9'>");
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
                        sb.Append("<td><a target='_new' href='https://pragerp.com:130/Report/DisplayControlledDocument.aspx?strvalue=" + ds.Tables["tbl1"].Rows[i]["porderid"].ToString() + ".pdf?CaseFile?0?0?1'>"+ ds.Tables["tbl1"].Rows[i]["casefileno"].ToString() + "</a></td>");
                        //
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["railwayname"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["billno"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["vdatestr"].ToString() + "</td>");
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["netamount"].ToString())) + "</td>");
                        //sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["recoverableamt"].ToString())) + "</td>");
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["invbalance"].ToString())) + "</td>");
                        sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["dayspending"].ToString())) + "</td>");
                        netamt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["netamount"].ToString());
                        gtnetamt += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["netamount"].ToString());
                        //billbalance += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["recoverableamt"].ToString());
                        //gtbillbalance += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["recoverableamt"].ToString());
                        invbalance += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["invbalance"].ToString());
                        gtinvbalance += Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["invbalance"].ToString());
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["rcinfo"].ToString() + "</td>");
                        sb.Append("</tr>");
                        sn += 1;
                    }
                }
                sb.Append("<tr class='tblrow'>");//group-total
                sb.Append("<td>&nbsp;</td>");
                sb.Append("<td style='text-align:left;' colspan='4'>");
                sb.Append("<b>" + "Group Total For " + arlGroup[x].ToString());
                sb.Append("</b></td>");
                sb.Append("<td align='right'><b>" + mc.getINRCFormat(netamt) + "</b></td>");
                //sb.Append("<td align='right'><b>" + mc.getINRCFormat(billbalance) + "</b></td>");
                sb.Append("<td align='right'><b>" + mc.getINRCFormat(invbalance) + "</b></td>");
                sb.Append("<td colspan='2'></td>");
                sb.Append("</td></tr>");
            }
            sb.Append("<tr class='tblrow'>");//grand-total
            sb.Append("<td>&nbsp;</td>");
            sb.Append("<td style='text-align:left;' colspan='4'>");
            sb.Append("<b>Grand Total</b></td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(gtnetamt) + "</b></td>");
            //sb.Append("<td align='right'><b>" + mc.getINRCFormat(gtbillbalance) + "</b></td>");
            sb.Append("<td align='right'><b>" + mc.getINRCFormat(gtinvbalance) + "</b></td>");
            sb.Append("<td colspan='2'></td>");
            sb.Append("</td></tr>");
            sb.Append("</table><br/>");
            
            sb.Append("<div>");
            sb.Append("Note: Days Pending from Invoice Date, Sorted on Company + Invoice Date + Invoice No");
            sb.Append("</div>");

            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        [HttpGet]
        public FileResult getPendingPaymentReport(string potype, DateTime dtfrom, DateTime dtto, int railwayid, int compcode, bool billos=false)
        {
            //[100110]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            if (billos == true)
            {
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "PendingPaymentRpt_V2.rpt"));
            }
            else
            {
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "PendingPaymentRpt.rpt"));
            }
            setLoginInfo(rptDoc);
            //
            string potypename = mc.getNameByKey(mc.getPOTypesRpt(), "potype", potype, "potypename");
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Invoice Date From: " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            txtrpthead.Text += "\r\nPO Type: " + potypename;
            if (railwayid > 0)
            {
                DataSet ds = rptBll.getRailwayData();
                txtrpthead.Text += ", Railway : " + mc.getNameByKey(ds, "railwayid", railwayid.ToString(), "railwayname");
            }
            //setLoginInfo(rptDocSub);
            //dbp parameters- usp_rpt_pending_payment_report
            rptDoc.SetParameterValue("@potype", potype);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@railwayid", railwayid);
            rptDoc.SetParameterValue("@compcode", compcode);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
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

        public ActionResult getPendingPaymentReportCSV(string potype, DateTime dtfrom, DateTime dtto, int railwayid, int compcode)
        {
            //[100110]-Excel
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            string rptname = "Pending Payment Report";
            ERP_V1_ReportBLL rptBLL = new ERP_V1_ReportBLL();
            DataSet ds = new System.Data.DataSet();
            //to be changed for billos
            ds = rptBLL.getPendingPaymentReportCSVData(potype, dtfrom, dtto, railwayid, compcode);
            //
            DataTable dtr = new DataTable();
            dtr.Columns.Add("SNo");//1
            dtr.Columns.Add("Company");//2
            dtr.Columns.Add("CaseFileNo");//3
            dtr.Columns.Add("Railway");//4
            dtr.Columns.Add("Bill Balance");//8
            dtr.Columns.Add("Invoice Balance");//9
            dtr.Columns.Add("Paying Authority");//12
            dtr.Columns.Add("Consignee");//13
            dtr.Columns.Add("PONumber");//14
            dtr.Columns.Add("PODate");//15
            dtr.Columns.Add("Items");//16
            dtr.Columns.Add("Remarks");//11
            dtr.Columns.Add("Days Pending");//10
            dtr.Columns.Add("Invoice No");//5
            dtr.Columns.Add("Invoice Date");//6
            dtr.Columns.Add("Invoice Amount");//7
            DataRow dr = dtr.NewRow();
            DateTime vdate = new DateTime();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                dr = dtr.NewRow();
                dr["SNo"] = (i + 1).ToString();
                dr["Company"] = ds.Tables[0].Rows[i]["CmpShortName"].ToString();
                dr["Invoice No"] = ds.Tables[0].Rows[i]["BillNo"].ToString();
                dr["Railway"] = ds.Tables[0].Rows[i]["RailwayName"].ToString();
                vdate = Convert.ToDateTime(ds.Tables[0].Rows[i]["vdate"].ToString());
                dr["Invoice Date"] = mc.getStringByDateDDMMYYYY(vdate);
                dr["Invoice Amount"] = ds.Tables[0].Rows[i]["NetAmount"].ToString();
                dr["Bill Balance"] = ds.Tables[0].Rows[i]["RecoverableAmt"].ToString();
                dr["Invoice Balance"] = ds.Tables[0].Rows[i]["InvBalance"].ToString();
                dr["Days Pending"] = ds.Tables[0].Rows[i]["DaysPending"].ToString();
                dr["CaseFileNo"] = ds.Tables[0].Rows[i]["CaseFileNo"].ToString();
                dr["Remarks"] = ds.Tables[0].Rows[i]["RCInfo"].ToString();
                dr["Paying Authority"] = ds.Tables[0].Rows[i]["PayingAuthName"].ToString();
                dr["Consignee"] = ds.Tables[0].Rows[i]["ConsigneeName"].ToString();
                dr["PONumber"] = "";
                dr["PODate"] = "";
                if (ds.Tables[0].Rows[i]["PONumber"].ToString().Length > 0)
                {
                    vdate = Convert.ToDateTime(ds.Tables[0].Rows[i]["podate"].ToString());
                    dr["PONumber"] = "[" + ds.Tables[0].Rows[i]["PONumber"].ToString() + "]";
                    dr["PODate"] = mc.getStringByDateDDMMYYYY(vdate);
                }
                dr["Items"] = ds.Tables[0].Rows[i]["DispItemCode"].ToString().Replace("<br/>","-");
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
            string potypename = mc.getNameByKey(mc.getPOTypesRpt(), "potype", potype, "potypename");
            string rptheader = "Invoice Date From: " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            rptheader += ", PO Type: " + potypename;
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
            htw.WriteLine("---End of Report---[100110], Note: Days Pending from Invoice Date, Sorted on Company + Invoice Date + Invoice No, Paying Authority & Consignee for Railway + Private PO Only. Not from BillOS.");
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
        //

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
