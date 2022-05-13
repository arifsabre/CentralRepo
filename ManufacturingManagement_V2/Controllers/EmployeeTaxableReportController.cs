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
    public class EmployeeTaxableReportController : Controller
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
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname");
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            rptOption.FinYear = objCookie.getFinYear();
            rptOption.Amount = 250000;
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult DisplayReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            bool viewper = mc.getPermission(Entry.Salary_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Salary_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (rptOption.EmpName == null) { rptOption.NewEmpId = 0; };
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "EmployeeTaxableReport/GetReportHTML";
                string reportpms = "ccode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                reportpms += "&newempid=" + rptOption.NewEmpId + "";
                reportpms += "&addsummary=" + rptOption.Detailed + "";
                reportpms += "&filterbyamt=" + rptOption.Amount + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { ccode = rptOption.CompCode, finyear = rptOption.FinYear, newempid=rptOption.NewEmpId, addsummary=rptOption.Detailed, filterbyamt=rptOption.Amount });
        }

        //get
        public ActionResult GetReportHTML(int ccode, string finyear, int newempid, bool addsummary, double filterbyamt = 0)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            //dataset from dbProcedures/EmployeeTaxableRPT_SP.sql
            EmployeeReportBLL rptBLL = new EmployeeReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetEmployeeTaxableReportHtml(ccode, finyear, newempid, addsummary, filterbyamt);
            
            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            sbHeader.Append("<div style='text-align:center;'>");//div main
            sbHeader.Append("<div style='font-size:10pt;color:red;'>* Under Trial</div>");
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
                sbHeader.Append("<b><u>Employee Taxable Report</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
                sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString());
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();// ds.Tables["tbl"].Rows[0]["rptheader"].ToString();

            //report content
            int detslno = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
                sb.Append("<thead>");
                    sb.Append("<tr>");
                    sb.Append("<th style='width:15px;'>SlNo</th>");
                    sb.Append("<th style='width:15px;'>EMP&nbsp;ID</th>");
                    sb.Append("<th style='width:15px;'>Grade</th>");
                    sb.Append("<th style='width:auto;'>Name</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>Current<br/>Gross&nbsp;Salary</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>Salary&nbsp;[A]</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>Bonus&nbsp;[B]</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>ExGratia&nbsp;[C]</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>Others&nbsp;[D]</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>Total=<br/>[A]&nbsp;To&nbsp;[D]</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>TDS<br/>Deduction</th>");
                    sb.Append("</tr>");
                    sb.Append("</thead>");
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                //tr-1/2
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["EmpId"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["GradeName"].ToString() + "</td>");
                //
                string strlnk = "<a target='_new' href='../FinancialLedgerReport/FinLedgerEmployee";
                strlnk += "?newempid=" + ds.Tables["tbl1"].Rows[i]["NewEmpId"].ToString() + "";
                strlnk += "&finyear=" + finyear + "'>";
                strlnk += "" + ds.Tables["tbl1"].Rows[i]["EmpName"].ToString() + "</a>";
                sb.Append("<td>" + strlnk + "</td>");
                //
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["grosssalary"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["FYSalary"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["FYBonus"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["FYExGratia"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["FYOthers"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["FYTotal"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["FYTdsDeduction"].ToString())) + "</td>");
                sb.Append("</tr>");
                //tr-2/2
                if (addsummary == true)
                {
                    sb.Append("<tr class='tblrow'>");
                    sb.Append("<td></td>");//slno
                    sb.Append("<td colspan='10'>");//salary, ex-gratia & debits
                    sb.Append("<table class='tblcontainer' style='width:100%;'>");
                    sb.Append("<thead>");
                    sb.Append("<tr>");
                    sb.Append("<th style='width:15px;'>SlNo</th>");
                    sb.Append("<th style='width:15px;'>Month</th>");
                    sb.Append("<th style='width:15px;'>Year</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>Salary&nbsp;[A]</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>Bonus&nbsp;[B]</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>Incentive</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>ExGratia&nbsp;[C]</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>Others&nbsp;[D]</th>");
                    sb.Append("<th style='width:15px;text-align:right;'>TDS&nbsp;Deduction</th>");
                    sb.Append("</tr>");
                    sb.Append("</thead>");
                    detslno = 1;
                    for (int j = 0; j < ds.Tables["tbl2"].Rows.Count; j++)
                    {
                        if (ds.Tables["tbl2"].Rows[j]["newempid"].ToString() == ds.Tables["tbl1"].Rows[i]["newempid"].ToString())
                        {
                            sb.Append("<tr class='tblrow'>");
                            sb.Append("<td>" + detslno.ToString() + "</td>");
                            sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["mthname"].ToString() + "</td>");
                            sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["attyear"].ToString() + "</td>");
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["totalearned"].ToString())) + "</td>");
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["bonusamount"].ToString())) + "</td>");
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["totalpaidinc"].ToString())) + "</td>");
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["ExGratia"].ToString())) + "</td>");
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["DrAmount"].ToString())) + "</td>");
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["TDSDeduction"].ToString())) + "</td>");
                            sb.Append("</tr>");
                            detslno += 1;
                        }
                    }
                    sb.Append("</table>");
                    sb.Append("</td>");//salary, ex-gratia & debits
                    sb.Append("</tr>");
                }
            }
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();
            
            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        [HttpGet]
        public ActionResult GetReportHTML_1X(int ccode, string finyear, int newempid, bool addsummary)
        {
            //OK -separate details display
            //from dbProcedures/EmployeeTaxableRPT_SP.sql
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            EmployeeReportBLL rptBLL = new EmployeeReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetEmployeeTaxableReportHtml(ccode, finyear, newempid, addsummary, 0);
            //
            ReportModelObject.ReportHeader = ds.Tables["tbl"].Rows[0]["rptheader"].ToString();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>EmpId</th>");
            sb.Append("<th style='width:15px;'>Grade</th>");
            sb.Append("<th style='width:auto;'>EmpName</th>");
            sb.Append("<th style='width:auto;'>FatherName</th>");
            sb.Append("<th style='width:15px;text-align:center;'>GrossSalary</th>");
            sb.Append("<th style='width:15px;text-align:center;'>FYSalary</th>");
            sb.Append("<th style='width:15px;text-align:center;'>FYBonus</th>");
            sb.Append("<th style='width:15px;text-align:center;'>FYExGratia</th>");
            sb.Append("<th style='width:15px;text-align:center;'>FYTotal</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                //tr-1/3
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["EmpId"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["GradeName"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["EmpName"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["FatherName"].ToString() + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["grosssalary"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["FYSalary"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["FYBonus"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["FYExGratia"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["FYTotal"].ToString())) + "</td>");
                sb.Append("</tr>");
                //tr-2/2
                if (addsummary == true)
                {
                    sb.Append("<tr class='tblrow'>");
                    sb.Append("<td></td>");//slno
                    sb.Append("<td colspan='5'>");//salary detail
                    sb.Append("<table class='tblcontainer' style='width:100%;'>");
                    sb.Append("<thead>");
                    sb.Append("<tr>");
                    sb.Append("<th style='width:15px;'>SlNo</th>");
                    sb.Append("<th style='width:15px;'>Year</th>");
                    sb.Append("<th style='width:15px;'>Month</th>");
                    sb.Append("<th style='width:15px;text-align:center;'>TotalEarned</th>");
                    sb.Append("<th style='width:15px;text-align:center;'>BonusAmount</th>");
                    sb.Append("</tr>");
                    sb.Append("</thead>");
                    for (int j = 0; j < ds.Tables["tbl2"].Rows.Count; j++)
                    {
                        if (ds.Tables["tbl2"].Rows[j]["newempid"].ToString() == ds.Tables["tbl1"].Rows[i]["newempid"].ToString())
                        {
                            sb.Append("<tr class='tblrow'>");
                            sb.Append("<td>" + (j + 1).ToString() + "</td>");
                            sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["attyear"].ToString() + "</td>");
                            sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["attmonth"].ToString() + "</td>");
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["totalearned"].ToString())) + "</td>");
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["bonusamount"].ToString())) + "</td>");
                            sb.Append("</tr>");
                        }
                    }
                    sb.Append("</table>");
                    sb.Append("</td>");
                    sb.Append("<td colspan='4'>");//exgratia detail
                    sb.Append("<table class='tblcontainer' style='width:100%;'>");
                    sb.Append("<thead>");
                    sb.Append("<tr>");
                    sb.Append("<th style='width:15px;'>SlNo</th>");
                    sb.Append("<th style='width:15px;'>Year</th>");
                    sb.Append("<th style='width:15px;'>Month</th>");
                    sb.Append("<th style='width:15px;text-align:center;'>TotalPaid</th>");
                    sb.Append("<th style='width:15px;text-align:center;'>ExGratia</th>");
                    sb.Append("</tr>");
                    sb.Append("</thead>");
                    for (int k = 0; k < ds.Tables["tbl3"].Rows.Count; k++)
                    {
                        if (ds.Tables["tbl3"].Rows[k]["newempid"].ToString() == ds.Tables["tbl1"].Rows[i]["newempid"].ToString())
                        {
                            sb.Append("<tr class='tblrow'>");
                            sb.Append("<td>" + (k + 1).ToString() + "</td>");
                            sb.Append("<td>" + ds.Tables["tbl3"].Rows[k]["attyear"].ToString() + "</td>");
                            sb.Append("<td>" + ds.Tables["tbl3"].Rows[k]["attmonth"].ToString() + "</td>");
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl3"].Rows[k]["TotalPaid"].ToString())) + "</td>");
                            sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl3"].Rows[k]["ExGratia"].ToString())) + "</td>");
                            sb.Append("</tr>");
                        }
                    }
                    sb.Append("</table>");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }
            }
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();
            //
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

    }
}
