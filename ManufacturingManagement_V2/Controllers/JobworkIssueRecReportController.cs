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
    public class JobworkIssueRecReportController : Controller
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

        #region itc-4
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname",objCookie.getCompCode());
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            ViewBag.JobworkStatusList = new SelectList(mc.getJobworkStatusList(), "Value", "Text");
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult ITC4Report(rptOptionMdl rptOption)
        {
            //[100103]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Jobwork_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Jobwork_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "JobworkIssueRecReport/getITC4ReportFile";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&opt=" + rptOption.Grade + "";
                reportpms += "&vendorid=" + rptOption.GroupId + "";
                reportpms += "&vendorname=" + rptOption.GroupName + "";
                reportpms += "&challanno=" + rptOption.EmpName + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getITC4ReportFile", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, opt = rptOption.Grade, vendorid=rptOption.GroupId, vendorname=rptOption. GroupName, challanno=rptOption.EmpName  });
        }

        [HttpGet]
        public FileResult getITC4ReportFile(DateTime dtfrom, DateTime dtto, string opt = "p", int vendorid=0, string vendorname="", string challanno="")
        {
            //[100103]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/Jobwork/"), "ITC4Report.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula = "{vw_order_report.orderno}=" + orderno + " and {vw_order_report.compcode}=" + objCookie.getCompCode() + " and {vw_order_report.finyear}='" + objCookie.getFinYear() + "'";
            if (challanno == null) { challanno = ""; };
            if (vendorname == null) { vendorname = ""; };
            if (vendorname == "") { vendorid = 0; };
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpttitle"];
            if (opt == "0")
            {
                txtrpttitle.Text = "Challan Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            }
            else if (opt.ToLower() == "allpending")
            {
                txtrpttitle.Text = "Challan Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
                txtrpttitle.Text += ", Status: All Pending";
            }
            else if (opt.ToLower() == "partialrec")
            {
                txtrpttitle.Text = "From Challan Date " + mc.getStringByDate(dtfrom) + " Upto Receiving Date " + mc.getStringByDate(dtto);
                txtrpttitle.Text += ", Status: Partially Received";
            }
            else if (opt.ToLower() == "totalrec")
            {
                txtrpttitle.Text = "Receiving Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
                txtrpttitle.Text += ", Status: Total Received";
            }
            if (vendorname.Length > 0)
            {
                txtrpttitle.Text += ", Vendor: " + vendorname;
            }
            if (challanno.Length > 0)
            {
                txtrpttitle.Text += ", Challan No: " + challanno;
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters for- usp_jobwork_issue_receipt
            rptDoc.SetParameterValue("@opt", opt);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@vendorid", vendorid);
            rptDoc.SetParameterValue("@challanno", challanno);
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(objCookie.getCompCode()));
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

        [HttpPost]
        public ActionResult CancelledJobworkReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Jobwork_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Jobwork_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "JobworkIssueRecReport/getCancelledJobworkReport";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&vendorid=" + rptOption.GroupId + "";
                reportpms += "&vendorname=" + rptOption.GroupName + "";
                reportpms += "&challanno=" + rptOption.EmpName + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getCancelledJobworkReport", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, vendorid = rptOption.GroupId, vendorname = rptOption.GroupName, challanno = rptOption.EmpName });
        }

        [HttpGet]
        public FileResult getCancelledJobworkReport(DateTime dtfrom, DateTime dtto, int vendorid = 0, string vendorname = "", string challanno = "")
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/Jobwork/"), "CancelledJObworkRpt.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            if (challanno == null) { challanno = ""; };
            if (vendorname == null) { vendorname = ""; };
            if (vendorname == "") { vendorid = 0; };
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpttitle"];
            txtrpttitle.Text = "Challan Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (vendorname.Length > 0)
            {
                txtrpttitle.Text += ", Vendor: " + vendorname;
            }
            if (challanno.Length > 0)
            {
                txtrpttitle.Text += ", Challan No: " + challanno;
            }
            
            //usp_cancelled_jobwork_report
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@vendorid", vendorid);
            rptDoc.SetParameterValue("@challanno", challanno);
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(objCookie.getCompCode()));
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

        #region jobwork challan
        public ActionResult JobworkChallanView()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult DisplayChallanReport(rptOptionMdl rptOption)
        {
            //[100174]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Jobwork_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Jobwork_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "JobworkIssueRecReport/getJobworkChallanFile";
                string reportpms = "ccode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                reportpms += "&challanno=" + rptOption.InvoiceNo + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getJobworkChallanFile", new { dispid = 0, ccode=rptOption.CompCode, finyear=rptOption.FinYear, challanno=rptOption.InvoiceNo });
        }

        [HttpGet]
        public ActionResult JobworkChallanReport(int dispid)
        {
            //[100174]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (dispid == 0)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            bool viewper = mc.getPermission(Entry.Jobwork_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Jobwork_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "JobworkIssueRecReport/getJobworkChallanFile";
                string reportpms = "dispid=" + dispid + "";
                //reportpms += "&opt=" + rptOption.Grade + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getJobworkChallanFile", new { dispid = dispid });
        }

        public ActionResult getJobworkChallanFile(int dispid = 0, int ccode=0, string finyear="", string challanno="")
        {
            //[100174]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            JobworkIssueBLL jwBll = new JobworkIssueBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/Jobwork/"), "JobworkChallanRpt.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            CrystalDecisions.CrystalReports.Engine.TextObject txtNumToWords = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtNumToWords"];
            txtNumToWords.Text = mc.getWordByNumericDouble(jwBll.getJobworkItemAmount(dispid,ccode,finyear,challanno).ToString());
            //dbp parameters
            rptDoc.SetParameterValue("@dispid", dispid);
            rptDoc.SetParameterValue("@compcode", ccode);
            rptDoc.SetParameterValue("@finyear", finyear);
            rptDoc.SetParameterValue("@challanno", challanno);
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
