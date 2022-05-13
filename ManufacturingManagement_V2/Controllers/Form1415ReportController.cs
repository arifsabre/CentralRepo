using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using CrystalDecisions.Shared;

namespace ManufacturingManagement_V2.Controllers
{
    public class Form1415ReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();
        private UserBLL bllUser = new UserBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        public ActionResult Index(int newempid = 0, string empname = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.AttYear = DateTime.Now.Year;
            rptOption.DateFrom = DateTime.Now;
            rptOption.DateTo = DateTime.Now;
            ViewBag.NewEmpId = newempid;
            ViewBag.EmpName = empname;
            return View(rptOption);
        }

        private ParameterFields getReportParameters(ParameterField pf1,int pf1value)
        {
            ParameterFields pfs = new ParameterFields();
            //getting list of named parameters identical with crpt
            ParameterField pfMonthFrom = pf1;
            //ParameterField pfdwst = pf2;
            //getting list of discrete value objects with named parameters as above
            ParameterDiscreteValue pdvMonthFrom = new ParameterDiscreteValue();
            //ParameterDiscreteValue pdvdwst = new ParameterDiscreteValue();
            //settinng values into discrete value objects
            pdvMonthFrom.Value = pf1value;
            //pdvdwst.Value = chkDwSt.Checked;
            //add parameter to the list
            pfs.Add(pfMonthFrom);
            //pfs.Add(pfdwst);
            return pfs; //returning list of parameters with values
        }
        //

        #region form-14/15 report

        [HttpPost]
        public ActionResult Form1415Report(rptOptionMdl rptOption)
        {
            //[100094]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Attendance_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Attendance_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "Form1415Report/getForm1415ReportFile";
                string reportpms = "monthfrom=" + rptOption.VNoFrom + "";
                reportpms += "&monthto=" + rptOption.VNoTo + "";
                reportpms += "&attyear=" + rptOption.AttYear + "";
                reportpms += "&newempid=" + rptOption.NewEmpId + "";
                reportpms += "&formno=" + rptOption.FormNo + "";
                reportpms += "&printsum=" + rptOption.Detailed + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getForm1415ReportFile", new { monthfrom = rptOption.VNoFrom, monthto = rptOption.VNoTo, attyear = rptOption.AttYear, newempid = rptOption.NewEmpId, formno = rptOption.FormNo, printsum=rptOption.Detailed });
        }

