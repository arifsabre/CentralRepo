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
    public class BirthdayAnniversaryReportController : Controller
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
            ViewBag.MonthList = new SelectList(mc.getMonthList(), "Value", "Text");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            rptOptionMdl rptOption = new rptOptionMdl();
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult DisplayReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            bool p1 = mc.getPermission(Entry.Employee_Report, permissionType.Add);
            bool p2 = mc.getPermission(Entry.BirthdayAnnivAlert, permissionType.Add);
            bool viewper = false;
            if (p1 == true || p2 == true)
            {
                viewper = true;
            }
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            p1 = mc.getPermission(Entry.Employee_Report, permissionType.Edit);
            p2 = mc.getPermission(Entry.BirthdayAnnivAlert, permissionType.Edit);
            bool downloadper = false;
            if (p1 == true || p2 == true)
            {
                downloadper = true;
            }
            //
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            string methodname = "GetReportHTML";
            if (rptOption.ReportFormat == "Excel") { methodname = "getReportFileCSV"; };
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "EmployeeFormsReport/" + methodname;
                string reportpms = "monthfrom=" + rptOption.VNoFrom + "";
                reportpms += "monthto = " + rptOption.VNoTo + "";
                reportpms += "grade = " + rptOption.Grade + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction(methodname, new { monthfrom = rptOption.VNoFrom, monthto = rptOption.VNoTo, grade = rptOption.Grade });
        }

        //get
        public ActionResult GetReportHTML(int monthfrom, int monthto, string grade)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            //dataset from dbProcedures/HR_Reports_SP
            //dbp-usp_get_birthday_anniversary_report
            EmployeeReportBLL rptBLL = new EmployeeReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.getBirthdayAnniversaryReportHtml(monthfrom, monthto, grade);

            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            sbHeader.Append("<div style='text-align:center;'>");//div main
            sbHeader.Append("<div style='font-size:10pt;color:red;'>* Under Trial</div>");
            //company
            sbHeader.Append("<div style='font-size:12pt;'>");
            sbHeader.Append("PRAG GROUP");
            sbHeader.Append("</div>");
            //cmp address
            //sbHeader.Append("<div style='font-size:10pt;'>");
            //sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpFooter1"].ToString());
            //sbHeader.Append("</div>");
            //repoprt name
            sbHeader.Append("<div style='font-size:11pt;background-color:lightgray;'>");
            sbHeader.Append("<b><u>Employee Birthday/Anniversary Report</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            string mfrom = mc.getNameByKey(mc.getMonths(), "monthid", monthfrom.ToString(), "monthname");
            string mto = mc.getNameByKey(mc.getMonths(), "monthid", monthto.ToString(), "monthname");
            string rptFilter = "";
            if (monthfrom == monthto)
            {
                rptFilter = "List of Employees for Birthday/Anniversary in " + mfrom;
            }
            else
            {
                rptFilter = "List of Employees for Birthday/Anniversary from " + mfrom + " To " + mto;
            }
            string gradename = "Worker";
            if (grade.ToLower() != "w")
            {
                gradename = "Other than Worker";
            }
            rptFilter += ", Grade: " + gradename;
            sbHeader.Append(rptFilter);
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            //report content
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>EMP&nbsp;ID</th>");
            sb.Append("<th style='width:auto;'>Employee&nbsp;Name</th>");
            sb.Append("<th style='width:15px;'>Contact&nbsp;No</th>");
            sb.Append("<th style='width:15px;'>For</th>");
            sb.Append("<th style='width:15px;'>Date</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["EmpId"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["EmpName"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["ContactNo"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["DateFor"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["DateStr"].ToString() + "</td>");
            }
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        [HttpGet]
        public ActionResult getReportFileCSV(int monthfrom, int monthto, string grade)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            string rptname = "Employee Birthday/Anniversary Report";
            EmployeeReportBLL objBLL = new EmployeeReportBLL();
            DataSet ds = new DataSet();
            ds = objBLL.getBirthdayAnniversaryReportHtml(monthfrom, monthto, grade);
            //
            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = ds.Tables[0];
            grid.DataBind();
            //
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + rptname + ".xls");
            Response.ContentType = "application/ms-excel";
            //
            Response.Charset = "";
            //
            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            //
            string mfrom = mc.getNameByKey(mc.getMonths(), "monthid", monthfrom.ToString(), "monthname");
            string mto = mc.getNameByKey(mc.getMonths(), "monthid", monthto.ToString(), "monthname");
            string rptheader = "";
            if (monthfrom == monthto)
            {
                rptheader = "List of Employees for Birthday/Anniversary in " + mfrom;
            }
            else
            {
                rptheader = "List of Employees for Birthday/Anniversary from " + mfrom + " To " + mto;
            }
            string gradename = "Worker";
            if (grade.ToLower() != "w")
            {
                gradename = "Other than Worker";
            }
            rptheader += ", Grade: " + gradename;
            //
            //company name
            htw.AddAttribute("CustomAttribute", "CustomAttributeValue");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "16pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "bold");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.TextAlign, "center");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine("PRAG GROUP OF INDUSTRIES");
            htw.WriteBreak();
            //report header
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "14pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "bold");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.TextAlign, "center");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine(rptheader);
            htw.WriteBreak();
            //
            grid.RenderControl(htw);
            //
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontSize, "10pt");
            htw.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "regular");
            htw.AddStyleAttribute("Customstyle", "CustomStyleValue");
            htw.RenderBeginTag(System.Web.UI.HtmlTextWriterTag.Span);
            htw.WriteLine("---End of Report---, Sorted on: Birth Date/Anniversary Date, Run Date: " + mc.getDateTimeString(DateTime.Now) + ".");
            htw.WriteBreak();
            //
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            //
            return View();
            //
        }

    }
}
