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
    public class B_INOUT_MisssingController : Controller
    {
        //
        // GET: /B_OT_830/
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();
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
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");

            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.DateTo = DateTime.Now;
            rptOption.DateFrom = DateTime.Now;
            return View(rptOption);
        }

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
        [HttpPost]
        public ActionResult Get_B_InoutMising(rptOptionMdl rptOption)
        {
            //[100140]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Biometric_Data, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Biometric_Data, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "B_INOUT_Misssing/Get_B_InoutMisingFile";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&rpptformat=" + rptOption.ReportFormat + "";
                //reportpms += "compcode=" + rptOption.CompCode + "";
                // reportpms += "grade=" + rptOption.Grade + "";
                // reportpms += "DesId=" + rptOption.DesId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("Get_B_InoutMisingFile", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, rptformat = rptOption.ReportFormat });
        }

        public ActionResult Get_B_InoutMisingFile(DateTime dtfrom, DateTime dtto, string rptformat)
        {
            //[100140]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "B_FirstIn_OR_Lastout_Missing.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            rptDoc.SetParameterValue("@from", dtfrom);
            rptDoc.SetParameterValue("@to", dtto);
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
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
                return File(stream, "application/excel", "ArrivalDeparture.xls");
            }
            return File(stream, "application/pdf");
        }

        [HttpPost]
        public ActionResult Get_B_SendSMSReport(rptOptionMdl rptOption)
        {
            //[New]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Biometric_Data, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Biometric_Data, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "B_INOUT_Misssing/Get_B_SendSMSReport_File";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                //reportpms += "compcode=" + rptOption.CompCode + "";
                // reportpms += "grade=" + rptOption.Grade + "";
                // reportpms += "DesId=" + rptOption.DesId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("Get_B_SendSMSReport_File", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, rptformat = rptOption.ReportFormat });
        }

        [HttpPost]
        public ActionResult Get_B_AbsentListSMS(rptOptionMdl rptOption)
        {
            //[100141]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Biometric_Data, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Biometric_Data, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "B_INOUT_Misssing/Get_B_AbsentListSMSFile";

                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl });
            }
            //full access with download (no escalation)
            return RedirectToAction("Get_B_AbsentListSMSFile", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, rptformat = rptOption.ReportFormat });
        }
        
        public ActionResult Get_B_AbsentListSMSFile(string rptformat)
        {
            //[100141]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "B_DailyAbsentSMS.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
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
                return File(stream, "application/excel", "ArrivalDeparture.xls");
            }
            return File(stream, "application/pdf");
        }
    }
}
