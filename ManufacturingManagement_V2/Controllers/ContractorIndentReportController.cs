using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ContractorIndentReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        readonly EmployeeBLL empBLL = new EmployeeBLL();
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
            //if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}

            setViewData();
            //ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            //ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            //ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            ContractorIndentMDI rptOption = new ContractorIndentMDI();
            //rptOption.DateTo = DateTime.Now;
            ///rptOption.DateFrom = DateTime.Now;
            return View(rptOption);
        }

        private void SetLoginInfo(CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc)
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
        [HttpPost]
        public ActionResult ContractorIndentPrintForm(ContractorIndentMDI rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.CivilIndentHOD, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.CivilIndentHOD, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ContractorIndentReport/ContractorIndentPrintFormFile";
                string reportpms = "&RecordId=" + (rptOption.RecordId) + "";
                //reportpms += "&ToPulse=" + (rptOption.ToPulse) + "";
                //reportpms += "&compcode=" + rptOption.CompCode + "";
                //reportpms += "&grade=" + rptOption.Grade + "";
                // reportpms += "&rpptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("ContractorIndentPrintFormFile", new { recordId = rptOption.RecordId });
        }
        [HttpGet]
        public ActionResult ContractorIndentPrintFormFile(string recordId)
        {
            //by usp_get_indent_report
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ContractorIndentApprovalReport.rpt"));//TestXCrystalReport1
            SetLoginInfo(rptDoc);
            //dbp --ZZZ_USP_GET_FirstIn_Lastout--ZZZ_USP_GET_FirstIn_Lastout_Night
            rptDoc.SetParameterValue("@RecordId", recordId);

            ///rptDoc.SetParameterValue("@gradecode", grade);
            //
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            //  System.IO.Stream stream = null;
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
            // stream.Seek(0, System.IO.SeekOrigin.Begin);
            // return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }
    }
}