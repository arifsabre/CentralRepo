
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;
using System.Text;

namespace ManufacturingManagement_V2.Controllers
{
    public class TenderReportV2Controller : Controller
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

        [HttpGet]
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.TenderStatusList = new SelectList(mc.getTenderStatusList(), "Value", "Text");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region tender report

        [HttpPost]
        public ActionResult DisplayTenderReport(rptOptionMdl rptOption)
        {
            //[100030]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Tender_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Tender_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            string methodName = "getTenderReport";
            if (rptOption.ReportFormat.ToLower() == "excel")
            {
                methodName = "getTenderReport_CSV";
            }
            //methodName = "getTenderReport_RDLC";//note
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "TenderReportV2/" + methodName + "";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&groupid=" + rptOption.GroupId + "";
                reportpms += "&itemid=" + rptOption.RmItemId + "";
                reportpms += "&railwayid=" + rptOption.AgentId + "";
                reportpms += "&tenderstatus=" + rptOption.EmpType + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction(methodName, new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, groupid = rptOption.GroupId, itemid = rptOption.RmItemId, railwayid = rptOption.AgentId, tenderstatus = rptOption.EmpType, compcode = rptOption.CompCode });
        }

        [HttpGet]
        public FileResult getTenderReport(DateTime dtfrom, DateTime dtto, int groupid, int itemid, int railwayid, string tenderstatus, int compcode)
        {
            //[100030]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "TenderReportV2.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Tender Report Opening Date From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            //dbp --usp_get_tender_report_v2
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@GroupId", groupid);
            rptDoc.SetParameterValue("@ItemId", itemid);
            rptDoc.SetParameterValue("@railwayid", railwayid);
            rptDoc.SetParameterValue("@tenderstatus", tenderstatus);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
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

        public ActionResult getTenderReport_CSV(DateTime dtfrom, DateTime dtto, int groupid, int itemid, int railwayid, string tenderstatus, int compcode)
        {
            //[100030]-Excel
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            string rptname = "Tender Report";
            ERP_V1_ReportBLL rptBLL = new ERP_V1_ReportBLL();
            DataSet ds = new System.Data.DataSet();
            ds = rptBLL.getTenderReportV2(dtfrom, dtto, groupid, itemid, railwayid, tenderstatus, compcode);
            //
            DataTable dtr = new DataTable();
            dtr.Columns.Add("SNo");
            dtr.Columns.Add("Company");
            dtr.Columns.Add("Railway");
            dtr.Columns.Add("Tender No");
            dtr.Columns.Add("Due On");
            dtr.Columns.Add("Item Group");
            dtr.Columns.Add("Item");
            dtr.Columns.Add("Item Desc");
            dtr.Columns.Add("Unit");
            dtr.Columns.Add("Tender Qty");
            dtr.Columns.Add("PO Qty");
            dtr.Columns.Add("Less Qty");
            dtr.Columns.Add("L1L2");
            dtr.Columns.Add("Status");
            dtr.Columns.Add("LOADate/PODate");
            dtr.Columns.Add("Remark");
            dtr.Columns.Add("Rate");
            dtr.Columns.Add("Basic Value");
            dtr.Columns.Add("Delivery Period");
            dtr.Columns.Add("Delivery Schedule");
            DataRow dr = dtr.NewRow();
            DateTime opdt = new DateTime();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                dr = dtr.NewRow();
                dr["SNo"] = (i + 1).ToString();
                dr["Company"] = ds.Tables[0].Rows[i]["CmpName"].ToString();
                opdt = Convert.ToDateTime(ds.Tables[0].Rows[i]["OpeningDate"].ToString());
                dr["Railway"] = ds.Tables[0].Rows[i]["RlyShortName"].ToString();
                dr["Tender No"] = "["+ds.Tables[0].Rows[i]["TenderNo"].ToString()+"]";
                dr["Item Group"] = ds.Tables[0].Rows[i]["GroupName"].ToString();
                dr["Due On"] = mc.getStringByDateDDMMYYYY(opdt);
                dr["Item"] = ds.Tables[0].Rows[i]["ShortName"].ToString();
                dr["Item Desc"] = ds.Tables[0].Rows[i]["ItemName"].ToString();
                dr["Unit"] = ds.Tables[0].Rows[i]["UnitName"].ToString();
                dr["Tender Qty"] = ds.Tables[0].Rows[i]["Qty"].ToString();
                dr["PO Qty"] = ds.Tables[0].Rows[i]["OurQty"].ToString();//note
                dr["Less Qty"] = Convert.ToDouble(dr["PO Qty"].ToString())- Convert.ToDouble(dr["Tender Qty"].ToString());
                dr["L1L2"] = ds.Tables[0].Rows[i]["L1L2"].ToString();
                dr["Status"] = ds.Tables[0].Rows[i]["TenderStatus"].ToString();
                dr["LOADate/PODate"] = "";
                if (ds.Tables[0].Rows[i]["PONumber"].ToString().Length > 0)
                {
                    dr["LOADate/PODate"] = ds.Tables[0].Rows[i]["PODate"].ToString();
                }
                else if (ds.Tables[0].Rows[i]["LOANumber"].ToString().Length > 0)
                {
                    dr["LOADate/PODate"] = ds.Tables[0].Rows[i]["LOADate"].ToString();
                }
                dr["Remark"] = ds.Tables[0].Rows[i]["Remarks"].ToString();
                dr["Rate"] = ds.Tables[0].Rows[i]["Rate"].ToString();
                dr["Basic Value"] = ds.Tables[0].Rows[i]["BasicValue"].ToString();
                dr["Delivery Period"] = ds.Tables[0].Rows[i]["TC2DelvPeriod"].ToString();
                dr["Delivery Schedule"] = ds.Tables[0].Rows[i]["DelvSchedule"].ToString();
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
            //for company name
            htw.AddAttribute("CustomAttribute", "CustomAttributeValue");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "16pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "bold");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.TextAlign, "center");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine(compMdl.CmpName);
            htw.WriteBreak();
            //for report name
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "12pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "bold");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.TextAlign, "center");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine(compMdl.Footer1);
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
            string rptheader = "Opening Date From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (railwayid != 0)
            {
                rptheader += ", Railway: " + ds.Tables[0].Rows[0]["RailwayName"].ToString();
            }
            if (groupid != 0)
            {
                rptheader += ", Item Group: " + ds.Tables[0].Rows[0]["GroupName"].ToString();
            }
            if (itemid != 0)
            {
                rptheader += ", Item: " + ds.Tables[0].Rows[0]["ShortName"].ToString();
            }
            if (tenderstatus != "0")
            {
                rptheader += ", Status: " + ds.Tables[0].Rows[0]["TenderStatus"].ToString();
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
            htw.WriteLine("Report---[100030], Sorting: Item Group + Due On, Note: Item Group Accessiblity is not checked in this report. Basic Value is of Tender Basic Value.");
            htw.WriteBreak();
            //
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            //
            return View();
            //
        }

        public ActionResult getTenderReport_RDLC(DateTime dtfrom, DateTime dtto, int groupid, int itemid, int railwayid, string tenderstatus, int compcode)
        {
            //[100030]-RDLC-[OK, but Not in Use]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            //string rptformat = "Excel";
            string rptformat = "PDF";
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/TenderReport/TenderReportV2Rdlc.rdlc");
            ERP_V1_ReportBLL rptBLL = new ERP_V1_ReportBLL();
            DataSet ds = new System.Data.DataSet();
            ds = rptBLL.getTenderReportV2(dtfrom, dtto, groupid, itemid, railwayid, tenderstatus, compcode);
            Reports.dsReport dsr = new Reports.dsReport();
            //
            dsr.Tables["dtRDLC1"].Rows.Clear();
            DataRow dr = dsr.Tables["dtRDLC1"].NewRow();
            DateTime opdt = new DateTime();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                dr = dsr.Tables["dtRDLC1"].NewRow();
                opdt = Convert.ToDateTime(ds.Tables[0].Rows[i]["OpeningDate"].ToString());
                dr["Col_1"] = ds.Tables[0].Rows[i]["RlyShortName"].ToString();
                dr["Col_2"] = ds.Tables[0].Rows[i]["TenderNo"].ToString() + " ";
                dr["Col_3"] = ds.Tables[0].Rows[i]["GroupName"].ToString();
                dr["Col_4"] = mc.getStringByDateDDMMYYYY(opdt);
                dr["Col_5"] = ds.Tables[0].Rows[i]["ShortName"].ToString();
                dr["Col_6"] = ds.Tables[0].Rows[i]["UnitName"].ToString();
                dr["Col_7"] = ds.Tables[0].Rows[i]["Qty"].ToString();
                dr["Col_8"] = ds.Tables[0].Rows[i]["TenderStatus"].ToString();
                dr["Col_9"] = ds.Tables[0].Rows[i]["Remarks"].ToString();
                dr["Col_10"] = ds.Tables[0].Rows[i]["Rate"].ToString();
                dr["Col_11"] = ds.Tables[0].Rows[i]["BasicValue"].ToString();
                dsr.Tables["dtRDLC1"].Rows.Add(dr);
            }
            //
            compBLL = new CompanyBLL();
            CompanyMdl compMdl = compBLL.searchObject(compcode);
            ReportParameter rp1 = new ReportParameter("prCmpName", compMdl.CmpName);
            ReportParameter rp2 = new ReportParameter("prCmpAddress", compMdl.Address1 + " " + compMdl.Address2 + " " + compMdl.Address3);
            ReportParameter rp3 = new ReportParameter("prReportName", "Tender Report");
            string rptheader = "Opening Date From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (railwayid != 0)
            {
                rptheader += ", Railway: " + ds.Tables[0].Rows[0]["RailwayName"].ToString();
            }
            if (groupid != 0)
            {
                rptheader += ", Item Group: " + ds.Tables[0].Rows[0]["GroupName"].ToString();
            }
            if (itemid != 0)
            {
                rptheader += ", Item: " + ds.Tables[0].Rows[0]["ShortName"].ToString();
            }
            if (tenderstatus != "0")
            {
                rptheader += ", Status: " + ds.Tables[0].Rows[0]["TenderStatus"].ToString();
            }
            ReportParameter rp4 = new ReportParameter("prRptHead", rptheader);
            localReport.SetParameters(new ReportParameter[] { rp1, rp2, rp3, rp4 });
            //
            ReportDataSource reportDataSource = new ReportDataSource("dsReport", dsr.Tables["dtRDLC1"]);
            localReport.DataSources.Add(reportDataSource);
            //
            string filename = "TenderReport." + mc.getNameByKey(mc.getReportFormats(), "format", rptformat, "ext");
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>'" + rptformat + "'</OutputFormat>" +
            "  <PageWidth>320mm</PageWidth>" +
            "  <PageHeight>210mm</PageHeight>" +
            "  <MarginTop>0.25in</MarginTop>" +
            "  <MarginLeft>0.45in</MarginLeft>" +
            "  <MarginRight>0.2in</MarginRight>" +
            "  <MarginBottom>0.2in</MarginBottom>" +
            "  <FileName>'" + filename + "'</FileName>" +
            "</DeviceInfo>";
            //
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                rptformat,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
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
