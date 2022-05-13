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
    public class AdvanceReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();

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
            rptOption.ReportDate = DateTime.Now;
            rptOption.DateFrom = DateTime.Now;
            rptOption.DateTo = DateTime.Now;
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            return View(rptOption);
        }

        #region advance outstanding

        [HttpPost]
        public ActionResult AdvanceOutStanding(rptOptionMdl rptOption)
        {
            //[100062]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Employee_Advance_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Employee_Advance_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "AdvanceReport/getAdvanceOutstandingReportFile";
                string reportpms = "compcode=" + rptOption.CompCode.ToString() + "";
                reportpms += "&vdate=" + mc.getStringByDate(rptOption.ReportDate) + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAdvanceOutstandingReportFile", new { compcode = rptOption.JoiningUnit, vdate = rptOption.ReportDate });
        }

        public ActionResult getAdvanceOutstandingReportFile(int compcode, DateTime vdate)
        {
            //[100062]
            if (mc.isValidToDisplayReport() == false)
            {
                //return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (mc.isValidDate(vdate) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid date selected!</h1></a>");
            }
            //
            AdvanceReportBLL rptBLL = new AdvanceReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AdvanceOutstandingRpt.rpt"));//incrpt
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["tbl_advancereport"].Merge(rptBLL.getAdvanceOutStanding(compcode,vdate));
            //additional values
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            if (compMdl.CompCode == 6)
            {
                compMdl.CmpName = "PRAG POLYMERS";
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtcmpname = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtcmpname"];
            txtcmpname.Text = compMdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtaddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtaddress"];
            txtaddress.Text = compMdl.Address1 + compMdl.Address2 + compMdl.Address3;
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtrpttitle"];
            txtrpttitle.Text = "Advance outstanding as on date: " + mc.getStringByDate(vdate);
            //
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

        #region advance deduction list

        [HttpPost]
        public ActionResult AdvanceDeductionList(rptOptionMdl rptOption)
        {
            //[100063]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Employee_Advance_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Employee_Advance_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "AdvanceReport/getAdvanceDeductionListReportFile";
                string reportpms = "compcode=" + rptOption.CompCode.ToString() + "";
                reportpms += "&vdate=" + mc.getStringByDate(rptOption.ReportDate) + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAdvanceDeductionListReportFile", new { compcode = rptOption.JoiningUnit, vdate = rptOption.ReportDate });
        }

        public ActionResult getAdvanceDeductionListReportFile(int compcode, DateTime vdate)
        {
            //[100063]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            if (mc.isValidDate(vdate) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid date selected!</h1></a>");
            }
            //
            AdvanceReportBLL rptBLL = new AdvanceReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AdvanceDeductionListRpt.rpt"));
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["tbl_advancereport"].Merge(rptBLL.getAdvanceDeductionList(compcode,vdate));
            //additional values
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(compcode);
            if (compMdl.CompCode == 6)
            {
                compMdl.CmpName = "PRAG POLYMERS";
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtcmpname = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtcmpname"];
            txtcmpname.Text = compMdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtaddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtaddress"];
            txtaddress.Text = compMdl.Address1 + compMdl.Address2 + compMdl.Address3;
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtrpttitle"];
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", vdate.Month.ToString(), "monthname");
            txtrpttitle.Text = "Advance Deduction List for: " + monthname + ", " + vdate.Year.ToString();
            //
            rptDoc.SetDataSource(dsr);
            //rd.Subreports[0].SetDataSource(dsr);
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

        #region advance deduction ledger

        [HttpPost]
        public ActionResult AdvanceDeductionLedger(rptOptionMdl rptOption)
        {
            //[100064]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Employee_Advance_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Employee_Advance_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "AdvanceReport/getAdvanceDeductionLedgerReportFile";
                string reportpms = "compcode=" + rptOption.CompCode.ToString() + "";
                reportpms += "&vdate=" + mc.getStringByDate(rptOption.ReportDate) + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAdvanceDeductionLedgerReportFile", new { newempid = rptOption.NewEmpId, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo });
        }

        [HttpPost]
        public ActionResult AdvanceDeductionLedgerByUser(rptOptionMdl rptOption)
        {
            //[100064]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            Session["xsid"] = objCookie.getUserId();
            return RedirectToAction("getAdvanceDeductionLedgerReportFile", new { newempid = rptOption.NewEmpId, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo });
        }

        [HttpGet]
        public ActionResult getAdvanceDeductionLedgerReportFile(int newempid, DateTime dtfrom, DateTime dtto)
        {
            //[100064]
            if (mc.isValidToDisplayReport() == false)//note
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            if (newempid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Employee not selected!</h1></a>");
            }
            if (mc.isValidDate(dtfrom) == false || mc.isValidDate(dtto) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid date selected!</h1></a>");
            }
            //
            AdvanceReportBLL rptBLL = new AdvanceReportBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rdSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AdvanceDeductionLedgerRpt.rpt"));
            //rdSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Test_SubReport.rpt"));
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["tbl_empadvledger"].Merge(rptBLL.getEmployeeAdvanceDeductionLedger(newempid,dtfrom,dtto));
            //additional values
            //getting employee detail
            EmployeeMdl empmdl = new EmployeeMdl();
            empmdl = empBLL.searchEmployeeByNewEmpId(newempid);
            string category = mc.getNameByKey(mc.getEmpCategory(), "categoryid", empmdl.CategoryId.ToString(), "categoryname");
            //getting company name and address
            CompanyMdl compMdl = new CompanyMdl();
            compMdl = compBLL.searchObject(empmdl.JoiningUnit);
            if (compMdl.CompCode == 6)
            {
                compMdl.CmpName = "PRAG POLYMERS";
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtcmpname = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtcmpname"];
            txtcmpname.Text = compMdl.CmpName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtaddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtaddress"];
            txtaddress.Text = compMdl.Address1 + compMdl.Address2 + compMdl.Address3;
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtrpttitle"];
            txtrpttitle.Text ="Emp. Id: "+ empmdl.EmpId + ", Emp. Name: " + empmdl.EmpName + ", Father's Name: " + empmdl.FatherName;
            txtrpttitle.Text += "\r\nDate From: " + mc.getStringByDate(dtfrom) + " To: " + mc.getStringByDate(dtto);
            //
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

        #region advance balance without deduction

        [HttpPost]
        public ActionResult BalanceWithoutDeduction(rptOptionMdl rptOption)
        {
            //[100065]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Employee_Advance_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Employee_Advance_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SalaryReport/getBalanceWithoutDeductionReport";
                string reportpms = "compcode=" + rptOption.JoiningUnit + "";
                reportpms += "&vdate=" + mc.getStringByDateForReport(rptOption.ReportDate) + "";
                //reportpms += "&rptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getBalanceWithoutDeductionReport", new { compcode = rptOption.JoiningUnit, vdate = rptOption.ReportDate });
        }

        [HttpGet]
        public FileResult getBalanceWithoutDeductionReport(int compcode, DateTime vdate, string rptformat="pdf")
        {
            //[100065]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AdvanceBalanceWtDeduction.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", vdate.Month.ToString(), "monthname");
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Salary Month " + monthname + " - " + vdate.Year.ToString();
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters--usp_get_advance_balance
            rptDoc.SetParameterValue("@vdate", vdate);
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