        public ActionResult getForm1415ReportFile(int monthfrom, int monthto, int attyear, int newempid, string formno, bool printsum)
        {
            //[100094]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (newempid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Employee not selected!</h1></a>");
            }
            if (attyear < 2017)
            {
                //data not available in erp
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid calender year selected!</h1></a>");
            }
            if (monthfrom > 12 || monthto > 12)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid month entered!</h1></a>");
            }
            //
            //getting employee detail
            EmployeeMdl empmdl = new EmployeeMdl();
            empmdl = empBLL.searchEmployeeByNewEmpId(newempid);
            if (attyear < empmdl.JoiningDate.Year)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid calender year selected for the employee!</h1></a>");
            }
            //
            int indx = empmdl.EmpId.IndexOf('_') + 2;
            string numEmpId = empmdl.EmpId.Substring(indx,empmdl.EmpId.Length-indx);
            //
            string department = mc.getNameByKey(bllUser.getDepartmentData(), "depcode", empmdl.DepCode, "department");
            string category = mc.getNameByKey(mc.getEmpCategory(), "categoryid", empmdl.CategoryId.ToString(), "categoryname");
            //getting company name and address
            CompanyMdl cmpmdl = new CompanyMdl();
            cmpmdl = compBLL.searchObject(empmdl.JoiningUnit);
            if (cmpmdl.CompCode == 6)
            {
                cmpmdl.CmpName = "PRAG POLYMERS";
            }
            //
            Form1415ReportBLL rptBLL = new Form1415ReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //attmonth is used as printmonthfrom variable here
            if (monthto < monthfrom) { monthto = monthfrom; };
            if (monthfrom == 0)//new card
            {
                //[100094]/NCRPT
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Form1415Rpt.rpt"));
            }
            else if (monthfrom > 0)//pre-printed used
            {
                //s-note
                // Form1415Rpt as main and Form1415PrePrintedRpt as record lines
                // are correct in present scenario, and Form1415PrePrintedRpt_Img
                // is correct for old format main report only
                //e-note
                if (monthto < monthfrom) { monthto = monthfrom; };
                //[100094]/NCRPT
                rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Form1415PrePrintedRpt.rpt"));//for recent main report
                //rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Form1415PrePrintedRpt_Img.rpt"));//for old format only
            }
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["tbl_form1415"].Merge(rptBLL.getForm1415ReportData(attyear,newempid));
            //additional values for new card
            if (monthfrom == 0)//for new card
            {
                CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
                txtCmpName.Text = cmpmdl.CmpName;
                CrystalDecisions.CrystalReports.Engine.TextObject txtFormNo = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtFormNo"];
                txtFormNo.Text = formno;
                CrystalDecisions.CrystalReports.Engine.TextObject txtAddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress"];
                txtAddress.Text = cmpmdl.Footer1;//note
                txtAddress.Text = cmpmdl.Address1.ToUpper() + "\r\n" + cmpmdl.Address2.ToUpper();
                CrystalDecisions.CrystalReports.Engine.TextObject txtSlNo = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtSlNo"];
                txtSlNo.Text = numEmpId;
                CrystalDecisions.CrystalReports.Engine.TextObject txtDepCategory = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtDepCategory"];
                txtDepCategory.Text = department + ", " + category;
                CrystalDecisions.CrystalReports.Engine.TextObject txtRegSlNo = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtRegSlNo"];
                txtRegSlNo.Text = "";// rptOption.EmpId;
                CrystalDecisions.CrystalReports.Engine.TextObject txtJoiningDate = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtJoiningDate"];
                txtJoiningDate.Text = mc.getStringByDate(empmdl.JoiningDate);
                CrystalDecisions.CrystalReports.Engine.TextObject txtEmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtEmpName"];
                txtEmpName.Text = empmdl.EmpName;
                CrystalDecisions.CrystalReports.Engine.TextObject txtFatherName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtFatherName"];
                txtFatherName.Text = empmdl.FatherName;
            }
            dsr.Tables["dtRptVariables"].Rows.Clear();
            System.Data.DataRow dr = dsr.Tables["dtRptVariables"].NewRow();
            dr["numcol1"] = monthfrom;
            dr["numcol2"] = monthto;
            dr["numcol3"] = 0;
            if (printsum == true)
            {
                double sm = 0;
                for (int x = 0; x < dsr.Tables["tbl_form1415"].Rows.Count; x++)
                {
                    sm += Math.Round(Convert.ToDouble(dsr.Tables["tbl_form1415"].Rows[x]["DaysWorked"].ToString()),0,MidpointRounding.AwayFromZero);
                }
                dr["numcol3"] = sm;
            }
            dsr.Tables["dtRptVariables"].Rows.Add(dr);
            //
            rptDoc.SetDataSource(dsr);
            //rptDoc.Subreports[0].SetDataSource(dsr);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecordSelectionFormula = "";
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
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region leave statement report

        [HttpPost]
        public ActionResult LeaveStatementReport(rptOptionMdl rptOption)
        {
            //[100095]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Attendance_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Attendance_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "Form1415Report/getLeaveStatementReportFile";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&newempid=" + rptOption.NewEmpId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getLeaveStatementReportFile", new { newempid = rptOption.NewEmpId, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo });
        }

        [HttpPost]
        public ActionResult LeaveStatementReportByUser(rptOptionMdl rptOption)
        {
            //[100095]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            Session["xsid"] = objCookie.getUserId();
            return RedirectToAction("getLeaveStatementReportFile", new { newempid = rptOption.NewEmpId, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo });
        }

        [HttpGet]
        public ActionResult getLeaveStatementReportFile(int newempid, DateTime dtfrom, DateTime dtto)
        {
            //[100095]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (newempid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Employee not selected!</h1></a>");
            }
            //
            //getting employee detail
            EmployeeMdl empmdl = new EmployeeMdl();
            empmdl = empBLL.searchEmployeeByNewEmpId(newempid);
            string department = empmdl.DepCode;
            string category = mc.getNameByKey(mc.getEmpCategory(), "categoryid", empmdl.CategoryId.ToString(), "categoryname");
            //getting company name and address
            CompanyMdl cmpmdl = new CompanyMdl();
            cmpmdl = compBLL.searchObject(empmdl.JoiningUnit);
            if (cmpmdl.CompCode == 6)
            {
                cmpmdl.CmpName = "PRAG POLYMERS";
            }
            //
            Form1415ReportBLL rptBLL = new Form1415ReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "LeaveStatementRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["tbl_salary"].Merge(rptBLL.getLeaveStatementData(newempid, dtfrom, dtto));
            //additional values
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            txtCmpName.Text = cmpmdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress"];
            //txtAddress.Text = cmpmdl.Footer1;//note
            string mfrom = mc.getNameByKey(mc.getMonths(), "monthid", dtfrom.Month.ToString(), "monthname");
            string mto = mc.getNameByKey(mc.getMonths(), "monthid", dtto.Month.ToString(), "monthname");
            txtAddress.Text = cmpmdl.Address1.ToUpper() + "\r\n" + cmpmdl.Address2.ToUpper();
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtrpttitle"];
            txtrpttitle.Text = "From " + mfrom +"-"+ dtfrom.Year.ToString() + " To " + mto + "-" + dtto.Year.ToString() + ", Emp. Id: " + empmdl.EmpId + ", Employee Name: " + empmdl.EmpName + ", Father's Name: " + empmdl.FatherName;
            //
            rptDoc.SetDataSource(dsr);
            //rptDoc.Subreports[0].SetDataSource(dsr);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecorptDocSelectionFormula = "";
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

        #region leave summary report

        [HttpPost]
        public ActionResult LeaveSummaryReport(rptOptionMdl rptOption)
        {
            //[100096]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Attendance_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Attendance_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "AttendanceReport/getLeaveSummaryReportFile";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getLeaveSummaryReportFile", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, grade = rptOption.Grade, rptformat=rptOption.ReportFormat });
        }

        public ActionResult getLeaveSummaryReportFile(DateTime dtfrom, DateTime dtto, int compcode, string grade = "0", string rptformat = "pdf")
        {
            //[100096]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            string gradename = "All";
            if (grade != "0")
            {
                gradename = mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename");
            }
            string mfrom = mc.getNameByKey(mc.getMonths(), "monthid", dtfrom.Month.ToString(), "monthname");
            string mto = mc.getNameByKey(mc.getMonths(), "monthid", dtto.Month.ToString(), "monthname");
            //getting company name and address
            CompanyMdl cmpmdl = new CompanyMdl();
            cmpmdl = compBLL.searchObject(compcode);
            if (cmpmdl.CompCode == 6)
            {
                cmpmdl.CmpName = "PRAG POLYMERS";
            }
            //
            Form1415ReportBLL rptBLL = new Form1415ReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "LeaveSummaryRpt.rpt"));
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["tbl_salary"].Merge(rptBLL.getLeaveSummaryData(dtfrom,dtto,compcode,grade));
            //additional values
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtCmpName"];
            txtCmpName.Text = cmpmdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtAddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtAddress"];
            //txtAddress.Text = cmpmdl.Footer1;//note
            txtAddress.Text = cmpmdl.Address1.ToUpper() + "\r\n" + cmpmdl.Address2.ToUpper();
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtrpttitle"];
            txtrpttitle.Text = "From: " + mfrom + " " + dtfrom.Year.ToString() + "  To: " + mto + " " + dtto.Year.ToString()+", Grade: " + gradename;
            //
            rptDoc.SetDataSource(dsr);
            //rptDoc.Subreports[0].SetDataSource(dsr);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
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
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "SalaryReportHO.xls");
            }
            return File(stream, "application/pdf");
        }

        #endregion

    }
}
